using NovaPrinter.Models;
using NovaPrinter.Services;
using NovaPrinter.Shared;
using System.Windows;
using System.Windows.Controls;

namespace NovaPrinter.Views;

/// <summary>
/// Lógica de interacción para AuthPage.xaml
/// </summary>
public partial class AuthPage : UserControl
{
    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    public AuthPage()
    {
        InitializeComponent();
        InitializeSettings();
    }

    /// <summary>
    /// Metodo que inicializa los datos de la pagina
    /// </summary>
    private void InitializeSettings()
    {
        // Inicializa los valores
        TxtUsername.Text = Settings.Username;
        TxtPassword.Password = Utils.Decrypt(Settings.Password);
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnSave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSave_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Save();
    }

    /// <summary>
    /// Guarda los datos del formulario
    /// </summary>
    private void Save()
    {
        Settings.Username = TxtUsername.Text.Trim();
        Settings.Password = Utils.Encrypt(TxtPassword.Password.Trim());

        SettingsService.Save();

        MessageBox.Show("Credenciales guardadas con exito.", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
    }

}
