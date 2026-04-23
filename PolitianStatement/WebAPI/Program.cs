using Base.Tools;

using Core.Contracts;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Persistence;

using WebAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options => { options.AddDefaultPolicy(policy => { policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); }); });


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var configuration    = ConfigurationHelper.GetConfiguration();
var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ping", () => "pong")
    .WithName("Ping")
    .WithTags("Health");

app.MapCategoryEndpoints();
//app.MapRatingEndpoints();
//app.MapStatementEndpoints();

app.Run();