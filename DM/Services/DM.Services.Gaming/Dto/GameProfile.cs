using System.Linq;
using AutoMapper;
using DM.Services.Gaming.Dto.Output;
using DtoGame = DM.Services.Gaming.Dto.Output.Game;
using DbGame = DM.Services.DataAccess.BusinessObjects.Games.Game;

namespace DM.Services.Gaming.Dto
{
    /// <summary>
    /// Profile for game DTO and DAL mapping
    /// </summary>
    public class GameProfile : Profile
    {
        /// <inheritdoc />
        public GameProfile()
        {
            CreateMap<DbGame, DtoGame>()
                .ForMember(d => d.Id, s => s.MapFrom(g => g.GameId));
            CreateMap<DbGame, GameExtended>()
                .ForMember(d => d.Id, s => s.MapFrom(g => g.GameId))
                .ForMember(d => d.Tags, s => s.MapFrom(g => g.GameTags.Select(t => t.Tag.Title)));
        }
    }
}