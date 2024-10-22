using AutoMapper;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Infrastructure.MappingProfilers;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Chat, ChatDto>().ReverseMap();
        
        CreateMap<AddChatRequest, ChatDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserIds, opt => opt.MapFrom(src => src.UserIds ?? new List<Guid>())) // Якщо UserIds null, встановлюємо порожній список
            .ReverseMap();
        
        CreateMap<UpdateChatRequest, ChatDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ChatId))
            .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserIds, opt => opt.MapFrom(src => src.UserIds ?? new List<Guid>())) // Якщо UserIds null, встановлюємо порожній список
            .ReverseMap();
    }
}