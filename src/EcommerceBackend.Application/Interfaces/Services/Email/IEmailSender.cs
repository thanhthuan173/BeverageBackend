using EcommerceBackend.Application.Common.Email;

namespace EcommerceBackend.Application.Interfaces.Email;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}