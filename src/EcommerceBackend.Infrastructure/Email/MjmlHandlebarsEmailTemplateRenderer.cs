using System.Diagnostics;
using EcommerceBackend.Application.Interfaces.Email;
using HandlebarsDotNet;

namespace EcommerceBackend.Infrastructure.Email;

public sealed class MjmlHandlebarsEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly IWebHostEnvironment _env;

    public MjmlHandlebarsEmailTemplateRenderer(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> RenderAsync<TModel>(
        string templateFileName,
        TModel model,
        CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "EcommerceBackend.API/EmailTemplates", templateFileName);

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Email template not found: {templatePath}");

        var templateSource = await File.ReadAllTextAsync(templatePath, cancellationToken);

        var compiledHandlebars = Handlebars.Compile(templateSource);
        var mjmlContent = compiledHandlebars(model);

        return await CompileMjmlToHtmlAsync(mjmlContent, cancellationToken);
    }

    private static async Task<string> CompileMjmlToHtmlAsync(
        string mjmlContent,
        CancellationToken cancellationToken)
    {
        var tempMjmlPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.mjml");
        await File.WriteAllTextAsync(tempMjmlPath, mjmlContent, cancellationToken);

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "npx",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("mjml");
            startInfo.ArgumentList.Add(tempMjmlPath);
            startInfo.ArgumentList.Add("-s");
    
            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start MJML process.");

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(cancellationToken);
            await Task.WhenAll(stdoutTask, stderrTask);

            var html = await stdoutTask;
            var stderr = await stderrTask;

            if (process.ExitCode != 0)
                throw new InvalidOperationException($"MJML compile failed: {stderr}");

            return html;
        }
        finally
        {
            if (File.Exists(tempMjmlPath))
                File.Delete(tempMjmlPath);
        }
    }
}