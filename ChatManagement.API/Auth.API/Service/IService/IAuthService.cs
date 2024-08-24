using Auth.API.Models.Dto;

namespace Auth.API.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto> Login(LoginRequest loginRequest);
        Task<bool> AssignRole(string email, string roleName);
    }
}
