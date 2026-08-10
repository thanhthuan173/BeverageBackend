using System.Text.RegularExpressions;
using EcommerceBackend.Application.Common.Email;
using EcommerceBackend.Application.Interfaces.Email;
using Microsoft.Extensions.Options;

namespace EcommerceBackend.Infrastructure.Email;

public sealed class FilePreviewEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<FilePreviewEmailSender> _logger;

    public FilePreviewEmailSender(
        IOptions<EmailOptions> options,
        IWebHostEnvironment env,
        ILogger<FilePreviewEmailSender> logger)
    {
        _options = options.Value;
        _env = env;
        _logger = logger;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var outputFolder = Path.Combine(_env.ContentRootPath, _options.PreviewOutputFolder);
        Directory.CreateDirectory(outputFolder);

        var safeSubject = Regex.Replace(message.Subject, @"[^\w\-]+", "_");
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{safeSubject}.html";
        var filePath = Path.Combine(outputFolder, fileName);

        await File.WriteAllTextAsync(filePath, message.HtmlBody, cancellationToken);

        _logger.LogInformation("Email preview saved to {FilePath}", filePath);
    }
}