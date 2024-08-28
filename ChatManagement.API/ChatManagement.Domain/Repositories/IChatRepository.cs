using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Repositories;

public interface IChatRepository : IRepository<Chat>
{
    Task AttachUserToChatAsync(Guid chatId, Guid userId);
    Task DetachUserFromChatAsync(Guid chatId, Guid userId);
    Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
    
    //Task<IEnumerable<User>> GetUsersByChatIdAsync(string chatId);
    Task<Chat> GetChatByIdAsync(Guid chatId);
    Task AddChatAsync(Chat chat);
    Task UpdateChatAsync(Chat chat, Guid userId);
    Task RemoveChatAsync(Guid chatId, Guid userId);
}