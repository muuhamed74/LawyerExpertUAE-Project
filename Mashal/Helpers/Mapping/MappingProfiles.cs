using AutoMapper;
using Core.DTOs.App;
using Core.DTOs.Identity;
using Core.Models;
using Core.Models.Identity;
using lawyer.Api.Helpers.Resolvers;

namespace lawyer.Api.Helpers.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<ContractTemplate, ContractTemplateDto>()
                  .ForMember(dest => dest.FileUrl, opt => opt.MapFrom<ContractFileUrlResolver>())
                  .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<ContractImageResolver>());   


            //CreateMap<UserContract, UserContractResponseDto>();

            //CreateMap<FillContractRequestDto, UserContract>()
            //      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));




            CreateMap<RegisterDto, AppUser>()
                  .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                  .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));


            CreateMap<RegisterDto, UserStoreTemporary>()
            .ForMember(dest => dest.Code, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore());


            CreateMap<UserStoreTemporary, AppUser>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Adress, opt => opt.Ignore())
                    .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                    .ForMember(dest => dest.RefreshTokenExpiration, opt => opt.Ignore());


            CreateMap<RegisterDto, AppUser>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Adress, opt => opt.Ignore())
                    .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                    .ForMember(dest => dest.RefreshTokenExpiration, opt => opt.Ignore());
        }
    }
}

