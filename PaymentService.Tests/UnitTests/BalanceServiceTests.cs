using Moq;
using PaymentService.Application.DTO;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Services;
using PaymentService.Domain.Entity;
using RedLockNet;
using Shared.Redis.Redis;
using Shouldly;

namespace PaymentService.Tests.UnitTests;

public class BalanceServiceTests
{
    private readonly Mock<IBalanceRepository> _balanceRepositoryMock;
    private readonly Mock<IRedisLock> _redisLockMock;
    private readonly IBalanceService _balanceService;

    public BalanceServiceTests()
    {
        _balanceRepositoryMock = new Mock<IBalanceRepository>();
        _redisLockMock = new Mock<IRedisLock>();
        _balanceService = new BalanceService(_balanceRepositoryMock.Object, _redisLockMock.Object);
    }

    [Fact]
    public async Task GetBalanceByUserId_ShouldReturnBalance_WhenWeCallMethod()
    {
        var userId = Guid.NewGuid();
        var balance = new Balance
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = 1000m,
            CreatedAt = DateTime.Now,
            RechargedAt = null
        };
        
        _balanceRepositoryMock.Setup
            (x => x.GetBalanceByUserId(userId))
            .ReturnsAsync(balance);
        
        var result = await _balanceService.GetBalanceByUserId(userId);

        result.ShouldNotBeNull();
        result.Amount.ShouldBe(balance.Amount);
        _balanceRepositoryMock.Verify(x => x.GetBalanceByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task CreateBalance_ShouldReturnSuccess_WhenWeCallMethod()
    {
        var userId = Guid.NewGuid();
    
        _balanceRepositoryMock
            .Setup(x => x.CreateBalance(It.IsAny<Balance>()))
            .Returns(Task.CompletedTask);
    
        await _balanceService.CreateBalance(userId);
    
        _balanceRepositoryMock.Verify(
            x => x.CreateBalance(It.Is<Balance>(b => 
                b.UserId == userId && b.Amount == 0m && b.AccountNumber != null)),
            Times.Once);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnBalance_WhenWeCallMethod()
    {
        var depositDto = new DepositDto
        {
            AccountNumber = "QQ1111",
            Amount = 1000m
        };

        var entity = new Balance
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Amount = 1000m + depositDto.Amount,
            AccountNumber = depositDto.AccountNumber,
            CreatedAt = DateTime.Now,
            RechargedAt = null
        };

        _balanceRepositoryMock
            .Setup(x => x.DepositBalanceAsync(It.IsAny<Balance>()))
            .ReturnsAsync(entity);
        
        var result = await _balanceService.DepositAsync(depositDto);
        
        result.ShouldNotBeNull();
        result.Amount.ShouldBe(entity.Amount);
        _balanceRepositoryMock.Verify(x => x.DepositBalanceAsync(It.IsAny<Balance>()), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnBalance_WhenWeCallMethod()
    {
        var withdrawDto = new WithdrawDto
        { 
            AccountNumber = "ACC999",
            Amount = 150m
        };
        
        var redLockMock = new Mock<IRedLock>();
        redLockMock.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);

        _redisLockMock
            .Setup(x => x.LockAsync(It.IsAny<string>(), It.IsAny<TimeSpan?>(), null, null))
            .ReturnsAsync(redLockMock.Object);

        var expectedBalance = new Balance
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            AccountNumber = withdrawDto.AccountNumber,
            Amount = 850m, 
            CreatedAt = DateTime.UtcNow,
            RechargedAt = DateTime.UtcNow
        };

        _balanceRepositoryMock
            .Setup(x => x.WithdrawBalanceAsync(It.IsAny<Balance>()))
            .ReturnsAsync(expectedBalance);
        
        var result = await _balanceService.WithdrawAsync(withdrawDto);

        result.ShouldNotBeNull();
        result.Amount.ShouldBe(expectedBalance.Amount);
        result.AccountNumber.ShouldBe(withdrawDto.AccountNumber);

        _redisLockMock.Verify(
            x => x.LockAsync($"payment:{withdrawDto.AccountNumber}", It.IsAny<TimeSpan?>(), null, null),
            Times.Once);

        _balanceRepositoryMock.Verify(
            x => x.WithdrawBalanceAsync(It.Is<Balance>(b =>
                b.AccountNumber == withdrawDto.AccountNumber)),
            Times.Once);

        redLockMock.Verify(x => x.DisposeAsync(), Times.Once);
    }
}