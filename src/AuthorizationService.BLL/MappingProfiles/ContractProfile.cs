using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.DAL.Models;
using AutoMapper;

namespace AuthorizationService.BLL.MappingProfiles;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<Contract, ContractResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(
                dest => dest.IsValid,
                opt => opt.MapFrom(src => src.EndDate >= DateOnly.FromDateTime(DateTime.Today)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<ContractRequest, Contract>()
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}
