using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Microsoft.Extensions.Logging;
using Shared.Redis.Redis;

namespace Airbnb.Application.BookingOrchestrator;

public class BookingSagaOrchestrator : IBookingSagaOrchestrator
{
    private readonly ILogger<BookingSagaOrchestrator> _logger;
    private readonly IBookingSagaJournalRepository _journal;
    private readonly IBookingService _bookingService;
    private readonly IPaymentClient _paymentClient;
    private readonly IRedisLock _redisLock;

    public BookingSagaOrchestrator(IBookingService bookingService, IPaymentClient paymentClient,
        IRedisLock redisLock, IBookingSagaJournalRepository journal, ILogger<BookingSagaOrchestrator> logger)
    {
        _bookingService = bookingService;
        _paymentClient = paymentClient;
        _redisLock = redisLock;
        _journal = journal;
        _logger = logger;
    }
    
    public async Task<Guid?> CreateSagaBooking(Guid apartmentId, Guid userId, DateRange range, decimal price, string accountNumber)
    {
        Booking booking = null;
        
        await _redisLock.LockDateRangeForBooking(apartmentId, range.Start, range.End);

        try
        {
            booking = await _bookingService.CreateBooking(userId, apartmentId, range);
            await _journal.AddSagaAsync(booking.Id, SagaStep.PaymentStarted, accountNumber, price);
            
            var transactionSuccess = await _paymentClient.TransactionWithdrawAsync(accountNumber, price);
        
            if (transactionSuccess)
            {
                await _bookingService.MarkAsPaid(booking.Id);
                await _journal.UpdateSagaAsync(booking.Id, SagaStep.BookingCompleted);
                return booking.Id;
            }
            else
            {
                await _bookingService.MarkAsCancelled(booking.Id);
                await _journal.UpdateSagaAsync(booking.Id, SagaStep.PaymentFailed, "Error occurred during withdraw transaction");
                return null;
            }
        }
        catch (Exception ex)
        {
            if (booking != null)
                await _journal.UpdateSagaAsync(booking.Id, SagaStep.UnexpectedError, ex.Message);
            
            throw;
        }
    }

    public async Task RecoveryStuckSaga(BookingSagaJournal saga)
    {
        try 
        { 
            var transactionSuccess = await _paymentClient.TransactionWithdrawAsync(saga.AccountNumber, saga.Amount);
            
            if (transactionSuccess) 
            { 
                await _bookingService.MarkAsPaid(saga.BookingId); 
                await _journal.UpdateSagaAsync(saga.BookingId, SagaStep.BookingCompleted);
            }
            else 
            { 
                await _bookingService.MarkAsCancelled(saga.Id); 
                await _journal.UpdateSagaAsync(saga.BookingId, SagaStep.PaymentFailed, "Error occurred during withdraw transaction");
            }
        }
        catch (Exception ex) 
        { 
            _logger.LogDebug("Recovery saga failed: " + ex.Message);
        }
    }
}