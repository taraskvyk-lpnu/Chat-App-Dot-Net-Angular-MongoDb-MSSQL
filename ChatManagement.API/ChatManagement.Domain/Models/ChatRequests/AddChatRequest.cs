namespace ChatManagement.Domain.Models.ChatRequests;

public class AddChatRequest
{
    public Guid CreatorId { get; set; }
    public string CreatorName { get; set; }
    public string Title { get; set; }
    public List<Guid>? UserIds { get; set; }
}