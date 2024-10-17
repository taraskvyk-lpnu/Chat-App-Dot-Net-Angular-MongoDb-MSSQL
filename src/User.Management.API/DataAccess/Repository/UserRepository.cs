using Microsoft.EntityFrameworkCore;
using User.Management.Domain.Models;

namespace User.Management.API.DataAccess.Repository;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<ApplicationUser?>> GetUsersAsync()
    {
        return await _context.AspNetUsers.ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserAsync(Guid id)
    {
        return await _context.AspNetUsers.FindAsync(id.ToString());
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email)
    {
        return await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersByFilterAsync(string filter)
    {
        return await _context.AspNetUsers
            .Where(u => 
                u.Name.Contains(filter) ||
                u.Email.Contains(filter) || 
                u.UserName.Contains(filter)
            ).ToListAsync();
    }
}