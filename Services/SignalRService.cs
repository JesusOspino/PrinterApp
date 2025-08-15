using Microsoft.AspNetCore.SignalR.Client;
using NovaPrinter.Models;

namespace NovaPrinter.Services;

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting,
    Disconnecting
}

/// <summary>
/// Servicio para manejar la conexión SignalR.
/// Soporta JWT y eventos tipados.
/// </summary>
public class SignalRService
{
    private HubConnection? _connection;

    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    private Timer _tokenRenewalTimer = null!;

    //public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    private ConnectionState _currentState = ConnectionState.Disconnected;

    public ConnectionState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnConnectionStateChanged?.Invoke(value);
            }
        }
    }

    // Evento para notificar cambios de estado
    public event Action<ConnectionState> OnConnectionStateChanged = null!;

    // Evento que se dispara cuando se recibe una solicitud de impresión.
    public event Action<object> OnPrintRequest = null!;

    /// <summary>
    /// Conecta al hub de SignalR usando la configuración de la aplicación.
    /// </summary>
    public async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(Settings.UrlHub))
            throw new InvalidOperationException("La Url del Hub no puede estar vacía.");

        if (Settings.JwtToken == null)
            throw new InvalidOperationException("Token no configurado. verifica las credenciales antes de conectar.");

        _connection = new HubConnectionBuilder()
            .WithUrl(Settings.UrlHub, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(Settings.JwtToken)!;
            })
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5) })
            .Build();

        // Registro del método que el servidor invoca
        _connection.On<object>("GetHutPrintInvoice", (object payload) =>
        {
            OnPrintRequest?.Invoke(payload);
        });

        // Manejo de eventos de conexión
        _connection.Reconnecting += error =>
        {
            Console.WriteLine($"Reconectando al hub... Error: {error?.Message}");
            CurrentState = ConnectionState.Reconnecting;
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Console.WriteLine($"Reconectado al hub. ConnectionId: {connectionId}");
            CurrentState = ConnectionState.Connected;
            return Task.CompletedTask;
        };

        _connection.Closed += async error =>
        {
            CurrentState = ConnectionState.Disconnected;
            Console.WriteLine($"Conexión cerrada: {error?.Message}. Reintentando en 3 segundos...");
            await Task.Delay(3000);
            try
            {
                await ConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al reconectar: {ex.Message}");
            }
        };

        await _connection.StartAsync();
        CurrentState = ConnectionState.Connected;
    }

    /// <summary>
    /// Desconecta del hub.
    /// </summary>
    public async Task DisconnectAsync()
    {
        try
        {
            _tokenRenewalTimer?.Dispose();
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
                Console.WriteLine("Desconectado del hub.");
                CurrentState = ConnectionState.Disconnected;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Error de Desconección al hub.");
            CurrentState = ConnectionState.Disconnected;
        }
    }

    #region Metodos privados

    private async Task TryReconnectAsync()
    {
        int attempts = 0;
        while (attempts < 5)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempts)));
                await _connection!.StartAsync();
                //OnConnectionStatusChanged?.Invoke("Reconectado");
                return;
            }
            catch
            {
                attempts++;
            }
        }
        //OnError?.Invoke("No se pudo reconectar después de varios intentos");
    }

    private void StartTokenRenewalTimer()
    {
        // Renovar token 5 minutos antes de que expire (ajusta según necesites)
        _tokenRenewalTimer = new Timer(async _ =>
        {
            await RenewTokenAsync();
        }, null, TimeSpan.FromMinutes(55), TimeSpan.FromMinutes(55));
    }

    private async Task RenewTokenAsync()
    {
        try
        {
            // Aquí implementa la lógica para renovar el token JWT
            // Por ejemplo, llamar a tu API de autenticación con las credenciales almacenadas
            // var newToken = await authService.RenewTokenAsync(_settings.Username, _settings.Password);
            // _settings.JwtToken = newToken;
            // Guardar configuración actualizada

            // Si el token cambió, reiniciar la conexión
            if (_connection?.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
                await _connection.StartAsync();
            }
        }
        catch (Exception)
        {
            //OnError?.Invoke($"Error renovando token: {ex.Message}");
        }
    }

    #endregion

}
