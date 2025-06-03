using Mapster;
using Airbnb.Application.DTOs;
using Airbnb.Domain.Entities;

namespace Airbnb.Application.Mappings;

public class RegisterMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterDto, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);
    }
}