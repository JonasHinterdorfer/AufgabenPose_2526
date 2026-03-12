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
        entity.Property(c => c.Name).HasMaxLength(256).IsRequired();

        entity.Property(c => c.RowVersion).IsRowVersion();
    }
}