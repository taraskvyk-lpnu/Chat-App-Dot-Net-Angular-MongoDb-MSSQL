using ChatMessaging.Contracts;
using ChatMessaging.Models;
using ChatMessaging.Models.MessageRequests;
using ChatMessaging.Services.Contracts;

namespace ChatMessaging.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
    }

    public async Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        return await _messageRepository.GetMessagesAsync(chatId);
    }

    public async Task<List<Message>> GetMessagesByUserAsync(GetMessagesByUserRequest request)
    {
        return await _messageRepository.GetMessagesByUserAsync(request.ChatId, request.UserId);
    }

    public async Task AddMessageAsync(AddMessageRequest request)
    {
        await _messageRepository.AddMessageAsync(request.ChatId, request.Message);
    }

    public async Task UpdateMessageAsync(UpdateMessageRequest request)
    {
        await _messageRepository.UpdateMessageAsync(request.ChatId, request.UserId, request.Message);
    }

    public async Task DeleteMessageAsync(DeleteMessageRequest request)
    {
        await _messageRepository.DeleteMessageAsync(request.ChatId, request.MessageId, request.UserId);
    }
}
