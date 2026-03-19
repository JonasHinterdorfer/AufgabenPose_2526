using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

using Core.Entities;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Email)
               .HasMaxLength(200);

        builder.HasMany(c => c.Bookings)
               .WithOne(b => b.Customer)
               .HasForeignKey(b => b.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
