using ChatManagement.API.Controllers;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.ResponseDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatManagement.API.Tests.ControllerTests;


public class ChatsControllerTests
{
    private readonly Mock<IChatManagementService> _mockChatManagementService;
    private readonly ChatsController _controller;

    public ChatsControllerTests()
    {
        _mockChatManagementService = new Mock<IChatManagementService>();
        _controller = new ChatsController(_mockChatManagementService.Object);
    }

    [Fact]
    public async Task GetChats_ReturnsOkObjectResult_WithChats()
    {
        _mockChatManagementService.Setup(service => service.GetAllChatsAsync())
            .ReturnsAsync(new[] { new ChatDto { Id = Guid.NewGuid(), Title = "Test Chat" } });

        var result = await _controller.GetChats();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetChat_ReturnsOkObjectResult_WithChat()
    {
        var chatId = Guid.NewGuid();
        _mockChatManagementService.Setup(service => service.GetChatByIdAsync(chatId))
            .ReturnsAsync(new ChatDto { Id = chatId, Title = "Test Chat" });

        var result = await _controller.GetChat(chatId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task CreateChat_ReturnsResponseDto_WithSuccessMessage()
    {
        var addChatRequest = new AddChatRequest { Title = "New Chat" };

        var result = await _controller.CreateChat(addChatRequest);

        var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
        var responseDto = Assert.IsType<ResponseDto>(actionResult.Value);
        Assert.True(responseDto.IsSuccess);
        Assert.Equal("Chat created successfully", responseDto.Message);
    }

    [Fact]
    public async Task UpdateChat_ReturnsResponseDto_WithSuccessMessage()
    {
        var updateChatRequest = new UpdateChatRequest { ChatId = Guid.NewGuid(), Title = "Updated Chat" };

        var result = await _controller.UpdateChat(updateChatRequest.ChatId, updateChatRequest);

        var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
        var responseDto = Assert.IsType<ResponseDto>(actionResult.Value);
        Assert.True(responseDto.IsSuccess);
        Assert.Equal("Chat updated successfully", responseDto.Message);
    }

    [Fact]
    public async Task RemoveChat_ReturnsResponseDto_WithSuccessMessage()
    {
        var removeChatRequest = new RemoveChatRequest { ChatId = Guid.NewGuid() };

        var result = await _controller.RemoveChat(removeChatRequest);

        var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
        var responseDto = Assert.IsType<ResponseDto>(actionResult.Value);
        Assert.True(responseDto.IsSuccess);
        Assert.Equal("Chat deleted successfully", responseDto.Message);
    }

    [Fact]
    public async Task AttachUserToChat_ReturnsResponseDto_WithSuccessMessage()
    {
        var attachUserRequest = new AttachUserRequest { ChatId = Guid.NewGuid(), UserToAddId = Guid.NewGuid() };

        var result = await _controller.AttachUserToChat(attachUserRequest);

        var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
        var responseDto = Assert.IsType<ResponseDto>(actionResult.Value);
        Assert.True(responseDto.IsSuccess);
        Assert.Equal("User attached to chat successfully", responseDto.Message);
    }

    [Fact]
    public async Task DetachUserFromChat_ReturnsResponseDto_WithSuccessMessage()
    {
        var detachUserRequest = new DetachUserRequset { ChatId = Guid.NewGuid(), UserToDetachId = Guid.NewGuid() };

        var result = await _controller.DetachUserFromChat(detachUserRequest);

        var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
        var responseDto = Assert.IsType<ResponseDto>(actionResult.Value);
        Assert.True(responseDto.IsSuccess);
        Assert.Equal("User detached from chat successfully", responseDto.Message);
    }
}