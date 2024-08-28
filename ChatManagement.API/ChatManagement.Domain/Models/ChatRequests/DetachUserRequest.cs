namespace ChatManagement.Domain.Models.ChatRequests;

public class DetachUserRequest
{
    public Guid ChatId { get; set; }
    public Guid UserToDetachId { get; set; }
    public Guid DetachedByUserId { get; set; }
}