using ChatManagement.DataAccess;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Repositories;
using ChatManagement.Infrastructure.CustomException;
using Microsoft.EntityFrameworkCore;
namespace ChatManagement.Infrastructure.Repositories;

public class ChatRepository : Repository<Chat>, IChatRepository
{
    public ChatRepository(ChatManagementDbContext chatContext) : base(chatContext)
    {
    }

    public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
    {
        return await _chatContext.Chats
            .Where(c => c.UserIds.Contains(userId))
            .ToListAsync();
    }
    
    
    public async Task AddChatAsync(Chat chat, Guid userId)
    {
        var existingChat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Title == chat.Title);

        if (existingChat != null)
        {
            throw new ApiException($"Chat \"{chat.Title}\" already exists");
        }

        chat.UserIds.Add(userId);
        
        await _chatContext.Chats.AddAsync(chat);
        await _chatContext.SaveChangesAsync();
    }
    
    public async Task UpdateChatAsync(Chat chat, Guid userId)
    {
        var existingChat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chat.Id);

        if (existingChat == null)
        {
            throw new NotFoundException("Chat does not exist");
        }
        
        if (existingChat.CreatorId != userId)
        {
            throw new AccessViolationException("You can't update this chat");
        }
        
        existingChat.Title = chat.Title;
        existingChat.UserIds = chat.UserIds;
        await _chatContext.SaveChangesAsync();
    }
    
    public async Task RemoveChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            throw new NotFoundException("Chat not found");
        }

        if (chat.CreatorId != userId)
        {
            throw new AccessViolationException("You can't delete this chat");
        }
        
        _chatContext.Chats.Remove(chat);
        await _chatContext.SaveChangesAsync();
    }
    
    public async Task AttachUserToChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);
        
        if (chat == null)
        {
            throw new NotFoundException("Chat not found");
        }
        
        if (chat.UserIds.Contains(userId))
        {
            throw new UserAttachmentException("User already attached to chat");
        }
        
        chat.UserIds.Add(userId);
        await _chatContext.SaveChangesAsync();
    }

    public async Task DetachUserFromChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            throw new NotFoundException("Chat not found");
        }
        
        if (!chat.UserIds.Contains(userId))
        {
            throw new UserAttachmentException("This user hadn't been attached to chat");
        }
        
        if(chat.UserIds.Contains(userId) && chat.CreatorId == userId)
        {
            throw new AccessViolationException("You can't detach yourself from the chat");
        }
        
        chat.UserIds.Remove(userId);
        await _chatContext.SaveChangesAsync();
    }
}