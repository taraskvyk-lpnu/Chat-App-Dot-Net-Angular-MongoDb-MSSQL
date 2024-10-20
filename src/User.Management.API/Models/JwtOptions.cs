namespace User.Management.API.Models;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string Secret { get; set; } = string.Empty;
    public string ExpirationHours { get; set; } = string.Empty;
}