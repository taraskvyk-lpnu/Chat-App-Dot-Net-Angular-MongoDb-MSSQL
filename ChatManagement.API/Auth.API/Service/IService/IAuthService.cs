using Auth.API.Models.Dto;

namespace Auth.API.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegisterRequestDto registrationRequestDto);
        Task<ResponseDto> LoginAsync(LoginRequest loginRequest);
        Task<bool> AssignRoleAsync(string email, string roleName);
    }
}
