using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.API.Models;
using Auth.API.Models.Dto;
using Auth.API.Service;
using Auth.API.Service.IService;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Auth.API.Tests.ServicesTests
{
    public class AuthServiceTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly IAuthService _authService;
        private readonly AuthServiceTestHelper _authHelper;

        public AuthServiceTest()
        {
            _authHelper = new AuthServiceTestHelper();
            _userManagerMock = _authHelper.CreateUserManagerMock();
            _roleManagerMock = _authHelper.CreateRoleManagerMock();
            _jwtTokenGeneratorMock = _authHelper.CreateJwtTokenGeneratorMock();

            _authService = new AuthService(_jwtTokenGeneratorMock.Object, 
                                           _userManagerMock.Object, 
                                           _roleManagerMock.Object);
        }

        [Fact]
        public async Task AssignRole_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _userManagerMock.Setup(m => 
                    m.FindByEmailAsync(email))
                            .ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _authService.AssignRoleAsync(email, "Admin");
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AssignRole_ShouldThrowInvalidOperationException_WhenRoleNotExists()
        {
            // Arrange
            var email = "user@example.com";
            var user = _authHelper.CreateApplicationUser(email);

            _userManagerMock.Setup(m => 
                    m.FindByEmailAsync(email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _authService.AssignRoleAsync(email, string.Empty));
        }

        [Fact]
        public async Task AssignRole_ShouldThrowInvalidOperationException_WhenUserAlreadyHasRole()
        {
            // Arrange
            var email = "user@example.com";
            var roleName = "Admin";
            var user = _authHelper.CreateApplicationUser(email);

            _userManagerMock.Setup(um => 
                    um.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => 
                    um.IsInRoleAsync(user, roleName))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _authService.AssignRoleAsync(email, roleName));
        }

        [Fact]
        public async Task AssignRole_ShouldReturnTrue_WhenRoleAssignedSuccessfully()
        {
            // Arrange
            var email = "user@example.com";
            var roleName = "Admin";
            var user = _authHelper.CreateApplicationUser(email);

            _userManagerMock.Setup(um => 
                    um.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _roleManagerMock.Setup(rm => 
                    rm.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            _userManagerMock.Setup(um => 
                    um.AddToRoleAsync(user, roleName))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.AssignRoleAsync(email, roleName);
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Login_ShouldReturnErrorResponse_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = _authHelper.CreateLoginRequest("nonexistent@example.com", "password");
            _userManagerMock.Setup(um => 
                    um.FindByEmailAsync(loginRequest.Email))
                .ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _authService.LoginAsync(loginRequest);
            
            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = _authHelper.CreateLoginRequest("user@example.com", "1#password");
            var user = _authHelper.CreateApplicationUser(loginRequest.Email);
            var token = "generated-jwt-token";

            _userManagerMock.Setup(um => 
                    um.FindByEmailAsync(loginRequest.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => 
                    um.CheckPasswordAsync(user, loginRequest.Password))
                .ReturnsAsync(true);

            _jwtTokenGeneratorMock.Setup(jt => 
                    jt.GenerateToken(user, It.IsAny<IList<string>>()))
                .Returns(token);

            // Act
            var result = await _authService.LoginAsync(loginRequest);
            
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(result.Token, token);
        }

        [Fact]
        public async Task Register_ShouldReturnSuccessResponse_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerRequest = _authHelper.CreateRegistrationRequest("user@example.com", "1#password", "John Doe", "Admin");
            var user = _authHelper.CreateApplicationUser(registerRequest.Email);
            var token = "generated-jwt-token";

            _userManagerMock.Setup(um => 
                    um.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => 
                    um.FindByEmailAsync(registerRequest.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(rm => 
                    rm.AddToRoleAsync(user, registerRequest.Role))
                .ReturnsAsync(IdentityResult.Success);

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(registerRequest.Role))
                .ReturnsAsync(true);

            _jwtTokenGeneratorMock.Setup(jt => 
                    jt.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
                .Returns(token);

            // Act
            var result = await _authService.RegisterAsync(registerRequest);
            
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(result.Token, token);
        }

        [Fact]
        public async Task Register_ShouldReturnFailureResponse_WhenRegistrationFails()
        {
            // Arrange
            var registerRequest = _authHelper.CreateRegistrationRequest("user@example.com", "", "John Doe", "User");

            _userManagerMock.Setup(um => 
                    um.CreateAsync(It.IsAny<ApplicationUser>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Empty(result.Token!);
        }
    }
}