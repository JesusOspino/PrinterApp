
namespace NovaPrinter.Models;

public class AuthResult
{
    public string AccessToken { get; set; } = null!;
    public dynamic? User { get; set; }
    public dynamic? Menu { get; set; }
}
