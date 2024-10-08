﻿using ChatManagement.DataAccess;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Infrastructure.CustomException;
using ChatManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ChatDomain = ChatManagement.Domain.Models.Chat;

namespace Chat.Management.Data.Access.Tests.RepositoryTests;

public class ChatRepositoryTests
{
    private readonly ChatRepository _chatRepository;
    private readonly ChatManagementDbContext _dbContext;

    public  ChatRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ChatManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "ChatManagementTestDb")
            .Options;
        _dbContext = new ChatManagementDbContext(options);
        _chatRepository = new ChatRepository(_dbContext);
    }
    
    [Fact]
    public async Task AddChatAsync_ChatDoesNotExist_AddsChatSuccessfully()
    {
        var chatDto = new ChatDto
        {
            Title = "Test Chat",
            UserIds = new List<Guid>()
        };
        var userId = Guid.NewGuid();

        await _chatRepository.AddChatAsync(chatDto, userId);

        var chat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.Title == "Test Chat");
        Assert.NotNull(chat);
        
        Assert.Equal("Test Chat", chat.Title);
        Assert.Contains(userId, chat.UserIds.ToList());
    }

    [Fact]
    public async Task AddChatAsync_ChatExists_ThrowsApiException()
    {
        var userId = Guid.NewGuid();
        var chat = new ChatDomain
        {
            Title = "Existing Chat",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        var chatDto = new ChatDto
        {
            Title = "Existing Chat",
            UserIds = new List<Guid>()
        };

        await Assert.ThrowsAsync<ApiException>(() => _chatRepository.AddChatAsync(chatDto, userId));
    }

    [Fact]
    public async Task GetChatsByUserIdAsync_UserHasChats_ReturnsChats()
    {
        var userId = Guid.NewGuid();
        var chat = new ChatDomain
        {
            Title = "User Chat",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        var chats = (await _chatRepository.GetChatsByUserIdAsync(userId))
            .ToList();
        
        Assert.Single(chats);
        Assert.Equal("User Chat", chats.First().Title);
    }

    [Fact]
    public async Task UpdateChatAsync_ChatExistsAndUserIsCreator_UpdatesChat()
    {
        var userId = Guid.NewGuid();
        var chat = new ChatDomain
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        var chatDto = new ChatDto
        {
            Id = chat.Id,
            Title = "Updated Title",
            UserIds = [userId]
        };

        await _chatRepository.UpdateChatAsync(chatDto, userId);

        var updatedChat = await _dbContext.Chats.FindAsync(chat.Id);
        Assert.Equal("Updated Title", updatedChat!.Title);
    }

    [Fact]
    public async Task UpdateChatAsync_UserNotCreator_ThrowsAccessViolationException()
    {
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var chat = new ChatDomain
        {
            Id = Guid.NewGuid(),
            Title = "Original Title",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        var chatDto = new ChatDto
        {
            Id = chat.Id,
            Title = "Updated Title",
            UserIds = [userId]
        };

        await Assert.ThrowsAsync<AccessViolationException>(() => _chatRepository.UpdateChatAsync(chatDto, anotherUserId));
    }

    [Fact]
    public async Task RemoveChatAsync_ChatExistsAndUserIsCreator_RemovesChat()
    {
        var userId = Guid.NewGuid();
        var chat = new ChatDomain
        {
            Id = Guid.NewGuid(),
            Title = "Chat to Remove",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await _chatRepository.RemoveChatAsync(chat.Id, userId);

        var removedChat = await _dbContext.Chats.FindAsync(chat.Id);
        Assert.Null(removedChat);
    }

    [Fact]
    public async Task RemoveChatAsync_UserNotCreator_ThrowsAccessViolationException()
    {
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
      
        var chat = new ChatDomain
        {
            Id = Guid.NewGuid(),
            Title = "Chat to Remove",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<AccessViolationException>(() => _chatRepository.RemoveChatAsync(chat.Id, anotherUserId));
    }

    [Fact]
    public async Task AttachUserToChatAsync_UserNotAttached_AttachesUser()
    {
        var userId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        var chat = new ChatDomain
        {
            Id = chatId,
            Title = "Chat",
            UserIds = new List<Guid>(),
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await _chatRepository.AttachUserToChatAsync(chatId, userId);

        var updatedChat = await _dbContext.Chats.FindAsync(chatId);
        Assert.Contains(userId, updatedChat!.UserIds.ToList());
    }

    [Fact]
    public async Task AttachUserToChatAsync_UserAlreadyAttached_ThrowsUserAttachmentException()
    {
        var userId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        
        var chat = new ChatDomain
        {
            Id = chatId,
            Title = "Chat",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<UserAttachmentException>(() => _chatRepository.AttachUserToChatAsync(chatId, userId));
    }

    [Fact]
    public async Task DetachUserFromChatAsync_UserAttached_DetachesUser()
    {
        var creatorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        
        var chat = new ChatDomain
        {
            Id = chatId,
            Title = "Chat",
            UserIds = [userId],
            CreatorId = creatorId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await _chatRepository.DetachUserFromChatAsync(chatId, userId);
        
        var updatedChat = await _dbContext.Chats.FindAsync(chatId);
        Assert.DoesNotContain(userId, updatedChat!.UserIds);
    }
    
    [Fact]
    public async Task DetachUserFromChatAsync_UserIsCreator_ThrowsAccessViolationException()
    {
        var userId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
        
        var chat = new ChatDomain
        {
            Id = chatId,
            Title = "Chat",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<AccessViolationException>(() => _chatRepository.DetachUserFromChatAsync(chatId, userId));
    }

    [Fact]
    public async Task DetachUserFromChatAsync_UserNotAttached_ThrowsUserAttachmentException()
    {
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var chatId = Guid.NewGuid();
      
        var chat = new ChatDomain
        {
            Id = chatId,
            Title = "Chat",
            UserIds = [userId],
            CreatorId = userId,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<UserAttachmentException>(() => _chatRepository.DetachUserFromChatAsync(chatId, anotherUserId));
    }
}