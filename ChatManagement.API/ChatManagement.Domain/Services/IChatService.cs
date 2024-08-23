﻿using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Services;

public interface IChatService
{
    Task AddChatAsync(AddChatRequest addChatRequest);
    Task UpdateChatAsync(UpdateChatRequest updateChatRequest);
    Task RemoveChatAsync(RemoveChatRequest deleteChatRequest);
    Task<IEnumerable<ChatDto>> GetAllChatsAsync();
    Task<ChatDto> GetChatByIdAsync(Guid chatId);
    
    Task AttachUserToChatAsync(AttachUserRequest addUserToChatRequest);
    Task DetachUserFromChatAsync(DetachUserRequset detachUserRequest);
}