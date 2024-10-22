using User.Management.Domain.Models;

namespace User.Management.API.DataAccess.Repository;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser?>> GetUsersAsync();
    Task<IEnumerable<ApplicationUser?>> GetUsersAsync(List<Guid> ids);
    Task<ApplicationUser?> GetUserAsync(Guid id);
    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetUsersByFilterAsync(string filter);
}