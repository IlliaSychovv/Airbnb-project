using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Shared.Redis.Redis;

namespace Airbnb.Application.BookingOrchestrator;

public class BookingSagaOrchestrator : IBookingSagaOrchestrator
{
    private readonly IBookingSagaJournalRepository _journal;
    private readonly IBookingService _bookingService;
    private readonly IPaymentClient _paymentClient;
    private readonly IRedisLock _redisLock;

    public BookingSagaOrchestrator(IBookingService bookingService, IPaymentClient paymentClient,
        IRedisLock redisLock, IBookingSagaJournalRepository journal)
    {
        _bookingService = bookingService;
        _paymentClient = paymentClient;
        _redisLock = redisLock;
        _journal = journal;
    }

    public async Task<Guid?> CreateSagaBooking(Guid apartmentId, Guid userId, DateRange range, decimal price, string accountNumber)
    {
        var lockKey = $"booking:{apartmentId}:{range.Start:yyyyMMdd}-{range.End:yyyyMMdd}";
        await using var handle = await _redisLock.LockAsync(lockKey, TimeSpan.FromSeconds(20));
        
        var booking = await _bookingService.CreateBooking(userId, apartmentId, range);
        await _journal.AddStepAsync(booking.Id, SagaStep.BookingCreated, SagaStepStatus.Completed);
        await _journal.AddStepAsync(booking.Id, SagaStep.PaymentStarted, SagaStepStatus.InProgress);
        
        var transactionSuccess = await _paymentClient.TransactionWithdrawAsync(accountNumber, price);
        
        if (transactionSuccess)
        {
            await _bookingService.MarkAsPaid(booking.Id);
            await _journal.UpdateStepStatusAsync(booking.Id, SagaStep.PaymentStarted, SagaStepStatus.Completed);
            return booking.Id;
        }
        else
        {
            await _bookingService.MarkAsCancelled(booking.Id);
            await _journal.UpdateStepStatusAsync(booking.Id, SagaStep.PaymentStarted, SagaStepStatus.Failed);
            return null;
        }
    }
}