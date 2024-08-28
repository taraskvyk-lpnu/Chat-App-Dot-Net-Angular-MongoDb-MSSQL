using ChatManagement.Domain;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.MappingExtensions;

namespace ChatManagement.Services.Services;

public class ChatManagementService : IChatManagementService
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatManagementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task AddChatAsync(AddChatRequest addChatRequest)
    {
        var chat = new Chat
        {
            CreatorId = addChatRequest.CreatorId,
            CreatedAt = DateTime.Now,
            Title = addChatRequest.Title,
            UserIds = addChatRequest.UserIds ?? new List<Guid>()
        };

        await _unitOfWork.Chat.AddChatAsync(chat);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateChatAsync(Guid id, UpdateChatRequest updateChatRequest)
    {
        var chat = new Chat
        {
            Id = id,
            Title = updateChatRequest.Title,
            UserIds = updateChatRequest.UserIds ?? new List<Guid>()
        };
        
        await _unitOfWork.Chat.UpdateChatAsync(chat, updateChatRequest.UserId);
        await _unitOfWork.CommitAsync();
    }

    public async Task RemoveChatAsync(RemoveChatRequest deleteChatRequest)
    {
        await _unitOfWork.Chat.RemoveChatAsync(deleteChatRequest.ChatId, deleteChatRequest.UserId);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<ChatDto>> GetAllChatsAsync()
    {
        var chats = await _unitOfWork.Chat.GetAllAsync();
        return chats.Select(c => c.ToDto());
    }

    public async Task<ChatDto> GetChatByIdAsync(Guid chatId)
    {
        var chat = await _unitOfWork.Chat.GetChatByIdAsync(chatId);
        return chat.ToDto();
    }

    public async Task AttachUserToChatAsync(AttachUserRequest addUserToChatRequest)
    {
        await _unitOfWork.Chat.AttachUserToChatAsync(addUserToChatRequest.ChatId, addUserToChatRequest.UserToAddId);
        await _unitOfWork.CommitAsync();
    }
    public async Task DetachUserFromChatAsync(DetachUserRequset detachUserRequest)
    {
        await _unitOfWork.Chat.DetachUserFromChatAsync(detachUserRequest.ChatId, detachUserRequest.UserToDetachId, detachUserRequest.DetachedByUserId);
        await _unitOfWork.CommitAsync();
    }
}