using System.Net;
using System.Net.Http.Json;
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

    #region GetChats
    
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
    
    #endregion

    #region GetChat
    
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
    
    #endregion

    #region CreateChat
    [Fact]
    public async Task CreateChat_ReturnsSuccess_WhenChatIsValid()
    {
        // Arrange
        var newChatRequest = new AddChatRequest
        {
            CreatorId = _validChat.CreatorId,
            Title = "New Test Chat1",
            UserIds = [Guid.NewGuid()],
            CreatorName = "Test User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats", newChatRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        
        Assert.True(responseDto!.IsSuccess);
        Assert.Equal("Chat created successfully", responseDto.Message);
    }

    [Fact]
    public async Task CreateChat_ReturnsBadRequest_WhenChatAlreadyExists()
    {
        // Arrange
        var duplicateChatRequest = new AddChatRequest
        {
            CreatorId = _validChat.CreatorId,
            Title = _validChat.Title,
            UserIds = _validChat.UserIds,
            CreatorName = "Test User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats", duplicateChatRequest);

        // Assert
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        
        Assert.False(responseDto!.IsSuccess);
        responseDto.Message.Should().Contain("already exists");
    }

    #endregion

    #region UpdateChat
    
    [Fact]
    public async Task UpdateChat_ReturnsSuccess_WhenChatIsUpdatedByCreator()
    {
        // Arrange
        var newUserIds = _validChat.UserIds.ToList();
        newUserIds.Add(Guid.NewGuid());
        
        var updatedChatRequest = new UpdateChatRequest
        {
            ChatId = _validChat.Id,
            UserId = _validChat.CreatorId,
            Title = _validChat.Title,
            UserIds = newUserIds
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/chats/{_validChat.Id}", updatedChatRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.True(responseDto!.IsSuccess);
        Assert.Equal("Chat updated successfully", responseDto.Message);

        // Verify the chat was updated
        var updatedChatResponse = await _client.GetAsync($"/api/chats/{_validChat.Id}");
        var updatedChat = await updatedChatResponse.Content.ReadFromJsonAsync<ChatDomain>();
        Assert.Equal(newUserIds, updatedChat!.UserIds);
    }
    
    [Fact]
    public async Task UpdateChat_ReturnsNotFound_WhenChatDoesNotExist()
    {
        // Arrange
        var updatedChatRequest = new UpdateChatRequest
        {
            ChatId = Guid.NewGuid(),
            UserId = _validChat.CreatorId,
            Title = _validChat.Title,
            UserIds = new List<Guid>()
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/chats/{updatedChatRequest.ChatId}", updatedChatRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateChat_ReturnsForbidden_WhenUserIsNotCreator()
    {
        // Arrange
        var updatedChatRequest = new UpdateChatRequest
        {
            Title = _validChat.Title,
            UserIds = []
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/chats/{_validChat.Id}", updatedChatRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("You can't update this chat", responseDto.Message);
    }
    
    #endregion

    #region RemoveChat
    
    [Fact]
    public async Task RemoveChat_ReturnsSuccess_WhenChatIsDeleted()
    {
        // Arrange
        var request = $"/api/chats/{_chatToRemove.Id}?userId={_chatToRemove.CreatorId}";
        
        // Act
        var response = await _client.DeleteAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.True(responseDto!.IsSuccess);
        Assert.Equal("Chat deleted successfully", responseDto.Message);
    }
    
    [Fact]
    public async Task RemoveChat_ReturnsNotFound_WhenChatDoesNotExist()
    {
        // Arrange
        var request = $"/api/chats/{Guid.NewGuid()}?userId={_chatToRemove.CreatorId}";
        
        // Act
        var response = await _client.DeleteAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task RemoveChat_ReturnsForbidden_WhenUserIsNotCreator()
    {
        // Arrange
        var request = $"/api/chats/{_validChat.Id}?userId={Guid.NewGuid()}";

        // Act
        var response = await _client.DeleteAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("You can't delete this chat", responseDto.Message);
    }
    
    #endregion
    
    #region AttachUserToChat
    
    [Fact]
    public async Task AttachUserToChat_ReturnsSuccess_WhenUserIsAttached()
    {
        // Arrange
        var userIdToAttach = Guid.NewGuid();
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = _validChat.Id,
            UserToAddId = userIdToAttach,
            AttachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/attach-user", attachUserRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.True(responseDto!.IsSuccess);
        Assert.Equal("User attached to chat successfully", responseDto.Message);

        // Verify the user was attached
        var chatResponse = await _client.GetAsync($"/api/chats/{_validChat.Id}");
        var chat = await chatResponse.Content.ReadFromJsonAsync<ChatDomain>();
        Assert.Contains(userIdToAttach, chat!.UserIds);
    }

    [Fact]
    public async Task AttachUserToChat_ReturnsNotFound_WhenChatDoesNotExist()
    {
        // Arrange
        var userIdToAttach = Guid.NewGuid();
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = Guid.NewGuid(),
            UserToAddId = userIdToAttach,
            AttachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/attach-user", attachUserRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("Chat not found", responseDto.Message);
    }

    [Fact]
    public async Task AttachUserToChat_ReturnsBadRequest_WhenUserAlreadyAttached()
    {
        // Arrange
        var userIdToAttach = _validChat.UserIds.First();
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = _validChat.Id,
            UserToAddId = userIdToAttach,
            AttachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/attach-user", attachUserRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("User already attached to chat", responseDto.Message);
    }
    
    #endregion
    
    #region DetachUserFromChat
    
     [Fact]
    public async Task DetachUserFromChat_ReturnsSuccess_WhenUserIsDetached()
    {
        // Arrange
        var userIdToDetach = _validChat.UserIds.First(id => id != _validChat.CreatorId);
        
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = _validChat.Id,
            UserToDetachId = userIdToDetach,
            DetachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/detach-user", detachUserRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.True(responseDto!.IsSuccess);
        Assert.Equal("User detached from chat successfully", responseDto.Message);

        // Verify the user was detached
        var chatResponse = await _client.GetAsync($"/api/chats/{_validChat.Id}");
        var chat = await chatResponse.Content.ReadFromJsonAsync<ChatDomain>();
        Assert.DoesNotContain(userIdToDetach, chat!.UserIds);
    }

    [Fact]
    public async Task DetachUserFromChat_ReturnsNotFound_WhenChatDoesNotExist()
    {
        // Arrange
        var userIdToDetach = Guid.NewGuid();
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = Guid.NewGuid(),
            UserToDetachId = userIdToDetach,
            DetachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/detach-user", detachUserRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("Chat not found", responseDto.Message);
    }

    [Fact]
    public async Task DetachUserFromChat_ReturnsBadRequest_WhenUserNotAttached()
    {
        // Arrange
        var userIdToDetach = Guid.NewGuid(); // User not attached to the chat
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = _validChat.Id,
            UserToDetachId = userIdToDetach,
            DetachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/detach-user", detachUserRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("This user hadn't been attached to chat", responseDto.Message);
    }

    [Fact]
    public async Task DetachUserFromChat_ReturnsForbidden_WhenUserIsCreator()
    {
        // Arrange
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = _validChat.Id,
            UserToDetachId = _validChat.CreatorId,
            DetachedByUserId = _validChat.CreatorId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/chats/detach-user", detachUserRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        Assert.False(responseDto!.IsSuccess);
        Assert.Equal("You can't detach yourself from chat", responseDto.Message);
    }
    
    #endregion
}