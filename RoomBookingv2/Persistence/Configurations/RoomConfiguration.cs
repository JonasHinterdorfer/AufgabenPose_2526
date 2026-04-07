using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

using Core.Entities;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.RowVersion).IsRowVersion();

            builder.Property(l => l.RoomNumber).IsRequired().HasMaxLength(40);
            builder.Property(l => l.RoomType).HasConversion<string>().IsRequired();
    }
}