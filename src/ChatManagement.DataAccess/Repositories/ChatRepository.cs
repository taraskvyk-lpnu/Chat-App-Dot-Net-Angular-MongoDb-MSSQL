﻿using ChatManagement.DataAccess;
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
    
    public async Task<Chat> AddChatAsync(ChatDto chatDto, Guid userId)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Title == chatDto.Title);

        if (chat != null)
        {
            throw new ApiException($"Chat \"{chat.Title}\" already exists");
        }

        chatDto.UserIds.Add(userId);

        chat = new Chat
        {
            Title = chatDto.Title,
            UserIds = chatDto.UserIds,
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _chatContext.Chats.AddAsync(chat);
        await _chatContext.SaveChangesAsync();
        
        return chat;
    }
    
    public async Task UpdateChatAsync(ChatDto chatDto, Guid userId)
    {
        var chat = await _chatContext.Chats.FirstOrDefaultAsync(c => c.Id == chatDto.Id);

        if (chat == null)
        {
            throw new NotFoundException("Chat does not exist");
        }
        
        if (chat.CreatorId != userId)
        {
            throw new AccessViolationException("You can't update this chat");
        }
        
        chat.Title = chatDto.Title;
        chat.UserIds = chatDto.UserIds;
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
            throw new AccessViolationException("You can't detach yourself from chat");
        }
        
        chat.UserIds.Remove(userId);
        await _chatContext.SaveChangesAsync();
    }
}