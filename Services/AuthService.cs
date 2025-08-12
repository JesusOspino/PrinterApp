using NovaPrinter.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace NovaPrinter.Services;

public static class AuthService
{
   
    public static async Task<AuthResult?> LoginAsync(string apiBaseUrl, string username, string password)
    {
        using var http = new HttpClient { BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/") };
        var body = new { username, password };

        var resp = await http.PostAsJsonAsync("auth/login", body);
        if (!resp.IsSuccessStatusCode) return null;

        var json = await resp.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var token = root.GetProperty("accessToken").GetString() ?? "";
        var user = root.GetProperty("user").GetString() ?? "";
        
        return new AuthResult
        {
            AccessToken = token,
            User = user
        };
    }
}
