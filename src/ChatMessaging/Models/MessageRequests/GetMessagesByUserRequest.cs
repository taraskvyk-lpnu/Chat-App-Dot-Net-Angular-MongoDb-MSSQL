namespace ChatMessaging.Models.MessageRequests;

public class GetMessagesByUserRequest
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
}