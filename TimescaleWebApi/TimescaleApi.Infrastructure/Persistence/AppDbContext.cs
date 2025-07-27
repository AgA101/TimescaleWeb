using Microsoft.EntityFrameworkCore;
using TimescaleApi.Domain.Entities;

namespace TimescaleApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Value> Values { get; set; }
    public DbSet<Result> Results { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Value>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.HasIndex(v => v.FileName);
            entity.Property(v => v.Date).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.FileName);
        });
    }
}