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

        public AuthService(JwtTokenGenerator jwtTokenGenerator,
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
            var user = await _userManager.FindByNameAsync(loginRequestDto.UserName);
            
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

        public async Task<ResponseDto> Register(RegistrationRequestDto registrationRequestDto)
        {
            var user = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

            if (result.Succeeded)
            {
                var userToReturn = await _userManager.FindByNameAsync(registrationRequestDto.Email);
                var token = await GenerateToken(userToReturn!);

                var userDto = new UserDto
                {
                    ID = userToReturn!.Id,
                    Name = userToReturn.Name,
                    Email = userToReturn.Email,
                    PhoneNumber = userToReturn.PhoneNumber
                };

                return new ResponseDto
                {
                    IsSuccess = true,
                    User = userDto, 
                    Token = token
                };
            }

            return new ResponseDto
            {
                Token = "",
                Message = result.Errors.FirstOrDefault()?.Description ?? "Unknown error occurred"
            };
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return _jwtTokenGenerator.GenerateToken(user, roles);
        }
    }
}