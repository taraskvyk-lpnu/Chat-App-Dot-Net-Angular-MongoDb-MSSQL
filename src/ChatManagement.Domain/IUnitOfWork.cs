using ChatManagement.Domain.Repositories;

namespace ChatManagement.Domain;

public interface IUnitOfWork : IDisposable
{
    IChatRepository Chat { get; }
    Task<int> CommitAsync();
}