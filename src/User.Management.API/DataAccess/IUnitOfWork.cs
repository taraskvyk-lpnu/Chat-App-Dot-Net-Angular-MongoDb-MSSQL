using User.Management.API.DataAccess.Repository;

namespace User.Management.API.DataAccess;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    Task<int> SaveChangesAsync();
}