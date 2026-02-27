using Logic;
using Logic.Tools;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IImportService, ImportService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var host = builder.Build();

await RecreateDatabaseAsync();
await Import();

async Task RecreateDatabaseAsync()
{
    Console.WriteLine("=====================");
    using (var scope = host.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Deleting database ...");
        await dbContext.Database.EnsureDeletedAsync();

        //        Console.WriteLine("Creating and migrating database ...");
        //        await dbContext.Database.EnsureCreatedAsync();

        Console.WriteLine("Recreating and migrating database ...");
        await dbContext.Database.MigrateAsync();
    }
}

async Task Import()
{
    Console.WriteLine("=====================");
    Console.WriteLine("Import");

    using (var scope = host.Services.CreateScope())
    {
        var importService = scope.ServiceProvider.GetRequiredService<IImportService>();
        var dbContext     = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await importService.ImportDbAsync();

        int countShipNames = await dbContext.ShipNames.CountAsync();
        int countShips     = await dbContext.Ships.CountAsync();
        int cuntCompanies  = await dbContext.Companies.CountAsync();
        Console.WriteLine($" {countShipNames} ship-names stored in DB");
        Console.WriteLine($" {countShips} ships stored in DB");
        Console.WriteLine($" {cuntCompanies} companies stored in DB");
    }

    Console.WriteLine($"Import done");
}