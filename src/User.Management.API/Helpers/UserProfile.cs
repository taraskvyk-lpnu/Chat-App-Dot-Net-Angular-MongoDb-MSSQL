using AutoMapper;
using User.Management.Domain.Models;

namespace User.Management.API.Helpers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}