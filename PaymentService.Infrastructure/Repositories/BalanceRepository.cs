using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entity;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories;

public class BalanceRepository : IBalanceRepository
{
    private readonly AppDbContext _context;

    public BalanceRepository(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Balance?> GetBalanceByUserId(Guid userId)
    {
        return await _context.Balances.FindAsync(userId);
    }

    public async Task CreateBalance(Balance balance)
    {
        if (await _context.Balances.AnyAsync(x => x.UserId == balance.UserId))
            return;
        
        await _context.Balances.AddAsync(balance);
        await _context.SaveChangesAsync();
    }
}