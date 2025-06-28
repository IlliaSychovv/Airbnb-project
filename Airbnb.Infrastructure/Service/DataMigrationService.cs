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
            await _context.Database.RollbackTransactionAsync(cancellationToken);
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
        if (batch.Count == 0)
            return;
        
        var externalIds = batch.Select(x => x.ExternalId).ToList();
        
        var existingApartments = await _context.Apartments
            .Where(x => externalIds.Contains(x.ExternalId))
            .ToListAsync(cancellationToken);
        
        var existingEntities = existingApartments.ToDictionary(x => x.ExternalId);

        foreach (var externalApartment in batch)
        {
            if (existingEntities.TryGetValue(externalApartment.ExternalId, out var entity))
                externalApartment.Adapt(entity);
            else
            {
                var newApartment = externalApartment.Adapt<Apartment>();
                _context.Apartments.Add(newApartment);
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Data migration successfully finished");
    }
    
    private async Task ProcessUserBatchAsync(List<ExternalUser> batch, CancellationToken cancellationToken)
    {
        if (batch.Count == 0)
            return;

        var externalIds = batch.Select(u => u.ExternalId).ToList();
        
        var existingUser = await _userManager.Users
            .Where(u => externalIds.Contains(u.ExternalId))
            .ToListAsync(cancellationToken);

        var existingEntities = existingUser.ToDictionary(u => u.ExternalId);
        
        foreach (var userExternal in batch)
        {
            if (existingEntities.TryGetValue(userExternal.ExternalId, out var entity))
            {
                userExternal.Adapt(entity);
                
                var updateData = await _userManager.UpdateAsync(entity);
                if (!updateData.Succeeded)
                {
                    var errors = updateData.Errors.Select(e => e.Description);
                    _logger.LogError($"Error: {errors}");
                }
            }
            else
            {
                var newUser = userExternal.Adapt<ApplicationUser>();
                
                var createUser = await _userManager.CreateAsync(newUser, DefaultPassword);
                if (!createUser.Succeeded)
                {
                    var errors = createUser.Errors.Select(e => e.Description);
                    _logger.LogError($"Error: {errors}");
                }
            }
        }
    }
}