using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Plate)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.Plate)
            .IsUnique();

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Owner)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Category)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}
