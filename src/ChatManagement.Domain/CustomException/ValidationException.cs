namespace ChatManagement.Infrastructure.CustomException;

public class ValidationException : ApiException
{
    public ValidationException(string message) : base(message, 400) { }
}