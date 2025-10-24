using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Contracts.VersionEvents;

namespace Airbnb.Infrastructure.Mapper;

public class UserAuditMapperV1 : IAuditMapper<ApplicationUser, UserUpdatedEventV1>
{
    public UserUpdatedEventV1 Map(ApplicationUser entity)
    {
        return new UserUpdatedEventV1
        {
            Id = entity.Id,
            Name = entity.Name,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            UpdatedAt = DateTime.UtcNow
        };
    }
}