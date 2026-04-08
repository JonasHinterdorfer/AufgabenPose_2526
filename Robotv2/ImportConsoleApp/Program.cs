using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Base.Core;
using Base.Tools;
using Base.Tools.CsvImport;

using Core.Contracts;

using ImportConsoleApp.ImportData;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Persistence;

var configuration = ConfigurationHelper.GetConfiguration();

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

AppService.ServiceCollection = new ServiceCollection();
AppService.ServiceCollection
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString))
    .AddScoped<IUnitOfWork, UnitOfWork>()
    .AddTransient<ICompetitionRepository, CompetitionRepository>()
    ;

AppService.BuildServiceProvider();

await RecreateDatabaseAsync();
await ImportCsvs();

async Task ImportCsvs()
{
    Console.WriteLine("=====================");
    Console.WriteLine("Import Results");
    int countTotal = 0;

    using (var scope = AppService.ServiceProvider!.CreateScope())
    {
        var import = scope.ServiceProvider.GetRequiredService<IImportService>();

        foreach (var race in await new CsvImport<CsvContent>().ReadAsync("ImportData/Races.csv"))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var moves = await import.ImportRace(race.Driver, race.Competition, race.Moves);

            stopwatch.Stop();

            Console.WriteLine($"Imported {race.Driver}-{race.Competition} in {stopwatch.Elapsed}");
            countTotal++;
        }
    }

    Console.WriteLine($"Import done: {countTotal} races");
}

async Task RecreateDatabaseAsync()
{
    using (var scope = AppService.ServiceProvider!.CreateScope())
    {
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        Console.WriteLine("Deleting database ...");
        await uow.DeleteDatabaseAsync();

        Console.WriteLine("Recreating and migrating database ...");
        await uow.MigrateDatabaseAsync();
    }
}