using System.Diagnostics;
using System.Windows.Controls;

namespace NovaPrinter.Views;

/// <summary>
/// Lógica de interacción para AboutPage.xaml
/// </summary>
public partial class AboutPage : UserControl
{
    public AboutPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton GitHubButton
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GitHubButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/JesusOspino")
        {
            UseShellExecute = true
        });
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton LinkedInButton
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LinkedInButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://linkedin.com/in/jesusospino25")
        {
            UseShellExecute = true
        });
    }
}
