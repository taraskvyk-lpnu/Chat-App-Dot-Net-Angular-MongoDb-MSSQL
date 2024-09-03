using ChatMessaging.Models;

namespace ChatMessaging.Contracts;

public interface IMessageRepository
{
    Task<List<Message>> GetMessagesAsync(Guid chatId);
    Task<List<Message>> GetMessagesByUserAsync(Guid chatId, Guid userId);
    Task AddMessageAsync(Guid chatId, Message message);
    Task UpdateMessageAsync(Guid chatId, Guid userId, Message message);
    Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId);
}