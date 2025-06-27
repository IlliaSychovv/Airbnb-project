using Airbnb.Application.Interfaces;
using Airbnb.Application.DTOs.Migrations;
using Airbnb.Data;
using Airbnb.Domain.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Airbnb.Infrastructure.Service;

public class DataMigrationService : IDataMigrationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DataMigrationService> _logger; 
    private readonly IJsonDataReader _jsonReader;
    private const string DefaultPassword = "default12345";

    public DataMigrationService(AppDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<DataMigrationService> logger,
        IJsonDataReader jsonReader)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _jsonReader = jsonReader;
    }

    public async Task RunAsync(string filePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting data migration");
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File {filePath} not found");

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            await MigrateUserAsync(filePath, cancellationToken);
            
            await MigrateApartmentsAsync(filePath, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Data migration successfully finished");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Data migration was canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during data migration");
            throw;
        }
    }
    
    private async Task MigrateUserAsync(string filePath, CancellationToken cancellationToken)
    {
        const int batchSize = 500;
        var batch = new List<ExternalUser>(batchSize);

        await foreach (var userExternal in _jsonReader.ReadUsers(filePath, cancellationToken))
        {
            batch.Add(userExternal);

            if (batch.Count >= batchSize)
            {
                await ProcessUserBatchAsync(batch, cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            await ProcessUserBatchAsync(batch, cancellationToken);
    }
    
    private async Task MigrateApartmentsAsync(string filePath, CancellationToken cancellationToken)
    {
        const int batchSize = 500;
        var batch = new List<ExternalApartment>(batchSize);

        await foreach (var apartmentExternal in _jsonReader.ReadApartment(filePath, cancellationToken))
        {
            batch.Add(apartmentExternal);

            if (batch.Count >= batchSize)
            {
                await ProcessApartmentBatchAsync(batch, cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            await ProcessApartmentBatchAsync(batch, cancellationToken);
    }

    private async Task ProcessApartmentBatchAsync(List<ExternalApartment> batch, CancellationToken cancellationToken)
    {
        var externalIds = batch.Select(a => a.ExternalId).ToList();

        var existingIds = await _context.Apartments
            .Where(a => externalIds.Contains(a.ExternalId))
            .Select(a => a.ExternalId)
            .ToListAsync(cancellationToken);

        var newApartments = batch
            .Where(a => !existingIds.Contains(a.ExternalId))
            .Select(a => a.Adapt<Apartment>())
            .ToList();

        if (newApartments.Count == 0)
        {
            _logger.LogInformation("Batch skipped, all apartments already exist");
            return;
        }

        _context.Apartments.AddRange(newApartments);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    private async Task ProcessUserBatchAsync(List<ExternalUser> batch, CancellationToken cancellationToken)
    {
        if (batch.Count == 0)
            return;

        var externalIds = batch.Select(u => u.ExternalId).ToList();
        
        var existingUserIds = await _userManager.Users
            .Where(u => externalIds.Contains(u.ExternalId))
            .Select(u => u.ExternalId)
            .ToListAsync(cancellationToken);

        foreach (var userExternal in batch)
        {
            if (existingUserIds.Contains(userExternal.ExternalId))
                continue;

            var newUser = userExternal.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(newUser, DefaultPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Error creating user {newUser.Email}: {errors}");
            }
        }
    }
}