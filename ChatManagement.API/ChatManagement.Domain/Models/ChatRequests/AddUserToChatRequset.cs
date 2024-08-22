namespace ChatManagement.Domain.Models.ChatRequests;

public class AddUserToChatRequset
{
    public Guid ChatId { get; set; }
    public Guid UserToAddId { get; set; }
    public Guid AddedByUserId { get; set; }
    public Guid AddedByUserName { get; set; }
}