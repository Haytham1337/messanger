using Application.Models.MessageDto;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using AutoMapper;
using Domain.Entities;

namespace Infrastructure
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, GetMessageDto>();

            CreateMap<User, GetPhotoDto>();

            CreateMap<User, GetUserDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(u => u.Id))
                .ForMember(dest=>dest.PhotoName,src=>src.MapFrom(u=>u.Photo))
                .ForMember(dest=>dest.Age,src=>src.MapFrom(u=>u.Age))
                .ForMember(dest=>dest.NickName,src=>src.MapFrom(u=>u.NickName))
                .ForMember(dest=>dest.Phone,src=>src.MapFrom(u=>u.PhoneNumber));

            CreateMap<User, SearchUserDto>()
                .ForMember(dest=>dest.PhotoName,src=>src.MapFrom(u=>u.Photo));

        }
    }
}
