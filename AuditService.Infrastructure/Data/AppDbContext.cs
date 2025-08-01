using AuditService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.Property(m => m.ProfileJson)
                .HasColumnType("jsonb");
        });
    }
    
    public DbSet<Audit> Audits { get; set; }
}