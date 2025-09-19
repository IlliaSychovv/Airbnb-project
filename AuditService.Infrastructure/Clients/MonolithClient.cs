using System.Net.Http.Json;
using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuditService.Infrastructure.Clients;

public class MonolithClient : IMonolithClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MonolithClient> _logger;

    public MonolithClient(HttpClient httpClient, ILogger<MonolithClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserLoginsDto?> GetUserLoginAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/users/logins?userId={userId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserLoginsDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing monolith for getting user login");
            return null;
        }
    }
}