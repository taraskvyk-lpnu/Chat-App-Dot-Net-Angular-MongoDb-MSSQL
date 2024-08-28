using Chat.Management.API.IntegrationTests.WebFactory;
using ChatManagement.API;
using ChatDomain = ChatManagement.Domain.Models.Chat;

namespace Chat.Management.API.IntegrationTests;

public class ChatsControllerIntegrationTests : IClassFixture<CustomWebFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ChatDomain _validChat;

    public ChatsControllerIntegrationTests(CustomWebFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _validChat = factory.GetValidChat();
    }
    
    
}