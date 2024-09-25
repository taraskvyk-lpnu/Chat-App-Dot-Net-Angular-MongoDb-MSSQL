namespace Auth.API.Models.Dto;

public class AssignRoleRequest
{
    public string Email { get; set; }
    public string Role { get; set; }
}