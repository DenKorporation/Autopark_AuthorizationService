using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.DAL.Models;
using AutoMapper;

namespace AuthorizationService.BLL.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles.First().Name))
            .ForMember(
                dest => dest.PassportId,
                opt => opt.MapFrom(src => src.Passport != null ? src.Passport.Id : (Guid?)null))
            .ForMember(
                dest => dest.WorkBookId,
                opt => opt.MapFrom(src => src.WorkBook != null ? src.WorkBook.Id : (Guid?)null))
            .ForMember(dest => dest.ContractIds, opt => opt.MapFrom(src => src.Contracts.Select(c => c.Id)));

        CreateMap<UserRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
