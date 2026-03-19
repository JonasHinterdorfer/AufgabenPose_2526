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
    var bookingsCsv = await new CsvImport<Bookings>().ReadAsync("ImportData/Bookings.csv");

    //TODO
}

Console.WriteLine("done");