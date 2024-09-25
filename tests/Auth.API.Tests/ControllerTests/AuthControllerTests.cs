using Auth.API.Controllers;
using Auth.API.Models.Dto;
using Auth.API.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Auth.API.Tests.ControllerTests;

public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto();
            var registerResponse = new ResponseDto { IsSuccess = true, Token = "valid-token" };

            _authServiceMock.Setup(auth => auth.RegisterAsync(registerRequest))
                .ReturnsAsync(registerResponse);

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(registerResponse.Token, ((ResponseDto)okResult.Value!).Token);
            Assert.Equal(registerResponse, okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto();
            var registerResponse = new ResponseDto { IsSuccess = false };

            _authServiceMock.Setup(auth => auth.RegisterAsync(registerRequest))
                .ReturnsAsync(registerResponse);

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(registerResponse, badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            var loginResponse = new ResponseDto { IsSuccess = true, Token = "valid-token" };

            _authServiceMock.Setup(auth => auth.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _authController.Login(loginRequest);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(loginResponse.Token, ((ResponseDto)okResult.Value!).Token);
            Assert.Equal(loginResponse, okResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            var loginResponse = new ResponseDto { IsSuccess = false };

            _authServiceMock.Setup(auth => auth.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(loginResponse, badRequestResult.Value);
        }

        [Fact]
        public async Task AssignRole_ShouldReturnOk_WhenRoleAssignedSuccessfully()
        {
            // Arrange
            var assignRoleRequest = new AssignRoleRequest { Email = "test@example.com", Role = "Admin" };
            var response = new ResponseDto { IsSuccess = true };

            _authServiceMock.Setup(auth => auth.AssignRoleAsync(assignRoleRequest.Email, assignRoleRequest.Role))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.AssignRole(assignRoleRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response.IsSuccess, ((ResponseDto)okResult.Value!).IsSuccess);
        }

        [Fact]
        public async Task AssignRole_ShouldReturnBadRequest_WhenRoleAssignmentFails()
        {
            // Arrange
            var assignRoleRequest = new AssignRoleRequest { Email = "test@example.com", Role = "Admin" };
            var response = new ResponseDto { IsSuccess = false, Message = "Error encountered" };

            _authServiceMock.Setup(auth => auth.AssignRoleAsync(assignRoleRequest.Email, assignRoleRequest.Role))
                .ReturnsAsync(false);

            // Act
            var result = await _authController.AssignRole(assignRoleRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response.IsSuccess, ((ResponseDto)badRequestResult.Value!).IsSuccess);
            Assert.Equal(response.Message, ((ResponseDto)badRequestResult.Value!).Message);
        }
    }