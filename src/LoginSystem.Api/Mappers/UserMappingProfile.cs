using AutoMapper;
using LoginSystem.Api.Models.Request;
using LoginSystem.Api.Models.Response;
using LoginSystem.Domain.Models;

namespace LoginSystem.Api.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserRequestDto, User>()
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

        CreateMap<User, CreateUserResponse>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName));

        CreateMap<User, SearchUserResponse>();

        CreateMap<User, UpdateRegistrationStatusRequest>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => src.UserStatus))
            .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy));

        CreateMap<User, DeleteIndividualUserResponse>();

        CreateMap<User, UpdateRegistrationStatusResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        CreateMap<UpdateRegistrationStatusResponse, User>();
    }
}
