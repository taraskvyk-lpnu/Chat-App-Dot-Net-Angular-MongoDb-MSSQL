using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Services;

public interface IChatManagementService
{
    Task<ChatDto> AddChatAsync(AddChatRequest addChatRequest);
    Task<ChatDto>  UpdateChatAsync(UpdateChatRequest updateChatRequest);
    Task RemoveChatAsync(RemoveChatRequest deleteChatRequest);
    Task<IEnumerable<ChatDto>> GetAllChatsAsync();
    Task<ChatDto> GetChatByIdAsync(Guid chatId);
    Task<IEnumerable<ChatDto>> GetChatsByUserIdAsync(Guid userId);
    
    Task<ChatDto> AttachUserToChatAsync(AttachUserRequest addUserToChatRequest);
    Task<ChatDto> DetachUserFromChatAsync(DetachUserRequest detachUserRequest);
}