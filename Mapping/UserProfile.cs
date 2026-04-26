using AutoMapper;
using UserManagementApi.DTOs;
using UserManagementApi.Models;

namespace UserManagementApi.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserReadDto>();
    }
}
