using System.Net;
using System.Text;
using Chat.Management.API.IntegrationTests.WebFactory;
using ChatManagement.API;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Infrastructure.ResponseDtos;
using FluentAssertions;
using Newtonsoft.Json;
using ChatDomain = ChatManagement.Domain.Models.Chat;

namespace Chat.Management.API.IntegrationTests;

public class ChatsControllerIntegrationTests : IClassFixture<CustomWebFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ChatDomain _validChat;
    private readonly ChatDomain _chatToRemove;

    public ChatsControllerIntegrationTests(CustomWebFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _validChat = factory.GetValidChat();
        _chatToRemove = factory.GetChatToRemove();
    }
    
    
    [Fact]
    public async Task GetChats_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/chats");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetChat_WithValidId_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync($"/api/chats/{_validChat.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var receivedChat = JsonConvert.DeserializeObject<ChatDomain>(content);
        Assert.Equal(_validChat.Id, receivedChat!.Id);
    }
    
    [Fact]
    public async Task GetChat_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/chats/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }



    [Fact]
    public async Task RemoveChat_ReturnsSuccess()
    {
        // Arrange
        var requestUri = "/api/chats/remove-chat";
        var removeChatRequest = new RemoveChatRequest
        {
            ChatId = _chatToRemove.Id,
            UserId = _chatToRemove.CreatorId
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(removeChatRequest), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = content
        };

        // Act
        var response = await _client.SendAsync(requestMessage);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);

        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().Be("Chat deleted successfully");
    }
}