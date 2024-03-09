
using SixMinAPI.Models;
using SixMinAPI.Dtos;

namespace SixMinAPI.Profiles
{
    public class CommandsProfile : AutoMapper.Profile
    {
        public CommandsProfile()
        {
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
        }
    }
}