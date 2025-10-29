using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Domain.Interfaces;
using Contracts.VersionEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OneOf;

namespace Airbnb.Infrastructure.Interceptor;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuditMapper<ApplicationUser, UserUpdatedEventV1> _userMapper;
    private readonly IEventSender _eventSender;

    public AuditInterceptor(IAuditMapper<ApplicationUser, UserUpdatedEventV1> userMapper, IEventSender eventSender)
    {
        _userMapper = userMapper;
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

            OneOf<UserUpdatedEventV1, NotFound> auditEvent = entry.Entity switch
            {
                ApplicationUser user => _userMapper.Map(user),
                _ => new NotFound()
            };

            await auditEvent.Match(
                async mappedEvent =>
                {
                    var key = entry.Property("Id").CurrentValue?.ToString() ?? Guid.NewGuid().ToString();
                    await _eventSender.SendEvent(key, mappedEvent);
                },
                _ => Task.CompletedTask
            );


            // var mapper = _mappers.FirstOrDefault(m =>
            // {
            //     var type = m.GetType()
            //         .GetInterfaces()
            //         .FirstOrDefault(i =>
            //             i.IsGenericType &&
            //             i.GetGenericTypeDefinition() == typeof(IAuditMapper<,>));
            //
            //     if (type == null) 
            //         return false;
            //
            //     var entityType = type.GetGenericArguments()[0];
            //     return entityType.IsAssignableFrom(entry.Entity.GetType());
            // });
            //
            // var mapMethod = mapper.GetType().GetMethod("Map");
            // var auditEvent = mapMethod.Invoke(mapper, new[] { entry.Entity });
            //
            // var key = entry.Property("Id").CurrentValue?.ToString() ?? Guid.NewGuid().ToString();
            // await _eventSender.SendEvent(key, (dynamic)auditEvent);
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

public class NotFound { }