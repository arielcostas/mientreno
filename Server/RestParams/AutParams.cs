namespace Server.RestParams;

public class LoginInput
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? TwoFactorCode { get; set; }
}