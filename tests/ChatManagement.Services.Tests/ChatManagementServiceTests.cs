using AutoMapper;
using ChatManagement.Domain;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Repositories;
using ChatManagement.Domain.Services;
using ChatManagement.Services.Services;
using Moq;
using ChatDomain = ChatManagement.Domain.Models.Chat;

namespace ChatManagement.Services.Tests;

public class ChatManagementServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IChatManagementService _chatManagementService;

    public ChatManagementServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        Mock<IChatRepository> chatRepository = new();
        
        chatRepository.Setup(c => c.AddAsync(It.IsAny<ChatDomain>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(m => m.Chat).Returns(chatRepository.Object);

        _chatManagementService = new ChatManagementService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddChatAsync_CallsMapper_AddAndCommit()
    {
        var addChatRequest = new AddChatRequest
        {
            CreatorId = Guid.NewGuid(),
            Title = "New Chat",
            UserIds = new List<Guid> { Guid.NewGuid() }
        };

        var chatDto = new ChatDto(); // Створюємо мокану DTO, яку поверне AutoMapper
        _mockMapper.Setup(m => m.Map<ChatDto>(addChatRequest)).Returns(chatDto);

        await _chatManagementService.AddChatAsync(addChatRequest);

        _mockMapper.Verify(m => m.Map<ChatDto>(addChatRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.Chat.AddChatAsync(chatDto, addChatRequest.CreatorId), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateChatAsync_CallsMapper_UpdateAndCommit()
    {
        var updateChatRequest = new UpdateChatRequest
        {
            ChatId = Guid.NewGuid(),
            Title = "Updated Chat",
            UserId = Guid.NewGuid(),
            UserIds = new List<Guid> { Guid.NewGuid() }
        };

        var chatDto = new ChatDto();
        _mockMapper.Setup(m => m.Map<ChatDto>(updateChatRequest)).Returns(chatDto);

        await _chatManagementService.UpdateChatAsync(updateChatRequest);

        _mockMapper.Verify(m => m.Map<ChatDto>(updateChatRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.Chat.UpdateChatAsync(chatDto, updateChatRequest.UserId), Times.Once);
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
    public async Task GetAllChatsAsync_ReturnsMappedChats()
    {
        var chatDomains = new List<ChatDomain>
        {
            new ChatDomain { Id = Guid.NewGuid(), Title = "Chat 1" },
            new ChatDomain { Id = Guid.NewGuid(), Title = "Chat 2" }
        };

        _mockUnitOfWork.Setup(u => u.Chat.GetAllAsync()).ReturnsAsync(chatDomains);

        var chatDtos = new List<ChatDto> { new ChatDto(), new ChatDto() };
        _mockMapper.Setup(m => m.Map<IEnumerable<ChatDto>>(chatDomains)).Returns(chatDtos);

        var result = await _chatManagementService.GetAllChatsAsync();

        Assert.Equal(2, result.Count());
        _mockMapper.Verify(m => m.Map<IEnumerable<ChatDto>>(chatDomains), Times.Once);
        _mockUnitOfWork.Verify(u => u.Chat.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetChatByIdAsync_ReturnsMappedChat()
    {
        var chatId = Guid.NewGuid();
        var chatDomain = new ChatDomain { Id = chatId, Title = "Chat" };

        _mockUnitOfWork.Setup(u => u.Chat.GetByIdAsync(chatId)).ReturnsAsync(chatDomain);

        var chatDto = new ChatDto();
        _mockMapper.Setup(m => m.Map<ChatDto>(chatDomain)).Returns(chatDto);

        var result = await _chatManagementService.GetChatByIdAsync(chatId);

        Assert.NotNull(result);
        _mockMapper.Verify(m => m.Map<ChatDto>(chatDomain), Times.Once);
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

        var chatDomain = new ChatDomain();
        _mockUnitOfWork.Setup(u => u.Chat.AttachUserToChatAsync(attachUserRequest.ChatId, attachUserRequest.UserToAddId)).ReturnsAsync(chatDomain);

        var chatDto = new ChatDto();
        _mockMapper.Setup(m => m.Map<ChatDto>(chatDomain)).Returns(chatDto);

        var result = await _chatManagementService.AttachUserToChatAsync(attachUserRequest);
        
        _mockUnitOfWork.Verify(u => u.Chat.AttachUserToChatAsync(attachUserRequest.ChatId, attachUserRequest.UserToAddId), Times.Once);
        _mockMapper.Verify(m => m.Map<ChatDto>(chatDomain), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DetachUserFromChatAsync_CallsDetachAndCommit()
    {
        var detachUserRequest = new DetachUserRequest
        {
            ChatId = Guid.NewGuid(),
            UserToDetachId = Guid.NewGuid(),
            DetachedByUserId = Guid.NewGuid()
        };

        var chatDomain = new ChatDomain();
        _mockUnitOfWork.Setup(u => u.Chat.DetachUserFromChatAsync(detachUserRequest.ChatId, detachUserRequest.UserToDetachId)).ReturnsAsync(chatDomain);

        var chatDto = new ChatDto();
        _mockMapper.Setup(m => m.Map<ChatDto>(chatDomain)).Returns(chatDto);

        var result = await _chatManagementService.DetachUserFromChatAsync(detachUserRequest);
        
        _mockUnitOfWork.Verify(u => u.Chat.DetachUserFromChatAsync(detachUserRequest.ChatId, detachUserRequest.UserToDetachId), Times.Once);
        _mockMapper.Verify(m => m.Map<ChatDto>(chatDomain), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
