namespace ChatMessaging.Models.MessageRequests;

public class UpdateMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public Message Message { get; set; }
}