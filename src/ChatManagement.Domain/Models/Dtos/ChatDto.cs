namespace ChatManagement.Domain.Models.Dtos;

public class ChatDto
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> UserIds { get; set; }
}