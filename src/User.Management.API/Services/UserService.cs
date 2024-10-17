using AutoMapper;
using User.Management.API.DataAccess;
using User.Management.Domain.Models;

namespace User.Management.API.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }
    
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await unitOfWork.UserRepository.GetUsersAsync();
        
        return mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserAsync(Guid id)
    {
        var user = await unitOfWork.UserRepository.GetUserAsync(id);

        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await unitOfWork.UserRepository.GetByEmailAsync(email);
        
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        return mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByFilterAsync(string filter)
    {
        var users = await unitOfWork.UserRepository.GetUsersByFilterAsync(filter);
        
        return mapper.Map<IEnumerable<UserDto>>(users);
    }
}