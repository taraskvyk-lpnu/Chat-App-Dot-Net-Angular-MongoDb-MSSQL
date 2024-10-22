using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.ResponseDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly IChatManagementService _chatManagementService;

    public ChatsController(IChatManagementService chatManagementService)
    {
        _chatManagementService = chatManagementService;
    }
    
    [HttpGet]
    [Authorize("RequireAdminRole")]
    public async Task<IActionResult> GetChats()
    {
        var chats = await _chatManagementService.GetAllChatsAsync();
        return Ok(chats);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetChatsByUserId(Guid userId)
    {
        var chat = await _chatManagementService.GetChatsByUserIdAsync(userId);
        return Ok(chat);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChat(Guid id)
    {
        var chat = await _chatManagementService.GetChatByIdAsync(id);
        return Ok(chat);
    }
    
    [HttpPost]
    public async Task<ActionResult<ResponseDto>> CreateChat([FromBody] AddChatRequest addChatRequest)
    {
        var chatDto = await _chatManagementService.AddChatAsync(addChatRequest);
        
        var response = new ResponseDto
        {
            IsSuccess = true,
            Data = chatDto,
            Message = "Chat created successfully",
        };

        return Ok(response);
    }
    
    [HttpPut]
    public async Task<ActionResult<ResponseDto>> UpdateChat([FromBody] UpdateChatRequest updateChatRequest)
    {
        var chatDto = await _chatManagementService.UpdateChatAsync(updateChatRequest);
        
        var response = new ResponseDto
        {
            Data = chatDto,
            IsSuccess = true,
            Message = "Chat updated successfully",
        };
        
        return Ok(response);
    }
    
    [HttpDelete("{chatId}")]
    public async Task<ActionResult<ResponseDto>> RemoveChat(Guid chatId, Guid userId)
    {
        var removeChatRequest = new RemoveChatRequest { ChatId = chatId, UserId = userId };
        await _chatManagementService.RemoveChatAsync(removeChatRequest);
        
        var response = new ResponseDto
        {
            Data = chatId,
            IsSuccess = true,
            Message = "Chat deleted successfully",
        };
        
        return Ok(response);
    }

    
    [HttpPost("attach-user")]
    public async Task<ActionResult<ResponseDto>> AttachUserToChat([FromBody] AttachUserRequest attachUserRequest)
    {
        var chatDto = await _chatManagementService.AttachUserToChatAsync(attachUserRequest);
        
        var response = new ResponseDto
        {
            Data = chatDto,
            IsSuccess = true,
            Message = "User attached to chat successfully",
        };
        
        return Ok(response);
    }
    
    [HttpPost("detach-user")]
    public async Task<ActionResult<ResponseDto>> DetachUserFromChat([FromBody] DetachUserRequest detachUserRequest)
    {
        var chatDto = await _chatManagementService.DetachUserFromChatAsync(detachUserRequest);
        
        var response = new ResponseDto
        {
            Data = chatDto,
            IsSuccess = true,
            Message = "User detached from chat successfully",
        };
        
        return Ok(response);
    }
}   