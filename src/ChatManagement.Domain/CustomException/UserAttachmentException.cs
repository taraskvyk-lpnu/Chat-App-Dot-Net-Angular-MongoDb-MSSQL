namespace ChatManagement.Infrastructure.CustomException;

public class UserAttachmentException : ApiException
{
    public UserAttachmentException(string message) : base(message)
    {
    }
}