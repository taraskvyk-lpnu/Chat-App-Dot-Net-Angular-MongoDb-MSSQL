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

    public async Task AttachUserToChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            throw new NotFoundException("Chat not found");
        }
        
        if (chat.UserIds.Contains(chatId))
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
        
        if (!chat.UserIds.Contains(chatId))
        {
            throw new UserAttachmentException("This user hadn't been  attached to chat");
        }
        
        chat.UserIds.Remove(userId);
        await _chatContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
    {
        return await _chatContext.Chats
            .Where(c => c.UserIds.Contains(userId))
            .ToListAsync();
    }

    public async Task UpdateChatAsync(ChatDto chatDto)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chatDto.Id);

        if (chat == null)
        {
            throw new NotFoundException("Chat not found");
        }
        
        chat.Title = chatDto.Title;
        chat.UserIds = chatDto.UserIds;
        await _chatContext.SaveChangesAsync();
    }
}