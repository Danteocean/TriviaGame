using AutoMapper;
using CoreLibrary.DTOs.Category.Response;
using CoreLibrary.DTOs.Question.Requests;
using Domain.Entities;

namespace CoreLibrary.Mappings;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        CreateMap<OptionDtoRequest, Option>();
        CreateMap<QuestionDtoRequest, Question>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
        CreateMap<Category, CategoryDtoResponse>();
    }
}
