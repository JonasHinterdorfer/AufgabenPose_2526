using Base.Tools.CsvImport;

using Core.Entities;

using Import.ImportData;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Persistence;

using System;
using System.Linq;

using Core.Contracts;

var builder = Host.CreateApplicationBuilder(args);


var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString))
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
    var uow                = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var bookingsCsv        = await new CsvImport<Bookings>().ReadAsync("ImportData/Bookings.csv");
    var roomCsv            = await new CsvImport<Rooms>().ReadAsync("ImportData/Rooms.csv");

    var rooms = roomCsv.Select(r => new Room
    {
        RoomNumber = r.RoomNumber,
        RoomType   = (RoomType) r.RoomType
    }).ToList();
    await uow.Rooms.AddRangeAsync(rooms);
    
    var customers = bookingsCsv.Select(b =>new Customer
    {
        FirstName = b.FirstName,
        LastName  = b.LastName,
        IBAN = b.CreditCardNumber
    }).ToList(); 
    await uow.Customers.AddRangeAsync(customers);
    
    var bookings = bookingsCsv.Select(b => new Booking
    {
        From       = b.From,
        To         = b.To,
        Customer   = customers.First(c => c.FirstName == b.FirstName && c.LastName == b.LastName),
        Room       = rooms.First(r => r.RoomNumber == b.RoomNumber),
    }).ToList();
    await uow.Bookings.AddRangeAsync(bookings);

    await uow.SaveChangesAsync();
}

Console.WriteLine("done");

