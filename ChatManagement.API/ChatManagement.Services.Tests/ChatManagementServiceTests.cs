using ChatManagement.Domain;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Repositories;
using ChatManagement.Domain.Services;
using ChatManagement.Services.Services;
using ChatDomain = ChatManagement.Domain.Models.Chat;
using Moq;

namespace ChatManagement.Services.Tests;

public class ChatManagementServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly IChatManagementService _chatManagementService;

    public ChatManagementServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        Mock<IChatRepository> chatRepository = new();
        
        chatRepository.Setup(c => c.AddAsync(It.IsAny<ChatDomain>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(m => m.Chat).Returns(chatRepository.Object);

        _chatManagementService = new ChatManagementService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task AddChatAsync_CallsAddAndCommit()
    {
        var addChatRequest = new AddChatRequest
        {
            CreatorId = Guid.NewGuid(),
            Title = "New Chat",
            UserIds = [Guid.NewGuid()]
        };

        await _chatManagementService.AddChatAsync(addChatRequest);

        _mockUnitOfWork.Verify(u => u.Chat.AddAsync(It.IsAny<ChatDomain>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateChatAsync_CallsUpdateAndCommit()
    {
        var updateChatRequest = new UpdateChatRequest
        {
            ChatId = Guid.NewGuid(),
            Title = "Updated Chat",
            UserId = Guid.NewGuid(),
            UserIds = new List<Guid> { Guid.NewGuid() }
        };

        await _chatManagementService.UpdateChatAsync(updateChatRequest);

        _mockUnitOfWork.Verify(u => u.Chat.UpdateChatAsync(It.IsAny<ChatDto>(), updateChatRequest.UserId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveChatAsync_CallsRemoveAndCommit()
    {
        var removeChatRequest = new RemoveChatRequest
        {
            ChatId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        await _chatManagementService.RemoveChatAsync(removeChatRequest);

        _mockUnitOfWork.Verify(u => u.Chat.RemoveChatAsync(removeChatRequest.ChatId, removeChatRequest.UserId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllChatsAsync_ReturnsChats()
    {
        _mockUnitOfWork.Setup(u => u.Chat.GetAllAsync())
            .ReturnsAsync(new List<ChatDomain>
            {
                new ChatDomain { Id = Guid.NewGuid(), Title = "Chat 1" },
                new ChatDomain { Id = Guid.NewGuid(), Title = "Chat 2" }
            });

        var result = await _chatManagementService.GetAllChatsAsync();

        Assert.Equal(2, result.Count());
        _mockUnitOfWork.Verify(u => u.Chat.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetChatByIdAsync_ReturnsChat()
    {
        var chatId = Guid.NewGuid();
        _mockUnitOfWork.Setup(u => u.Chat.GetByIdAsync(chatId))
            .ReturnsAsync(
                new ChatDomain
                {
                    Id = chatId, 
                    Title = "Chat"
                });

        var result = await _chatManagementService.GetChatByIdAsync(chatId);

        Assert.NotNull(result);
        Assert.Equal(chatId, result.Id);
        _mockUnitOfWork.Verify(u => u.Chat.GetByIdAsync(chatId), Times.Once);
    }
    
    [Fact]
    public async Task AttachUserToChatAsync_CallsAttachAndCommit()
    {
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = Guid.NewGuid(),
            UserToAddId = Guid.NewGuid()
        };

        await _chatManagementService.AttachUserToChatAsync(attachUserRequest);
        
        _mockUnitOfWork.Verify(u => 
            u.Chat.AttachUserToChatAsync(attachUserRequest.ChatId, attachUserRequest.UserToAddId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
    
    
    [Fact]
    public async Task DetachUserToChatAsync_CallsAttachAndCommit()
    {
        var detachUserRequest = new DetachUserRequset()
        {
            ChatId = Guid.NewGuid(),
            UserToDetachId = Guid.NewGuid(),
            DetachedByUserId = Guid.NewGuid()
        };

        await _chatManagementService.DetachUserFromChatAsync(detachUserRequest);
        
        _mockUnitOfWork.Verify(u => 
            u.Chat.DetachUserFromChatAsync(detachUserRequest.ChatId, detachUserRequest.UserToDetachId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once); 
    }
}