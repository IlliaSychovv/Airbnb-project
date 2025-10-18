using Mapster;
using PaymentService.Application.DTO;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entity;
using Shared.Redis.Redis;

namespace PaymentService.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IBalanceRepository _repository;
    private readonly IRedisLock _redisLock;
    
    public BalanceService(IBalanceRepository repository, IRedisLock redisLock)
    {
        _repository = repository;
        _redisLock = redisLock;
    }

    public async Task<BalanceDto> GetBalanceByUserId(Guid userId)
    {
        var balance = await _repository.GetBalanceByUserId(userId);
        return balance.Adapt<BalanceDto>();
    }

    public async Task CreateBalance(Guid userId)
    {
        var balance = new Balance
        {
            Id = userId,
            UserId = userId,
            Amount = 0m,
            AccountNumber = GenerateAccountNumber(),
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.CreateBalance(balance);
    }

    public async Task<DepositDto> DepositAsync(DepositDto dto)
    {
        var entity = dto.Adapt<Balance>();
        var deposit = await _repository.DepositBalanceAsync(entity);
        
        return deposit.Adapt<DepositDto>();
    }

    public async Task<WithdrawDto> WithdrawAsync(WithdrawDto dto)
    {
        var lockKey = $"payment:{dto.AccountNumber}";
        await using var handle = await _redisLock.LockAsync(lockKey, TimeSpan.FromSeconds(20));
        
        var entity = dto.Adapt<Balance>();
        var withdraw = await _repository.WithdrawBalanceAsync(entity);
        
        return withdraw.Adapt<WithdrawDto>();
    }

    private static string GenerateAccountNumber()
    {
        var random = new Random();
        
        string letters = new string(Enumerable.Range(0, 2)
            .Select(_ => (char)random.Next('A', 'Z' + 1))
            .ToArray());

        string numbers = random.Next(0, 100000).ToString("D5");

        return letters + numbers;
    }
}