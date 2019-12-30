using System.Linq;
using AutoMapper;
using DatingApp.API.Models;
using DatingApp_API.Data;
using DatingApp_API.Dtos;

namespace DatingApp_API.Helpers
{
  // automapper uses profiles to help her understand the source and the destination of what is mapping.
  public class AutoMapperProfiles : Profile
  {
    public AutoMapperProfiles()
    {
      // CreateMap<source, destination>
      //           Model   Dto
      // After CreateMap<>(), need to set up age and photUrl

      // ForMember(), dest => destination Dto. MapFrom can get variable from source model
      // Search p.Ismain = true and then get its Url, finally insert Url into src.photos.Url
      CreateMap<User, UserForListDto>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>  // First mapping
          src.Photos.FirstOrDefault(p => p.IsMain).Url))
        .ForMember(dest => dest.Age, opt =>                          // Second mapping,
          opt.MapFrom(src => src.DateOfBirth.CalculateAge())); // Need get value from Helper extension

      CreateMap<User, UserForDetailedDto>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
          src.Photos.FirstOrDefault(p => p.IsMain).Url))
        .ForMember(dest => dest.Age, opt =>                          // Second mapping,
          opt.MapFrom(src => src.DateOfBirth.CalculateAge())); // Need get value from Helper extension
      CreateMap<Photo, PhotosForDetailedDto>();

      // Want to update User model
      // <sourceModel, destModel>
      CreateMap<UserForUpdateDto, User>();
      CreateMap<Photo, PhotoForReturnDto>();
      CreateMap<PhotoForCreationDto, Photo>();
      CreateMap<UserForRegisterDto, User>();
      CreateMap<MessageForCreationDto, Message>().ReverseMap(); // Map "Message" model into "MessageForCreationDto"
      CreateMap<Message, MessageToReturnDto>()
        .ForMember(m => m.SenderPhotoUrl, opt => 
          opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
        .ForMember(m => m.RecipientPhotoUrl, opt => 
          opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

    }
  }
}