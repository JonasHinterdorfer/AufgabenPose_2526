using System;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<Persistence.ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<Core.Contracts.IUnitOfWork, Persistence.UnitOfWork>();
builder.Services.AddTransient<Core.Contracts.ICompetitionRepository, Persistence.CompetitionRepository>();
builder.Services.AddTransient<Core.Contracts.IDriverRepository, Persistence.DriverRepository>();
builder.Services.AddTransient<Core.Contracts.IRaceRepository, Persistence.RaceRepository>();
builder.Services.AddTransient<Core.Contracts.IMoveRepository, Persistence.MoveRepository>();
builder.Services.AddTransient<Core.Contracts.IImportService, Persistence.ImportService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
    return Results.Ok(list);
}).WithName("competitions").WithOpenApi();

app.MapGet("/races/count/{driverName}", async (string driverName, Core.Contracts.IUnitOfWork uow) =>
{
    var races = await uow.Race.GetByDriverAsync((await uow.Driver.GetByNameAsync(driverName))?.Id ?? 0);
    return Results.Ok(new { Count = races.Count });
}).WithName("races_count");

app.Run();