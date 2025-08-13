using NovaPrinter.Models;
using NovaPrinter.Services;
using NovaPrinter.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NovaPrinter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SignalRService _signalRService = new();
    
    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    /// <summary>
    /// Constructor de la clase
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // Carga settings
        SettingsService.Load();

        // Eventos del menu
        BtnApiConfig.Click += (_, _) => ShowApiConfig();
        BtnPrinters.Click += (_, _) => ShowPrinters();
        BtnAuth.Click += (_, _) => ShowAuthetication();
        BtnAbout.Click += (_, _) => ShowAbout();

        // Mostrar por defecto la vista de ApiConfig
        ShowApiConfig();

        // Wire SignalR events
        _signalRService.OnPrintRequest += OnPrintRequestReceived;

        // Actualiza el estado de conección
        UpdateStatus();
    }

    /// <summary>
    /// 
    /// </summary>
    private void ShowApiConfig()
    {
        var page = new ApiConfigPage(_signalRService);
        MainContent.Content = page;
    }

    private void ShowPrinters()
    {
        var page = new PrintersPage();
        MainContent.Content = page;
    }

    private void ShowAuthetication()
    {
        var page = new AuthPage();
        MainContent.Content = page;
    }

    private void ShowAbout()
    {
        var page = new AboutPage();
        MainContent.Content = page;
    }

    private void UpdateStatus()
    {
        TxtStatus.Text = _signalRService.IsConnected ? "Conectado" : "Desconectado";
        if (_signalRService.IsConnected)
        {
            TxtStatus.Text = "Conectado";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(52, 168, 83)); // Verde
            BtnConnect.Content = "Desconectar";
        }
        else
        {
            TxtStatus.Text = "Desconectado";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(255, 68, 68)); // Rojo
            BtnConnect.Content = "Conectar";
        }
    }

    private void OnPrintRequestReceived(string type, object? payload)
    {
        // Aquí recibes el evento desde SignalR; en prototipo solo logueamos.
        // Puedes deserializar payload según tu contrato y llamar PrinterService.PrintRaw
        Dispatcher.Invoke(() =>
        {
            System.Windows.MessageBox.Show($"Print request received: {type}");
        });
    }

    protected override void OnStateChanged(System.EventArgs e)
    {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Minimized)
        {
            Hide(); // Oculta la ventana pero la app sigue viva
        }
    }

    private async void BtnConnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _signalRService.ConnectAsync(Settings);
            MessageBox.Show($"Conectado al servidor", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al conectar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}