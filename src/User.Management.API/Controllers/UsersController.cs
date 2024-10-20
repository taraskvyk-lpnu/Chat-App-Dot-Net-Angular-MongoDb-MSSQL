using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Management.API.Services;
using User.Management.Domain.Models;

namespace User.Management.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController
{
    private readonly IUserService userService;

    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }
    
    [HttpGet]
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        return await userService.GetUsersAsync();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<UserDto> GetUserAsync(Guid id)
    {
        return await userService.GetUserAsync(id);
    }
    
    [HttpGet("{filter}")]
    public async Task<IEnumerable<UserDto>> GetUsersByFilterAsync([FromRoute] string filter)
    {
        return await userService.GetUsersByFilterAsync(filter);
    }
}