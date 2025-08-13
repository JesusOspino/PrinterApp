using NovaPrinter.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace NovaPrinter.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    public AuthService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<bool> LoginAsync()
    {
        var url = Settings.ApiUrl;

        var payload = new
        {
            Username = Settings.Username,
            Password = Settings.Password,
        };

        if (!string.IsNullOrWhiteSpace(url) || !string.IsNullOrWhiteSpace(payload.Username) || !string.IsNullOrWhiteSpace(payload.Password))
        {
            MessageBox.Show("Error, debe llenar todos los campos de inicio de sesión", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (result?.AccessToken == null)
        {
            return false;
        }

        Settings.JwtToken = result.AccessToken;
        SettingsService.Save();

        return true;
    }

}
