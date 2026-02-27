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
    public DbSet<CruiseShip>      ShipNames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.LogTo(message => Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO Add Mapping
    }
}