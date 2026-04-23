using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

using Core.Entities;

public class StatementConfiguration : IEntityTypeConfiguration<Statement>
{
    public void Configure(EntityTypeBuilder<Statement> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Description)
            .IsRequired();

        builder.Property(s => s.Politician)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(s => s.Created)
            .IsRequired();

        builder.Property(s => s.Modified);

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Statements)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}