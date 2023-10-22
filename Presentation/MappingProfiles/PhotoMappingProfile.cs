namespace Presentation.MappingProfiles;

using Application.Models;
using AutoMapper;
using Presentation.Dtos.Gallery;

public class PhotoMappingProfile : Profile {
    public PhotoMappingProfile()
    {
        CreateMap<Photo, PhotoInfoDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(src => src.Title)
            );
        
        CreateMap<Photo, PhotoDetailDto>()
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(src => src.Title)
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Description)
            )
            .ForMember(
                dest => dest.PhotoFormat,
                opt => opt.MapFrom(src => src.Format)
            )
            .ForMember(
                dest => dest.PhotoHeight,
                opt => opt.MapFrom(src => src.Heigth)
            )
            .ForMember(
                dest => dest.PhotoWidth,
                opt => opt.MapFrom(src => src.Width)
            );

        CreateMap<PhotoUploadRequest, Photo>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => -1)
            )
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(src => src.Title)
            )
            .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Description)
            )
            .ForMember(
                dest => dest.Format,
                opt => opt.MapFrom(src => "image/png")
            )
            .ForMember(//todo: change this to take the actual image props
                dest => dest.Heigth,
                opt => opt.MapFrom(src => 0)
            )
            .ForMember(
                dest => dest.Width,
                opt => opt.MapFrom(src => 0)
            );
    }
}