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
    
    public async Task AddStepAsync(Guid bookingId, SagaStep step, SagaStepStatus status)
    {
        var journal = new BookingSagaJournal
        {
            BookingId = bookingId,
            Step = step,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.BookingSagaJournals.Add(journal);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStepStatusAsync(Guid bookingId, SagaStep step, SagaStepStatus status)
    {
        var journal = await _context.BookingSagaJournals
            .FirstOrDefaultAsync(x => x.BookingId == bookingId && x.Step == step);
        
        journal.Status = status;
        await _context.SaveChangesAsync();
    }
}