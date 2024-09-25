using ChatManagement.Domain;
using ChatManagement.Domain.Repositories;
using ChatManagement.Infrastructure.Repositories;

namespace ChatManagement.DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly ChatManagementDbContext _chatContext;
    private IChatRepository _chatRepository;

    public UnitOfWork(ChatManagementDbContext chatContext)
    {
        this._chatContext = chatContext;
    }

    public IChatRepository Chat => _chatRepository = _chatRepository ?? new ChatRepository(_chatContext);

    public async Task<int> CommitAsync()
    {
        return await _chatContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _chatContext.Dispose();
    }
}