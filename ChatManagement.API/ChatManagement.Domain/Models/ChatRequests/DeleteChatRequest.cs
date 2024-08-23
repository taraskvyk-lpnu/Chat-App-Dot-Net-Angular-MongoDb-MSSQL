namespace ChatManagement.Domain.Models.ChatRequests;

public class RemoveChatRequest
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
}