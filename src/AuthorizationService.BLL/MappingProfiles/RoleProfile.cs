using AuthorizationService.BLL.DTOs.Response;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService.BLL.MappingProfiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<IdentityRole<Guid>, RoleResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}
