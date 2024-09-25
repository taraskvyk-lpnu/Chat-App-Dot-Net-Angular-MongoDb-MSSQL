using ChatMessaging.Contracts;
using ChatMessaging.Models;
using ChatMessaging.Models.MessageRequests;
using ChatMessaging.Services.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

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

    public async Task<bool> AddMessageAsync(AddMessageRequest request)
    {
        if (string.IsNullOrEmpty(request.Message.Text.Trim()))
        {
            return false;
        }
        
        await _messageRepository.AddMessageAsync(request.ChatId, request.Message);
        return true;
    }

    public async Task<bool> UpdateMessageAsync(UpdateMessageRequest request)
    {
        if (string.IsNullOrEmpty(request.Message.Text.Trim()))
        {
            return false;
        }
        
        await _messageRepository.UpdateMessageAsync(request.ChatId, request.UserId, request.Message);
        return true;
    }

    public async Task DeleteMessageAsync(DeleteMessageRequest request)
    {
        await _messageRepository.DeleteMessageAsync(request.ChatId, request.MessageId, request.UserId);
    }
}
