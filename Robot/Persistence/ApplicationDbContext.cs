namespace Persistence;

using Persistence.Mapping;

using Core.Entities;

using Microsoft.EntityFrameworkCore;

using Base.Tools;

using System.Diagnostics;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base()
    {
        //We need this constructor for migration
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
            optionsBuilder.UseSqlServer(connectionString);
        }

        optionsBuilder.LogTo(message => Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Competition>(e => e.Map());
        modelBuilder.Entity<Driver>(e => e.Map());
        modelBuilder.Entity<Race>(e => e.Map());
        modelBuilder.Entity<Move>(e => e.Map());
    }
}