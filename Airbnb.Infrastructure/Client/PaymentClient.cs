using System.Net.Http.Json;
using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces;
using Contracts.Payment;
using Microsoft.Extensions.Logging;

namespace Airbnb.Infrastructure.Client;

public class PaymentClient : IPaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentClient> _logger;

    public PaymentClient(HttpClient httpClient, ILogger<PaymentClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> TransactionWithdrawAsync(string accountNumber, decimal amount)
    {
        try
        {
            var dto = new WithdrawRequest
            {
                AccountNumber = accountNumber,
                Amount = amount
            };
            
            var response = await _httpClient.PostAsJsonAsync($"/api/v1/balance/transaction/withdraw", dto);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Withdraw transaction succeeded. Account: {accountNumber}, Amount: {amount}");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error while calling method transaction withdraw: " + ex.Message);
            return false;
        }
    }

    public async Task<BalanceResponse> GetUserBalanceAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/balance/{userId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<BalanceResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error while calling method get user balance: " + ex.Message);
            return null;
        }
    }
}