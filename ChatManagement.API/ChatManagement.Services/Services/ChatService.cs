using ChatManagement.Domain;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.MappingExtensions;

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
        var chatDto = chat.ToDto();
        return _unitOfWork.Chat.AddAsync(chatDto);
    }

    public Task UpdateChatAsync(Chat chat)
    {
        var chatDto = chat.ToDto();
        return _unitOfWork.Chat.UpdateChatAsync(chatDto);
    }

    public Task RemoveChatAsync(Chat chat)
    {
        return _unitOfWork.Chat.RemoveByIdAsync(chat.Id);
    }

    public async Task<IEnumerable<ChatDto>> GetAllChatsAsync()
    {
        return await _unitOfWork.Chat.GetAllAsync();
    }

    public async Task<ChatDto> GetChatByIdAsync(Guid chatId)
    {
        return await _unitOfWork.Chat.GetByIdAsync(chatId);
    }

    public async Task AttachUserToChatAsync(Guid chatId, Guid userId)
    {
        await _unitOfWork.Chat.AttachUserToChatAsync(chatId, userId);
    }

    public async Task DetachUserFromChatAsync(Guid chatId, Guid userId)
    {
        await _unitOfWork.Chat.AttachUserToChatAsync(chatId, userId);
    }
}