namespace ChatManagement.Domain.Models.ChatRequests;

public class AttachUserRequest
{
    public Guid ChatId { get; set; }
    public Guid UserToAddId { get; set; }
    public Guid AttachedByUserId { get; set; }
}