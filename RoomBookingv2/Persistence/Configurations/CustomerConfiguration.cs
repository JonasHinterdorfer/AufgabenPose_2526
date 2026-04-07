using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

using Core.Entities;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.RowVersion).IsRowVersion();

        builder.Property(l => l.FirstName).IsRequired().HasMaxLength(40);
        builder.Property(l => l.LastName).IsRequired().HasMaxLength(40);
        builder.Property(l => l.IBAN).IsRequired().HasMaxLength(10);
    }
}