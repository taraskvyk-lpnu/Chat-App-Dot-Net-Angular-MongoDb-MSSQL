using ChatMessaging.Models;
using ChatMessaging.Models.MessageRequests;

namespace ChatMessaging.Services.Contracts;

public interface IMessageService
{
    Task<List<Message>> GetMessagesAsync(Guid chatId);
    Task<List<Message>> GetMessagesByUserAsync(GetMessagesByUserRequest request);
    Task<bool> AddMessageAsync(AddMessageRequest request);
    Task<bool> UpdateMessageAsync(UpdateMessageRequest request);
    Task DeleteMessageAsync(DeleteMessageRequest request);
}