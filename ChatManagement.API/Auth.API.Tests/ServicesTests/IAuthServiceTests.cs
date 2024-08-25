using System.Threading.Tasks;

namespace Auth.API.Tests.ServicesTests
{
    /// <summary>
    /// Interface for testing the authentication service.
    /// </summary>
    public interface IAuthServiceTest
    {
        /// <summary>
        /// Tests the AssignRole method when the user is not found.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AssignRole_ShouldReturnFalse_WhenUserNotFound();

        /// <summary>
        /// Tests the AssignRole method when the role does not exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AssignRole_ShouldThrowInvalidOperationException_WhenRoleNotExists();

        /// <summary>
        /// Tests the AssignRole method when the user already has the role.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AssignRole_ShouldThrowInvalidOperationException_WhenUserAlreadyHasRole();

        /// <summary>
        /// Tests the AssignRole method when the role is successfully assigned.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AssignRole_ShouldReturnTrue_WhenRoleAssignedSuccessfully();

        /// <summary>
        /// Tests the Login method when the user is not found.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Login_ShouldReturnErrorResponse_WhenUserNotFound();

        /// <summary>
        /// Tests the Login method when the credentials are valid.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Login_ShouldReturnToken_WhenCredentialsAreValid();

        /// <summary>
        /// Tests the Register method when registration is successful.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Register_ShouldReturnSuccessResponse_WhenRegistrationIsSuccessful();

        /// <summary>
        /// Tests the Register method when registration fails.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Register_ShouldReturnFailureResponse_WhenRegistrationFails();
    }
}