using Microsoft.AspNetCore.SignalR.Client;
using NovaPrinter.Models;

namespace NovaPrinter.Services;

/// <summary>
/// Servicio para manejar la conexión SignalR.
/// Soporta JWT y eventos tipados.
/// </summary>
public class SignalRService
{
    private HubConnection? _connection;
    private Func<Task<string?>>? _getAccessTokenAsync;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    /// <summary>
    /// Evento que se dispara cuando se recibe una solicitud de impresión.
    /// </summary>
    public event Action<string, object?>? OnPrintRequest;

    /// <summary>
    /// Configura la función que provee el token JWT actual.
    /// </summary>
    public void ConfigureTokenProvider(Func<Task<string?>> tokenProvider)
    {
        _getAccessTokenAsync = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    /// <summary>
    /// Conecta al hub de SignalR usando la configuración de la aplicación.
    /// </summary>
    public async Task ConnectAsync(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ApiUrl))
            throw new InvalidOperationException("ApiUrl no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(settings.HubName))
            throw new InvalidOperationException("HubName no puede estar vacío.");
        if (_getAccessTokenAsync == null)
            throw new InvalidOperationException("TokenProvider no configurado. Llama a ConfigureTokenProvider antes de conectar.");

        var hubUrl = CombineUrl(settings.ApiUrl, settings.HubName);

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = _getAccessTokenAsync;
            })
            .WithAutomaticReconnect()
            .Build();

        // Registro del método que el servidor invoca
        _connection.On<string, object?>("Print", (messageType, payload) =>
        {
            OnPrintRequest?.Invoke(messageType, payload);
        });

        // Manejo de eventos de conexión
        _connection.Reconnecting += error =>
        {
            Console.WriteLine($"Reconectando al hub... Error: {error?.Message}");
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Console.WriteLine($"Reconectado al hub. ConnectionId: {connectionId}");
            return Task.CompletedTask;
        };

        _connection.Closed += async error =>
        {
            Console.WriteLine($"Conexión cerrada: {error?.Message}. Reintentando en 5 segundos...");
            await Task.Delay(5000);
            try
            {
                await ConnectAsync(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al reconectar: {ex.Message}");
            }
        };

        await _connection.StartAsync();
        Console.WriteLine("Conectado al hub.");
    }

    /// <summary>
    /// Desconecta del hub.
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
            Console.WriteLine("Desconectado del hub.");
        }
    }

    #region Metodos privados

    /// <summary>
    /// Combina la Url y el Hub para devolver la ruta final de conexión
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="hub"></param>
    /// <returns></returns>
    private string CombineUrl(string baseUrl, string hub)
    {
        // aseguro formato correcto
        var trimmed = baseUrl.TrimEnd('/');
        var h = hub.TrimStart('/');
        return $"{trimmed}/{h}";
    }

    #endregion Metodos privados
}
