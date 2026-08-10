using EcommerceBackend.Application.Common.Email;
using EcommerceBackend.Application.Interfaces.Email;

namespace EcommerceBackend.Infrastructure.Email;

public class AzureEmailSender : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}