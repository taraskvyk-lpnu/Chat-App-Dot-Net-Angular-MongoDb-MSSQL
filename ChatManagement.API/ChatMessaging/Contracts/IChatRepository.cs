using ChatMessaging.Models;

namespace ChatMessaging.Contracts;

public interface IChatRepository
{
    Task<List<Message>> GetMessagesAsync(Guid id);
    Task SaveMessageAsync(Guid chatId, Message message);
}