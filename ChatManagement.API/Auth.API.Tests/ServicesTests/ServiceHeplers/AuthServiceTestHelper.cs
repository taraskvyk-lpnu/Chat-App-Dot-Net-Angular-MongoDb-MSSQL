using System.Collections.Generic;
using Auth.API.Models;
using Auth.API.Models.Dto;
using Auth.API.Service.IService;
using Microsoft.AspNetCore.Identity;
using Moq;

public class AuthServiceTestHelper
{
    public Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        return new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object, 
            null, null, null, null, null, null, null, null);
    }

    public Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
    {
        return new Mock<RoleManager<IdentityRole>>(
            new Mock<IRoleStore<IdentityRole>>().Object, 
            null, null, null, null);
    }

    public Mock<IJwtTokenGenerator> CreateJwtTokenGeneratorMock()
    {
        return new Mock<IJwtTokenGenerator>();
    }

    public ApplicationUser CreateApplicationUser(string email)
    {
        return new ApplicationUser { Email = email };
    }

    public LoginRequest CreateLoginRequest(string email, string password)
    {
        return new LoginRequest { Email = email, Password = password };
    }

    public RegisterRequestDto CreateRegistrationRequest(string email, string password, string name, string role = "User")
    {
        return new RegisterRequestDto
        {
            Email = email,
            Password = password,
            Name = name,
            Role = role
        };
    }
}