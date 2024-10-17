using User.Management.Domain.Models;

namespace User.Management.API.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserAsync(Guid id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetUsersByFilterAsync(string filter);
}