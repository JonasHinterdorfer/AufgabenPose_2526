namespace Logic;

using System.Diagnostics;

using Logic.Entities;
using Logic.Mapping;
using Logic.Tools;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ShippingCompany> Companies { get; set; }
    public DbSet<CruiseShip>      Ships     { get; set; }
    public DbSet<ShipName>        ShipNames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.LogTo(message => Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CruiseShip>().Map();
        modelBuilder.Entity<ShipName>().Map();
        modelBuilder.Entity<ShippingCompany>().Map();
    }
}