using EcommerceBackend.Domain.Models;

namespace EcommerceBackend.Application.Interfaces.Email;

public interface ILoginNotificationService
{
    Task NotifyLoginSuccessAsync(User user, CancellationToken cancellationToken = default);
}