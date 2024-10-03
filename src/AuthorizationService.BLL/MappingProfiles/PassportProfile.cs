using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.DAL.Models;
using AutoMapper;

namespace AuthorizationService.BLL.MappingProfiles;

public class PassportProfile : Profile
{
    public PassportProfile()
    {
        CreateMap<Passport, PassportResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname))
            .ForMember(dest => dest.Patronymic, opt => opt.MapFrom(src => src.Patronymic))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<PassportRequest, Passport>()
            .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname))
            .ForMember(dest => dest.Patronymic, opt => opt.MapFrom(src => src.Patronymic))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}
