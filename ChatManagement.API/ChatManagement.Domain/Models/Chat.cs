namespace ChatManagement.Domain.Models;

public class Chat
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ICollection<Guid> UserIds { get; set; }
}