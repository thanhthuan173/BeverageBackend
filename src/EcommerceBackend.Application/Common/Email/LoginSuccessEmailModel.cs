namespace EcommerceBackend.Application.Common.Email;

public sealed class LoginSuccessEmailModel
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime LoginAtUtc { get; init; }

    public string LoginAtDisplay => LoginAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
}