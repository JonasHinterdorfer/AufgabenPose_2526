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
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString))
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
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    var statementsCsv = await new CsvImport<StatementCsv>().ReadAsync("ImportData/statements.txt");

    foreach (var csv in statementsCsv)
    {
        // Get or create category
        var categories = await uow.Categories.GetAsync(c => c.Description == csv.Category);
        var category   = categories.FirstOrDefault();

        if (category is null)
        {
            category = new Category { Description = csv.Category };
            await uow.Categories.AddAsync(category);
            await uow.SaveChangesAsync();
        }

        var statement = new Statement
        {
            Description   = csv.Statement,
            Politician    = csv.Politician,
            Created       = DateTime.UtcNow,
            StatementType = StatementType.Standard,
            CategoryId    = category.Id
        };

        await uow.Statements.AddAsync(statement);
    }

    await uow.SaveChangesAsync();
}

Console.WriteLine("done");