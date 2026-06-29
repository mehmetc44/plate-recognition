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
    public DbSet<AnprEvent> AnprEvents { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}