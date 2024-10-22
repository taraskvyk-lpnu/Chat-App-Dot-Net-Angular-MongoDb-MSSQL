using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Repositories;

public interface IChatRepository : IRepository<Chat>
{
    Task<Chat> AttachUserToChatAsync(Guid chatId, Guid userId);
    Task<Chat> DetachUserFromChatAsync(Guid chatId, Guid userId);
    Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);
    Task<Chat> AddChatAsync(ChatDto chatDto, Guid userId);
    Task<Chat> UpdateChatAsync(ChatDto chatDto, Guid userId);
    Task RemoveChatAsync(Guid chatId, Guid userId);
}