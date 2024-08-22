namespace ChatManagement.Domain.Models.Dtos;

public class ChatDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ICollection<Guid> UserIds { get; set; }
}