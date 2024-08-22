using ChatManagement.Domain;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Services;

namespace ChatManagement.Services.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public Task AddChatAsync(Chat chat)
    {
        throw new NotImplementedException();
    }

    public Task UpdateChatAsync(Chat chat)
    {
        throw new NotImplementedException();
    }

    public Task RemoveChatAsync(Chat chat)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Chat>> GetAllChatsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Chat> GetChatByIdAsync(string chatId)
    {
        throw new NotImplementedException();
    }

    public Task AttachUserToChatAsync(string chatId, string userId)
    {
        throw new NotImplementedException();
    }

    public Task DetachUserFromChatAsync(string chatId, string userId)
    {
        throw new NotImplementedException();
    }
}