namespace ChatManagement.Domain.Models;

public class Chat
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> UserIds { get; set; }
}