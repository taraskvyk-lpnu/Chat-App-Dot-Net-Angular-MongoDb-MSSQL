namespace ChatMessaging.Models.MessageRequests;

public class DeleteMessageRequest
{
    public Guid ChatId { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}