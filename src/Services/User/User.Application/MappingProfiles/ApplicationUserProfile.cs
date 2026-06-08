using AutoMapper;
using User.Application.Features.Users.Dtos;
using User.Domain.AggregatesModel.ApplicationUserAggregate;

namespace User.Application.MappingProfiles;

public class ApplicationUserProfile : Profile
{
    public ApplicationUserProfile()
    {
        CreateMap<ApplicationUser, ApplicationUserCacheDto>()
            .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Address));
        CreateMap<ApplicationUserCacheDto, ApplicationUser>()
            .ConstructUsing(src => new ApplicationUser(
                src.ExternalId,
                src.Email,
                src.UserName,
                src.FirstName,
                src.LastName,
                src.PhoneNumber
            ))
            .ForMember(d => d.Address, opt => opt.MapFrom(s => s.Address));
    }
}