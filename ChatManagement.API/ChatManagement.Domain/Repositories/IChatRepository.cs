using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Repositories;

public interface IChatRepository : IRepository<Chat>
{
    Task AttachUserToChatAsync(Guid chatId, Guid userId);
    Task DetachUserFromChatAsync(Guid chatId, Guid userId);
    Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
    
    //Task<IEnumerable<User>> GetUsersByChatIdAsync(string chatId);
    Task AddChatAsync(ChatDto chatDto, Guid userId);
    Task UpdateChatAsync(ChatDto chatDto, Guid userId);
    Task DeleteChatAsync(Guid chatId, Guid userId);
}