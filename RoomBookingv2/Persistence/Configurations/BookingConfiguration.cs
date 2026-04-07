using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

using Core.Entities;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.RowVersion).IsRowVersion();

        builder.Property(l => l.RoomId).IsRequired();
        builder.Property(l => l.From).IsRequired();
        
        builder.HasOne(l => l.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(l => l.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}