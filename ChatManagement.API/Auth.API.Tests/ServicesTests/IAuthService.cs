namespace Auth.API.Tests.ServicesTests
{
    /// <summary>
    /// Interface for testing the authentication service, including role assignment, user login, and registration.
    /// </summary>
    public interface IAuthServiceTest
    {
        /// <summary>
        /// Tests the AssignRole method to verify behavior when the user is not found.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        /// <example>
        /// Example test cases:
        /// - Email does not exist in the system: Should return false.
        /// </example>
        [Fact]
        Task AssignRole_ShouldReturnFalse_WhenUserNotFound();

        /// <summary>
        /// Tests the AssignRole method to verify successful role assignment.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        /// <example>
        /// Example test cases:
        /// - User exists and role is successfully assigned: Should return true.
        /// </example>
        [Fact]
        Task AssignRole_ShouldReturnTrue_WhenRoleAssignedSuccessfully();

        /// <summary>
        /// Tests the Login method to verify behavior when the user is not found.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        /// <example>
        /// Example test cases:
        /// - Email does not exist in the system: Should return a ResponseDto with IsSuccess = false and appropriate message.
        /// </example>
        [Fact]
        Task Login_ShouldReturnErrorResponse_WhenUserNotFound();

        /// <summary>
        /// Tests the Login method to verify successful login and token generation.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        /// <example>
        /// Example test cases:
        /// - Valid email and password: Should return a ResponseDto with IsSuccess = true and a valid JWT token.
        /// </example>
        [Fact]
        Task Login_ShouldReturnToken_WhenCredentialsAreValid();

        /// <summary>
        /// Tests the Register method to verify successful user registration.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        /// <example>
        /// Example test cases:
        /// - Valid registration details: Should create a user, generate a token, and return a success response.
        /// </example>
        [Fact]
        Task Register_ShouldReturnSuccessResponse_WhenRegistrationIsSuccessful();

        /// <summary>
        /// Tests the Register method to verify behavior when registration fails.
        /// </summary>
        /// <returns>A task that represents the asynchronous test operation.</returns>
        /// <example>
        /// Example test cases:
        /// - Registration fails due to some error (e.g., email already exists, weak password): Should return a ResponseDto with IsSuccess = false and error message.
        /// </example>
        [Fact]
        Task Register_ShouldReturnFailureResponse_WhenRegistrationFails();
    }
}
