namespace NovaPrinter.Models;

public class AppSettings
{
    public string? ApiUrl { get; set; }
    public string? HubName { get; set; }
    public string? Company { get; set; }
    public string? CompanyId { get; set; }
    public string? TenantId { get; set; }
    public string? PrinterName { get; set; }

    // Credenciales
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Token
    public string JwtToken { get; set; } = string.Empty;
}
