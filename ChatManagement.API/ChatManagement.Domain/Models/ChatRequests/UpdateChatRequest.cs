namespace ChatManagement.Domain.Models.ChatRequests;

public class UpdateChatRequest
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public ICollection<Guid>? UserIds { get; set; }
}