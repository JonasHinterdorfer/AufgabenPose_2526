using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

using Core.Entities;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rate)
            .IsRequired();

        builder.Property(r => r.Remark)
            .HasMaxLength(256);

        builder.Property(r => r.UserName)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne(r => r.Statement)
            .WithMany(s => s.Ratings)
            .HasForeignKey(r => r.StatementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
