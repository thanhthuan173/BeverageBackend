using EcommerceBackend.Application.Common.Email;

namespace EcommerceBackend.Application.Interfaces.Email;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<TModel>(
        string templateFileName,
        TModel model,
        CancellationToken cancellationToken = default);
}