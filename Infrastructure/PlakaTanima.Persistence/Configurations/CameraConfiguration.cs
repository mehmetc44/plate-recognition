using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Persistence.Configurations;

public class CameraConfiguration : IEntityTypeConfiguration<Camera>
{
    public void Configure(EntityTypeBuilder<Camera> builder)
    {
        builder.Property(x => x.Name)
    .IsRequired()
    .HasMaxLength(100);

        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.IpAddress)
            .IsUnique();

        builder.Property(x => x.Port)
            .IsRequired()
            .HasDefaultValue(80);

        builder.Property(x => x.StreamChannel)
            .IsRequired()
            .HasDefaultValue(101);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.HasOne(x => x.Location)
            .WithMany(x => x.Cameras)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}