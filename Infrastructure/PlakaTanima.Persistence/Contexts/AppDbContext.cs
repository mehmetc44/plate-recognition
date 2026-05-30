using System;
using System.Collections.Generic;
using PlakaTanima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PlakaTanima.Persistence.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Camera> Cameras { get; set; }
    public DbSet<LprEvent> LprEvents { get; set; }
    public DbSet<LprImage> LprImages { get; set; }
    
    // ISet yerine DbSet yapıldı
    public DbSet<LprRawEvent> LprRawEvents { get; set; }

    // ModuleBuilder yerine ModelBuilder yapıldı
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. LprEvent Konfigürasyonu
        modelBuilder.Entity<LprEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Plate).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CameraName).HasMaxLength(100).IsRequired();
        });

        // 2. LprEvent <-> LprImage (One-to-Many İlişkisi)
        modelBuilder.Entity<LprImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(20).IsRequired(); // plate, vehicle, full
            
            entity.HasOne(i => i.Event)
                  .WithMany(e => e.Images)
                  .HasForeignKey(i => i.EventId)
                  .OnDelete(DeleteBehavior.Cascade); // Ana event silinirse, resim yolları da silinsin
        });

        // 3. LprEvent <-> LprRawEvent (One-to-One İlişkisi)
        modelBuilder.Entity<LprRawEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // PostgreSQL'e özel kritik nokta: JSON verisini JSONB olarak tutmak!
            entity.Property(e => e.RawJson).HasColumnType("jsonb");
            entity.Property(e => e.RawXml).HasColumnType("text");

            entity.HasOne(r => r.Event)
                  .WithOne(e => e.RawEvent)
                  .HasForeignKey<LprRawEvent>(r => r.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}