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
    private readonly SignalRService _signalRService;

    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    public ApiConfigPage(SignalRService signalR)
    {
        InitializeComponent();
        InitializeApiConfig();

        _signalRService = signalR;
    }

    private void InitializeApiConfig()
    {
        // Inicializa los valores
        TxtApiUrl.Text = Settings.ApiUrl;
        TxtHub.Text = Settings.HubName;
        TxtCompany.Text = Settings.Company;
        TxtCompanyId.Text = Settings.CompanyId;
        TxtTenantId.Text = Settings.TenantId;
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnSave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        Save();
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnConnect
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    private async void BtnConnect_Click(object sender, RoutedEventArgs e)
    {
        await ConnectAsync();
    }

    /// <summary>
    /// Metodo que inicia la conección al backend y conecta SignalR
    /// </summary>
    /// <returns></returns>
    private async Task ConnectAsync()
    {
        Save();
        try
        {
            await _signalRService.ConnectAsync(Settings);
            MessageBox.Show($"Conectado al hub", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al conectar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Guarda los datos del formulario
    /// </summary>
    private void Save()
    {
        Settings.ApiUrl = TxtApiUrl.Text.Trim();
        Settings.HubName = TxtHub.Text.Trim();
        Settings.Company = TxtCompany.Text.Trim();
        Settings.CompanyId = TxtCompanyId.Text.Trim();
        Settings.TenantId = TxtTenantId.Text.Trim();

        SettingsService.Save();

        MessageBox.Show("Configuración guardada", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
