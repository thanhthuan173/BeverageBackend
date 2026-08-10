using EcommerceBackend.Application.Common.Email;
using EcommerceBackend.Application.Interfaces.Email;
using EcommerceBackend.Domain.Models;

namespace EcommerceBackend.Application.Services.Email;

public sealed class LoginNotificationService : ILoginNotificationService
{
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<LoginNotificationService> _logger;

    public LoginNotificationService(
        IEmailTemplateRenderer templateRenderer,
        IEmailSender emailSender,
        ILogger<LoginNotificationService> logger)
    {
        _templateRenderer = templateRenderer;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task NotifyLoginSuccessAsync(User user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            return;

        try
        {
            var model = new LoginSuccessEmailModel
            {
                FullName = user.FullName ?? user.Username ?? user.Email,
                Email = user.Email,
                LoginAtUtc = DateTime.UtcNow
            };

            var html = await _templateRenderer.RenderAsync(
                "login-success.mjml.hbs",
                model,
                cancellationToken);

            var message = new EmailMessage(
                To: user.Email,
                Subject: "Login successful",
                HtmlBody: html);

            await _emailSender.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send login success email for UserId={UserId}", user.Id);
            // login vẫn thành công, email fail thì chỉ log
        }
    }
}