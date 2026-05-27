using AutoMapper;
using CoreLibrary.DTOs.GameSession.Response;

namespace CoreLibrary.Mappings;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<QuestionDtoResponse, QuestionDtoResponse>().ReverseMap();
    }
}
