using Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Persistence;

using System;
using System.IO;
using System.Linq;

using Base.Tools.CsvImport;

using Core.Contracts;

using Import.ImportData;

var builder = Host.CreateApplicationBuilder(args);


var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services
    //TODO: Configure DbContext with the connection string AddDbContext
    .AddScoped<IUnitOfWork, UnitOfWork>()
    ;

var host = builder.Build();

Console.WriteLine("Migrate Database");

using (var scope = host.Services.CreateScope())
{
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    await uow.DeleteDatabaseAsync();
    throw new NotImplementedException(); // migrate or add Db
//    await uow.CreateDatabaseAsync();
//    await uow.MigrateDatabaseAsync();
}

Console.WriteLine("Import Data");

using (var scope = host.Services.CreateScope())
{
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    var statementsCsv = await new CsvImport<StatementCsv>().ReadAsync("ImportData/statements.txt");

    // Import data from the file and save to the database
}

Console.WriteLine("done");