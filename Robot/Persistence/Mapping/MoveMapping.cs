namespace Persistence.Mapping;

using Core.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public static class MoveMapping
{
    public static void Map(this EntityTypeBuilder<Move> entity)
    {
        entity.ToTable("Move");
        entity.HasKey(m => m.Id);

        entity.Property(m => m.No).IsRequired();
        entity.Property(m => m.Direction).IsRequired();
        entity.Property(m => m.Speed).IsRequired();
        entity.Property(m => m.Duration).IsRequired();

        entity.Property(m => m.RowVersion).IsRowVersion();

        entity.HasOne(m => m.Race)
            .WithMany(r => r.Moves)
            .HasForeignKey(m => m.RaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
