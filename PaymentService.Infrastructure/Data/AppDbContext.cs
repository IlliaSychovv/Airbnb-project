using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entity;

namespace PaymentService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Balance> Balances { get; set; }
}