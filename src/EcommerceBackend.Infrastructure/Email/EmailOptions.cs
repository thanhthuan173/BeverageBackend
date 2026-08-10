namespace EcommerceBackend.Infrastructure.Email;

public sealed class EmailOptions
{
    public string Provider { get; set; } = "Preview"; // Preview | Azure
    public string PreviewOutputFolder { get; set; } = "email-previews";
    
    public AzureEmailOptions Azure { get; set; } = new();
}

public sealed class AzureEmailOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderName { get; set; } = "EcommerceBackend";
}