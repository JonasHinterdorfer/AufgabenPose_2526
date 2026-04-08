namespace Persistence;

using Persistence.Mapping;

using Core.Entities;

using Microsoft.EntityFrameworkCore;

using Base.Tools;

using System.Diagnostics;

public class ApplicationDbContext : DbContext
{
    internal DbSet<Competition> Competitions { get; set; }
    internal DbSet<Race> Races { get; set; }
    internal DbSet<Driver> Drivers { get; set; }
    internal DbSet<Move> Moves { get; set; }
    public ApplicationDbContext() : base()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //We need this for migration
            var connectionString = ConfigurationHelper.GetConfiguration().Get("DefaultConnection", "ConnectionStrings");
            optionsBuilder.UseSqlite(connectionString);
        }
        
        optionsBuilder.LogTo(message => Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Competition>().Map();
        modelBuilder.Entity<Race>().Map();
        modelBuilder.Entity<Driver>().Map();
        modelBuilder.Entity<Move>().Map();
    }
}