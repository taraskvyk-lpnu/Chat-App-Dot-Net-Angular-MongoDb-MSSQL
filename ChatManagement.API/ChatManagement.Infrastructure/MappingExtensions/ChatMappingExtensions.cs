﻿using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Infrastructure.MappingExtensions;

public static class ChatMappingExtensions
{
    public static ChatDto ToDto(this Chat chat)
    {
        return new ChatDto
        {
            Id = chat.Id,
            Title = chat.Title,
            UserIds = chat.UserIds
        };
    }
    
    public static Chat ToDomain(this ChatDto chatDto)
    {
        return new Chat
        {
            Id = chatDto.Id,
            Title = chatDto.Title,
            UserIds = chatDto.UserIds
        };
    }
}