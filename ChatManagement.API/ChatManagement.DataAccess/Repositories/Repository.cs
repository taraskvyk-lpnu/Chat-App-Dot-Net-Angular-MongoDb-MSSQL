using ChatManagement.DataAccess;
using ChatManagement.Domain.Repositories;
using ChatManagement.Infrastructure.CustomException;
using Microsoft.EntityFrameworkCore;

namespace ChatManagement.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ChatManagementDbContext _chatContext;
    internal readonly DbSet<TEntity> _dbSet;

    public Repository(ChatManagementDbContext chatContext)
    {
        _chatContext = chatContext;
        _dbSet = _chatContext.Set<TEntity>();
    }
    
    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        int n = _dbSet.Count();
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task RemoveByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity != null)
        {
            throw new NotFoundException("Entity not found");
        }
        
        _dbSet.Remove(entity);
    }
}