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

    private void GitHubButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/JesusOspino")
        {
            UseShellExecute = true
        });
    }

    private void LinkedInButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://linkedin.com/in/jesusospino25")
        {
            UseShellExecute = true
        });
    }
}
