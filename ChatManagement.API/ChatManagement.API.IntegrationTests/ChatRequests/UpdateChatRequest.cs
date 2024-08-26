namespace ChatManagement.Domain.Models.ChatRequests;

public class UpdateChatRequest
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public List<Guid>? UserIds { get; set; }
}