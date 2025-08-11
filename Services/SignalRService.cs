using Microsoft.AspNetCore.SignalR.Client;
using NovaPrinter.Models;

namespace NovaPrinter.Services;

public class SignalRService
{
    private HubConnection? _connection;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public event Action<string, object?>? OnPrintRequest; // Evento simple

    public async Task ConnectAsync(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ApiUrl) || string.IsNullOrWhiteSpace(settings.HubName))
            throw new InvalidOperationException("Api url o Hub vacio.");

        var hubUrl = CombineUrl(settings.ApiUrl, settings.HubName);

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        // Ej: el servidor invoca "Print" con payload
        _connection.On<string, object?>("Print", (messageType, payload) =>
        {
            OnPrintRequest?.Invoke(messageType, payload);
        });

        await _connection.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    #region Metodos privados

    private string CombineUrl(string baseUrl, string hub)
    {
        // aseguro formato correcto
        var trimmed = baseUrl.TrimEnd('/');
        var h = hub.TrimStart('/');
        return $"{trimmed}/{h}";
    }

    #endregion Metodos privados
}
