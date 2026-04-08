namespace Persistence.Mapping;

using Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class CompetitionMapping
{
    public static void Map(this EntityTypeBuilder<Competition> entity)
   {
        entity.ToTable("Competition");
        entity.HasKey(l => l.Id);

        entity.HasIndex(c => c.Name).IsUnique();
        entity.Property(c => c.Name).HasMaxLength(128);
    }

    public static void Map(this EntityTypeBuilder<Race> entity)
    {
        entity.ToTable("Race");
        entity.HasKey(l => l.Id);
        
        entity.HasOne(l => l.Driver)
            .WithMany(l => l.Races)
            .HasForeignKey(l => l.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(l => l.Competition)
            .WithMany(l => l.Races)
            .HasForeignKey(l => l.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static void Map(this EntityTypeBuilder<Move> e)
    {
        e.ToTable("Move");
        e.HasKey(l => l.Id);
        
         e.HasOne(l => l.Race)
            .WithMany(l => l.Moves)
            .HasForeignKey(l => l.RaceId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static void Map(this EntityTypeBuilder<Driver> e)
    {
        e.ToTable("Driver");
        e.HasKey(l => l.Id);
        e.HasIndex(c => c.Name).IsUnique();
        e.Property(c => c.Name).HasMaxLength(128);
    }
}