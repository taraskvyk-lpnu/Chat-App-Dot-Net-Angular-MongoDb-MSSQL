using Auth.API.Models.Dto;
using Auth.API.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            var registerResponse = await _authService.RegisterAsync(model);

            if (!registerResponse.IsSuccess)
            {
                return BadRequest(registerResponse);
            }
            
            return Ok(registerResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var loginResponse = await _authService.LoginAsync(model);
           
            if (!loginResponse.IsSuccess)
            {
                return BadRequest(loginResponse);
            }
            
            return Ok(loginResponse);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest roleRequest)
        {
            var assignRoleSuccessful = await _authService.AssignRoleAsync(roleRequest.Email, roleRequest.Role);
            var response = new ResponseDto();
            
            if (!assignRoleSuccessful)
            {
                response.IsSuccess = false;
                response.Message = "Error encountered";
                return BadRequest(response);
            }
            
            return Ok(response);
        }
    }
}