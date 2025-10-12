using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.Infrastructure.Repositories;

public class BookingSagaJournalRepository : IBookingSagaJournalRepository
{
    private readonly AppDbContext _context;

    public BookingSagaJournalRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddSagaAsync(Guid bookingId, SagaStep step)
    {
        var journal = new BookingSagaJournal
        {
            BookingId = bookingId,
            Step = step,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.BookingSagaJournals.Add(journal);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSagaAsync(Guid bookingId, SagaStep step, string? error = null)
    {
        var journal = await _context.BookingSagaJournals
            .FirstOrDefaultAsync(x => x.BookingId == bookingId);
        
        journal.Step = step;
        journal.Error = error;
        await _context.SaveChangesAsync();
    }
}