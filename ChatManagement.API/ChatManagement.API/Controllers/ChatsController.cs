using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;
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
        await _chatManagementService.AddChatAsync(addChatRequest);
        
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Chat created successfully",
        };
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDto>> UpdateChat(Guid id, [FromBody] UpdateChatRequest updateChatRequest)
    {
        await _chatManagementService.UpdateChatAsync(updateChatRequest);
        
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Chat updated successfully",
        };
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDto>> RemoveChat(RemoveChatRequest removeChatRequest)
    {
        await _chatManagementService.RemoveChatAsync(removeChatRequest);
        
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Chat deleted successfully",
        };
    }
    
    [HttpPost("attach-user")]
    public async Task<ActionResult<ResponseDto>> AttachUserToChat([FromBody] AttachUserRequest attachUserRequest)
    {
        await _chatManagementService.AttachUserToChatAsync(attachUserRequest);
        
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "User attached to chat successfully",
        };
    }
    
    [HttpPost("detach-user")]
    public async Task<ActionResult<ResponseDto>> DetachUserFromChat([FromBody] DetachUserRequest detachUserRequest)
    {
        await _chatManagementService.DetachUserFromChatAsync(detachUserRequest);
        
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "User detached from chat successfully",
        };
    }
}