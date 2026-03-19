using Base.Tools.CsvImport;

using Core.Entities;

using Import.ImportData;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Persistence;

using System;
using System.Collections.Generic;
using System.Linq;

using Core.Contracts;

var builder = Host.CreateApplicationBuilder(args);


var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString))
    .AddScoped<IUnitOfWork, UnitOfWork>()
    ;

var host = builder.Build();

Console.WriteLine("Migrate Database");

using (var scope = host.Services.CreateScope())
{
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    await uow.DeleteDatabaseAsync();
    await uow.MigrateDatabaseAsync();
}

Console.WriteLine("Import Data");

using (var scope = host.Services.CreateScope())
{
    var uow         = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var roomsCsv    = await new CsvImport<Rooms>().ReadAsync("ImportData/Rooms.csv");
    var bookingsCsv = await new CsvImport<Bookings>().ReadAsync("ImportData/Bookings.csv");

    // Import Rooms (upsert by RoomNumber)
    foreach (var r in roomsCsv)
    {
        var existing = (await uow.Rooms.GetNoTrackingAsync(x => x.RoomNumber == r.RoomNumber)).FirstOrDefault();
        if (existing is null)
        {
            var room = new Room
            {
                RoomNumber = r.RoomNumber,
                RoomType = (RoomType)r.RoomType
            };

            await uow.Rooms.AddAsync(room);
        }
    }

    await uow.SaveChangesAsync();

    // Import Bookings and Customers
    foreach (var b in bookingsCsv)
    {
        // find or create customer
        var cust = (await uow.Customers.GetNoTrackingAsync(c => c.FirstName == (b.FirstName ?? string.Empty) && c.LastName == b.LastName)).FirstOrDefault();
        if (cust is null)
        {
            cust = new Customer { FirstName = b.FirstName ?? string.Empty, LastName = b.LastName, Email = b.EmailAddress };
            await uow.Customers.AddAsync(cust);
            await uow.SaveChangesAsync(); // get id
        }

        // find room
        var room = (await uow.Rooms.GetNoTrackingAsync(r => r.RoomNumber == b.RoomNumber)).FirstOrDefault();
        if (room is null)
        {
            // create room if missing
            room = new Room { RoomNumber = b.RoomNumber, RoomType = RoomType.Standard };
            await uow.Rooms.AddAsync(room);
            await uow.SaveChangesAsync();
        }

        // create booking
        var booking = new Booking
        {
            From = b.From,
            To = b.To,
            CustomerId = cust.Id,
            RoomId = room.Id
        };

        await uow.Bookings.AddAsync(booking);
    }

    await uow.SaveChangesAsync();
}

Console.WriteLine("done");