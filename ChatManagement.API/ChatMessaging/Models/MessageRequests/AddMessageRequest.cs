namespace ChatMessaging.Models.MessageRequests;

public class AddMessageRequest
{
    public Guid ChatId { get; set; }
    public Message Message { get; set; }
}
