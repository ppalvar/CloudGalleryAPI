namespace Presentation.MappingProfiles;

using AutoMapper;
using Application.Models;
using Presentation.Dtos.Auth;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserRegisterRequest, User>()
            .ForMember(
                dest => dest.Username,
                opt => opt.MapFrom(src => src.UserName)
            )
            .ForMember(
                dest => dest.IsBanned,
                opt => opt.MapFrom(src => false)
            )
            .ForMember(
                dest => dest.IsEmailVerified,
                opt => opt.MapFrom(src => false)
            )
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            );
    }
}