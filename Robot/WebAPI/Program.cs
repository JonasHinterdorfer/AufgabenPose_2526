using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization to ignore reference cycles caused by navigation properties
builder.Services.ConfigureHttpJsonOptions(opts =>
{
    // Use ReferenceHandler.Preserve to support object graphs with cycles.
    // This emits $id / $ref metadata in the JSON to preserve references and avoid infinite recursion.
    // Note: Preserve changes the JSON shape; for public APIs it's better to map entities to DTOs without back-references.
    opts.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    // Increase max depth if your object graphs are deep
    opts.SerializerOptions.MaxDepth = 128;
});

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

// Choose database provider based on platform and available configuration
if (OperatingSystem.IsWindows() && !string.IsNullOrEmpty(connectionString))
{
    // On Windows prefer configured SqlServer (e.g., LocalDB)
    builder.Services.AddDbContext<Persistence.ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    // On non-Windows (or when DefaultConnection is missing) use a file-based Sqlite DB for development
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

// Apply migrations / ensure database is created on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<Persistence.ApplicationDbContext>();
        // If SQL Server is used, prefer migrations; for Sqlite use EnsureCreated to avoid SQL Server-specific migration issues
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
        // Rethrow to prevent running with a broken DB schema
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

    // Project to a DTO-like shape that intentionally omits the Race.Competition back-reference
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