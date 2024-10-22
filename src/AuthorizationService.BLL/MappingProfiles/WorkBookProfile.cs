using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.DAL.Models;
using AutoMapper;

namespace AuthorizationService.BLL.MappingProfiles;

public class WorkBookProfile : Profile
{
    public WorkBookProfile()
    {
        CreateMap<WorkBook, WorkBookResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<WorkBookRequest, WorkBook>()
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => DateOnly.Parse(src.IssueDate)))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
    }
}
