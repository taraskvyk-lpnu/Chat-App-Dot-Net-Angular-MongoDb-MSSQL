using System.Net;
using System.Text;
using ChatManagement.API.IntegrationTests.WebFactory;
using ChatManagement.Domain.Models;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Infrastructure.ResponseDtos;
using FluentAssertions;
using Newtonsoft.Json;

namespace ChatManagement.API.IntegrationTests.ControllersTests;

public class ChatsControllerIntegrationTests : IClassFixture<CustomWebFactory<Program>>
{
    private readonly CustomWebFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Chat _validChat;
    private readonly Chat _chatToRemove;

    public ChatsControllerIntegrationTests(CustomWebFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _validChat = _factory.GetValidChat();
        _chatToRemove = _factory.GetChatToRemove();
    }

    #region GetMethods

        [Fact]
        public async Task GetChats_ReturnsOk()
        {
            // Arrange
            var request = "/api/chats";

            // Act
            var response = await _client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task GetChats_InvalidRoute_ReturnsNotFound()
        {
            // Arrange
            var request = $"/api/chat/{_validChat.Id}";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task GetChat_ReturnsOk()
        {
            // Arrange
            var request = $"/api/chats/{_validChat.Id}";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            
            content.Should().Contain(_validChat.Id.ToString());
        }
        
        [Fact]
        public async Task GetChat_ReturnsNotFound()
        {
            // Arrange
            var request = $"/api/chats/{Guid.NewGuid()}";

            // Act
            var response = await _client.GetAsync(request);
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    
    #endregion

    #region CreateChatTests
    
    [Fact]
    public async Task CreateChat_ReturnsSuccess()
    {
        // Arrange
        var request = "/api/chats";
        var addChatRequest = new AddChatRequest
        {
            Title = "New Chat",
            CreatorName = "Test User",
            UserIds = [Guid.NewGuid()],
            CreatorId = Guid.NewGuid()
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(addChatRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);
        response.EnsureSuccessStatusCode();
        
        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().Be("Chat created successfully");
    }
    
    [Fact]
    public async Task CreateChat_ChatAlreadyExists_ReturnsError()
    {
        // Arrange
        var request = "/api/chats";
        var addChatRequest = new AddChatRequest
        {
            Title = _validChat.Title,
            CreatorName = "Test User",
            UserIds = [Guid.NewGuid()],
            CreatorId = Guid.NewGuid()
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(addChatRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);
        
        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.Message.Should().Be($"Chat \"{addChatRequest.Title}\" already exists");
        responseDto.IsSuccess.Should().BeFalse();
    }
    
    #endregion

    #region UpdateChatTests

    [Fact]
    public async Task UpdateChat_ReturnsSuccess()
    {
        var newUserIdsList = _validChat.UserIds.ToList();
        newUserIdsList.Add(Guid.NewGuid());
        
        // Arrange
        var request = $"/api/chats/{_validChat.Id}";
        var updateChatRequest = new UpdateChatRequest
        {
            ChatId = _validChat.Id,
            UserId = _validChat.CreatorId,
            Title = _validChat.Title,
            UserIds = newUserIdsList
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(updateChatRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(request, content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().Be("Chat updated successfully");
    }
    
    [Fact]
    public async Task UpdateChat_ChatNotExist_ReturnsNotFound()
    {
        // Arrange
        var notExistChatId = Guid.NewGuid();
        var request = $"/api/chats/{_validChat.Id}";
        var updateChatRequest = new UpdateChatRequest
        {
            ChatId = notExistChatId,
            UserId = _validChat.CreatorId,
            Title = "Updated Chat Title"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(updateChatRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(request, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateChat_UserIsNotAllowedToUpdate_ReturnsAccessAccessViolation()
    {
        // Arrange
        var userNotAllowedToUpdateId = Guid.NewGuid();
        var request = $"/api/chats/{_validChat.Id}";
        var updateChatRequest = new UpdateChatRequest
        {
            ChatId = _validChat.Id,
            UserId = userNotAllowedToUpdateId,
            Title = "Updated Chat Title"
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(updateChatRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(request, content);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeFalse();
        responseDto.Message.Should().Be("You can't update this chat");
    }
    
    #endregion

    #region RemoveChatTests
    
    [Fact]
    public async Task RemoveChat_ReturnsSuccess()
    {
        // Arrange
        var chatId = _chatToRemove.Id;
        var userId = _chatToRemove.CreatorId;
        var requestUri = $"/api/chats/{chatId}?userId={userId}";

        // Act
        var response = await _client.DeleteAsync(requestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);

        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().Be("Chat deleted successfully");
    }

    [Fact]
    public async Task RemoveChat_NonExistingChatId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingChatId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var requestUri = $"/api/chats/{nonExistingChatId}?userId={userId}";

        // Act
        var response = await _client.DeleteAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeFalse();
        responseDto.Message.Should().Contain("Chat not found");
    }
    
    [Fact]
    public async Task RemoveChat_NonExistingUserId_ReturnsForbidden()
    {
        // Arrange
        var chatId = _chatToRemove.Id;
        var nonExistingUserId = Guid.NewGuid();
        var requestUri = $"/api/chats/{chatId}?userId={nonExistingUserId}";

        // Act
        var response = await _client.DeleteAsync(requestUri);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeFalse();
        responseDto.Message.Should().Be("You can't delete this chat");
    }
    
    #endregion
    
    #region AttachUserTests
    
    [Fact]
    public async Task AttachUserToChat_ReturnsSuccess()
    {
        // Arrange
        var request = "/api/chats/attach-user";
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = _validChat.Id,
            AttachedByUserId = Guid.NewGuid()
        };
        var content = new StringContent(JsonConvert.SerializeObject(attachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().Be("User attached to chat successfully");
    }
    
    [Fact]
    public async Task AttachUserToChat_InvalidChatId_ReturnsBadRequest()
    {
        // Arrange
        var request = "/api/chats/attach-user";
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = Guid.Empty,
            AttachedByUserId = Guid.NewGuid()
        };
        var content = new StringContent(JsonConvert.SerializeObject(attachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AttachUserToChat_UserAlreadyAttached_ReturnsBadRequest()
    {
        // Arrange
        var request = "/api/chats/attach-user";
        var attachUserRequest = new AttachUserRequest
        {
            ChatId = _validChat.Id,
            UserToAddId = _validChat.CreatorId
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(attachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);
        
        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeFalse();
        responseDto.Message.Should().Be("User already attached to chat");

    }
    
    #endregion

    #region DetachUserTests
    
    [Fact]
    public async Task DetachUserFromChat_ReturnsSuccess()
    {
        // Arrange
        var request = "/api/chats/detach-user";
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = _validChat.Id,
            DetachedByUserId = Guid.NewGuid()
        };
        var content = new StringContent(JsonConvert.SerializeObject(detachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeTrue();
        responseDto.Message.Should().Be("User detached from chat successfully");
    }
    
    [Fact]
    public async Task DetachUserFromChat_InvalidChatId_ReturnsNotFound()
    {
        // Arrange
        var request = "/api/chats/detach-user";
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = Guid.Empty,
            DetachedByUserId = Guid.NewGuid()
        };
        var content = new StringContent(JsonConvert.SerializeObject(detachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task DetachUserFromChat_UserWasNotAttached_ReturnsBadRequest()
    {
        // Arrange
        var request = "/api/chats/detach-user";
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = _validChat.Id,
            UserToDetachId = Guid.Empty,
            DetachedByUserId = Guid.NewGuid()
        };
        var content = new StringContent(JsonConvert.SerializeObject(detachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeFalse();
        responseDto.Message.Should().NotBeNull();
        responseDto.Message.Should().Be("This user hadn't been attached to chat");
    }
    
    [Fact]
    public async Task DetachUserFromChat_CreatorDetachesHimself_ReturnsBadRequest()
    {
        // Arrange
        var request = "/api/chats/detach-user";
        var detachUserRequest = new DetachUserRequset
        {
            ChatId = _validChat.Id,
            UserToDetachId = _validChat.CreatorId,
            DetachedByUserId = _validChat.CreatorId
        };
        var content = new StringContent(JsonConvert.SerializeObject(detachUserRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(request, content);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseContent);
        
        responseDto.Should().NotBeNull();
        responseDto.IsSuccess.Should().BeFalse();
        responseDto.Message.Should().NotBeNull();
        responseDto.Message.Should().Be("You can't detach yourself from the chat");
    }
    
    #endregion
}