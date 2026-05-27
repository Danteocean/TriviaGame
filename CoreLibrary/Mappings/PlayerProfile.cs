using AutoMapper;
using CoreLibrary.DTOs.Player.Requests;
using CoreLibrary.DTOs.Player.Response;
using Domain.Entities;

namespace CoreLibrary.Mappings;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        CreateMap<PlayerDtoRequest, Player>();
        CreateMap<Player, PlayerDtoResponse>();
    }
}
