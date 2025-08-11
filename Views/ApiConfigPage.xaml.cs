using NovaPrinter.Models;
using NovaPrinter.Services;
using System.Windows;
using System.Windows.Controls;

namespace NovaPrinter.Views;

/// <summary>
/// Lógica de interacción para ApiConfigPage.xaml
/// </summary>
public partial class ApiConfigPage : UserControl
{
    private AppSettings _settings;
    private readonly Action<AppSettings> _onSaved;
    private readonly SignalRService _signalRService;

    public ApiConfigPage(AppSettings settings, Action<AppSettings> onSaved, SignalRService signalR)
    {
        InitializeComponent();

        _settings = settings;
        _onSaved = onSaved;
        _signalRService = signalR;

        // Inicializa los valores
        TxtApiUrl.Text = _settings.ApiUrl;
        TxtHub.Text = _settings.HubName;
        TxtCompany.Text = _settings.Company;
        TxtCompanyId.Text = _settings.CompanyId;
        TxtTenantId.Text = _settings.TenantId;

        BtnSave.Click += (_, _) => Save();
        BtnConnect.Click += async (_, _) => await ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        Save();
        try
        {
            await _signalRService.ConnectAsync(_settings);
            MessageBox.Show($"Conectado al hub", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al conectar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save()
    {
        _settings.ApiUrl = TxtApiUrl.Text.Trim();
        _settings.HubName = TxtHub.Text.Trim();
        _settings.Company = TxtCompany.Text.Trim();
        _settings.CompanyId = TxtCompanyId.Text.Trim();
        _settings.TenantId = TxtTenantId.Text.Trim();

        _onSaved?.Invoke(_settings);
        MessageBox.Show("Configuración guardada", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
