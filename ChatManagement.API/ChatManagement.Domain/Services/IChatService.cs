using ChatManagement.Domain.Models;

namespace ChatManagement.Domain.Services;

public interface IChatService
{
    Task AddChatAsync(Chat chat);
    Task UpdateChatAsync(Chat chat);
    Task RemoveChatAsync(Chat chat);
    Task<IEnumerable<Chat>> GetAllChatsAsync();
    Task<Chat> GetChatByIdAsync(string chatId);
    
    Task AttachUserToChatAsync(string chatId, string userId);
    Task DetachUserFromChatAsync(string chatId, string userId);
}