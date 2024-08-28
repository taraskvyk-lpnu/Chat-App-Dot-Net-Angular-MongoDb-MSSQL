using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;

namespace ChatManagement.Domain.Services;

public interface IChatManagementService
{
    Task AddChatAsync(AddChatRequest addChatRequest);
    Task UpdateChatAsync(Guid id, UpdateChatRequest updateChatRequest);
    Task RemoveChatAsync(RemoveChatRequest deleteChatRequest);
    Task<IEnumerable<ChatDto>> GetAllChatsAsync();
    Task<ChatDto> GetChatByIdAsync(Guid chatId);
    
    Task AttachUserToChatAsync(AttachUserRequest addUserToChatRequest);
    Task DetachUserFromChatAsync(DetachUserRequset detachUserRequest);
}