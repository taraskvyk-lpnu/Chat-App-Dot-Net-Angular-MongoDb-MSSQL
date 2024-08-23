using Auth.API.Models;
using Auth.API.Models.Dto;
using Auth.API.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user == null)
                return false;

            if (string.IsNullOrEmpty(roleName))
                roleName = "User";
            
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                
                if (!roleResult.Succeeded)
                    return false;
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            return addToRoleResult.Succeeded;
        }
        
        public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
            
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDto.Password))
            {
                return new ResponseDto
                {
                    IsSuccess = false, 
                    Message = "Username or password is incorrect"
                };
            }

            var token = await GenerateToken(user);

            var userDto = new UserDto
            {
                ID = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return new ResponseDto
            {
                User = userDto, 
                Token = token
            };
        }

        public async Task<ResponseDto> Register(RegistrationRequestDto registerRequestDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                NormalizedEmail = registerRequestDto.Email.ToUpper(),
                Name = registerRequestDto.Name,
                PhoneNumber = registerRequestDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);

            if (result.Succeeded)
            {
                var userToReturn = await _userManager.FindByNameAsync(registerRequestDto.Email);
                var token = await GenerateToken(userToReturn!);

                var userDto = new UserDto
                {
                    ID = userToReturn!.Id,
                    Name = userToReturn.Name,
                    Email = userToReturn.Email,
                    PhoneNumber = userToReturn.PhoneNumber
                };

                await AssignRole(registerRequestDto.Email, registerRequestDto.Role ?? "User");
                
                return new ResponseDto
                {
                    IsSuccess = true,
                    User = userDto, 
                    Token = token
                };
            }

            var errorMessages = string.Join(";", result.Errors.Select(e => e.Description));

            return new ResponseDto
            {
                IsSuccess = false,
                Token = "",
                Message = errorMessages
            };
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return _jwtTokenGenerator.GenerateToken(user, roles);
        }
    }
}