using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace NovaPrinter;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private TaskbarIcon _notifyIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Mantener vivo el TaskbarIcon
        _notifyIcon = (TaskbarIcon)FindResource("MyNotifyIcon");
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow == null)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
        else
        {
            MainWindow.Show();
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        _notifyIcon.Dispose(); // Liberar icono
        Current.Shutdown();
    }
}
