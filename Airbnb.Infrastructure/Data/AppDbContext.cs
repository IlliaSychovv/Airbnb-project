using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Airbnb.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Apartment>()
            .HasIndex(i => i.ExternalId);
        
        builder.Entity<Apartment>(entity =>
        {
            entity.Property(p => p.Metadata)
                .HasColumnType("jsonb");
        });

        builder.Entity<BookingSagaJournal>()
            .Property(x => x.Step)
            .HasConversion<string>();
        
        builder.Entity<BookingSagaJournal>()
            .Property(x => x.Status)
            .HasConversion<string>();
        
        builder.Entity<Booking>()
            .Property(x => x.Status)
            .HasConversion<string>();
    }
    
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<BookingSagaJournal> BookingSagaJournals { get; set; }
}