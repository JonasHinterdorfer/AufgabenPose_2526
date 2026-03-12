using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    opts.SerializerOptions.MaxDepth = 128;
});

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

if (OperatingSystem.IsWindows() && !string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<Persistence.ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    var contentRoot = builder.Environment.ContentRootPath ?? AppContext.BaseDirectory;
    var sqlitePath = System.IO.Path.Combine(contentRoot, "Robot.db");
    var sqliteConnString = $"Data Source={sqlitePath}";
    builder.Services.AddDbContext<Persistence.ApplicationDbContext>(options => options.UseSqlite(sqliteConnString));
}

builder.Services.AddScoped<Core.Contracts.IUnitOfWork, Persistence.UnitOfWork>();
builder.Services.AddTransient<Core.Contracts.ICompetitionRepository, Persistence.CompetitionRepository>();
builder.Services.AddTransient<Core.Contracts.IDriverRepository, Persistence.DriverRepository>();
builder.Services.AddTransient<Core.Contracts.IRaceRepository, Persistence.RaceRepository>();
builder.Services.AddTransient<Core.Contracts.IMoveRepository, Persistence.MoveRepository>();
builder.Services.AddTransient<Core.Contracts.IImportService, Persistence.ImportService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<Persistence.ApplicationDbContext>();
        if (db.Database.IsSqlServer())
        {
            db.Database.Migrate();
        }
        else if (db.Database.IsSqlite())
        {
            db.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating or migrating the database.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/import", async (string driverName, string competitionName, string moves, Core.Contracts.IImportService importService) =>
{
    var count = await importService.ImportRace(driverName, competitionName, moves);
    return Results.Ok(new { MovesImported = count });
}).WithName("import");

app.MapGet("/competitions", async (Core.Contracts.IUnitOfWork uow) =>
{
    var list = await uow.Competition.GetAllWithRace();

    var dto = list.Select(c => new
    {
        c.Id,
        c.Name,
        Races = c.Races?.Select(r => new
        {
            r.Id,
            r.RaceTime,
            Driver = r.Driver == null ? null : new { r.Driver.Id, r.Driver.Name },
            Moves = r.Moves?.Select(m => new { m.Id, m.No, m.Direction, m.Speed, m.Duration }).ToList()
        }).ToList()
    }).ToList();

    return Results.Ok(dto);
}).WithName("competitions").WithOpenApi();

app.MapGet("/races/count/{driverName}", async (string driverName, Core.Contracts.IUnitOfWork uow) =>
{
    var races = await uow.Race.GetByDriverAsync((await uow.Driver.GetByNameAsync(driverName))?.Id ?? 0);
    return Results.Ok(new { Count = races.Count });
}).WithName("races_count");

app.Run();