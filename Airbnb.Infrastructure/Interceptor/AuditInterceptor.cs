using Airbnb.Application.CreatedEvent;
using Airbnb.Application.Interfaces;
using Airbnb.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Airbnb.Infrastructure.Interceptor;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IEventSender? _eventSender;

    public AuditInterceptor(IEventSender eventSender = null)
    {
        _eventSender = eventSender;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = context.ChangeTracker
            .Entries<IAuditableEntity>()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;

            var auditEvent = new UserUpdatedEvent
            {
                Id = (Guid)entry.Property("Id").CurrentValue,
                Name = entry.Property("Name").CurrentValue?.ToString(),
                Email = entry.Property("Email").CurrentValue?.ToString(),
                PhoneNumber = entry.Property("PhoneNumber").CurrentValue?.ToString(),
                UpdatedAt = DateTime.UtcNow 
            };
            
            if (_eventSender != null)
            {
                var key = entry.Property("Id").CurrentValue?.ToString() ?? Guid.NewGuid().ToString();
                await _eventSender.SendEvent(key, auditEvent);
            }
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}