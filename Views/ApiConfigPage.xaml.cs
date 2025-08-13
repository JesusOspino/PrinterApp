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
    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    public ApiConfigPage(SignalRService signalR)
    {
        InitializeComponent();
        InitializeApiConfig();
    }

    /// <summary>
    /// Metodo que inicializa los datos al abrir la pagina
    /// </summary>
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
