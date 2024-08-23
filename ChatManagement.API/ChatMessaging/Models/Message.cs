namespace ChatMessaging.Models;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public string UserName { get; set; }
    public string Text { get; set; }
    public string CreatedAt { get; set; }
}