using AutoMapper;
using FiapCLoud.Domain.Entities;
using FIapCloud.App.Dtos;

namespace FIapCloud.App.Mappings;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        CreateMap<CreateGameRequest, Game>()
            .ConstructUsing(src => new Game(src.Name, src.Description, src.Price));
    }
}
