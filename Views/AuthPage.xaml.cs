using NovaPrinter.Models;
using NovaPrinter.Shared;
using System.Windows;
using System.Windows.Controls;

namespace NovaPrinter.Views;

/// <summary>
/// Lógica de interacción para AuthPage.xaml
/// </summary>
public partial class AuthPage : UserControl
{
    private AppSettings _settings;
    private readonly Action<AppSettings> _onSaved;

    public AuthPage(AppSettings settings, Action<AppSettings> onSaved)
    {
        InitializeComponent();

        _settings = settings;
        _onSaved = onSaved;

        // Inicializa los valores
        TxtUsername.Text = _settings.Username;
        TxtPassword.Password = Utils.Decrypt(_settings.Password);
    }

    private void BtnSave_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        _settings.Username = TxtUsername.Text.Trim();
        _settings.Password = Utils.Encrypt(TxtPassword.Password.Trim());

        _onSaved?.Invoke(_settings);
        MessageBox.Show("Credenciales guardadas con exito.", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
    }

}
