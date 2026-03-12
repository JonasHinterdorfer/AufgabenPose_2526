namespace Persistence.Mapping;

using Core.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public static class RaceMapping
{
    public static void Map(this EntityTypeBuilder<Race> entity)
    {
        entity.ToTable("Race");
        entity.HasKey(r => r.Id);

        entity.Property(r => r.RaceTime).IsRequired();

        entity.Property(r => r.RowVersion).IsRowVersion();

        entity.HasOne(r => r.Competition)
            .WithMany(c => c.Races)
            .HasForeignKey(r => r.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(r => r.Driver)
            .WithMany(d => d.Races)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
