using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Services;

public interface IChatService
{
    Task AddChatAsync(Chat chat);
    Task UpdateChatAsync(Chat chat);
    Task RemoveChatAsync(Chat chat);
    Task<IEnumerable<ChatDto>> GetAllChatsAsync();
    Task<ChatDto> GetChatByIdAsync(Guid chatId);
    
    Task AttachUserToChatAsync(Guid chatId, Guid userId);
    Task DetachUserFromChatAsync(Guid chatId, Guid userId);
}