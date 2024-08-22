using ChatManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }
}