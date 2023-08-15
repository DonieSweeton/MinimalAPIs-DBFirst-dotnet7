using AutoMapper;
using MinimalDBFirst.Models;
using MinimalDBFirst.Models.DTO;

namespace MinimalDBFirst.AutoMapper
{
    public class AutoMappingConfig : Profile
    {
        public AutoMappingConfig()
        {
            CreateMap<User, GetAllDTO>();
            CreateMap<User, GetSmDTO>();
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, EditUserDTO>().ReverseMap();
        }
    }
}
