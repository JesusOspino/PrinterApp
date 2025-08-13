using NovaPrinter.Models;
using NovaPrinter.Services;
using NovaPrinter.Views;
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
        InitializeAPP();
    }

    /// <summary>
    /// Metodo que inicializa los datos al abrir la APP
    /// </summary>
    private void InitializeAPP()
    {
        // Carga settings
        SettingsService.Load();

        // Mostrar por defecto la vista de ApiConfig
        ShowDefaultApiConfigPage();

        // Wire SignalR events
        _signalRService.OnPrintRequest += OnPrintRequestReceived;

        // Actualiza el estado de conección
        UpdateStatus();
    }

    #region Metodos o eventos de controles de la app

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnApiConfig
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnApiConfig_Click(object sender, RoutedEventArgs e)
    {
        ShowDefaultApiConfigPage();
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnPrinters
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnPrinters_Click(object sender, RoutedEventArgs e)
    {
        var page = new PrintersPage();
        MainContent.Content = page;
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnAuth
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnAuth_Click(object sender, RoutedEventArgs e)
    {
        var page = new AuthPage();
        MainContent.Content = page;
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnAbout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnAbout_Click(object sender, RoutedEventArgs e)
    {
        var page = new AboutPage();
        MainContent.Content = page;
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnConnect
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    #endregion Metodos o eventos de controles de la app

    /// <summary>
    /// Metodo que abre la pagina de ApiConfig
    /// </summary>
    private void ShowDefaultApiConfigPage()
    {
        var page = new ApiConfigPage(_signalRService);
        MainContent.Content = page;
    }

    /// <summary>
    /// Verifica el estado de conexión y actualiza la parte visual
    /// </summary>
    private void UpdateStatus()
    {
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

    /// <summary>
    /// verifica cuando hay una facura y despara un evento para imprimirla
    /// </summary>
    /// <param name="type"></param>
    /// <param name="payload"></param>
    private void OnPrintRequestReceived(string type, object? payload)
    {
        // Aquí recibes el evento desde SignalR; en prototipo solo logueamos.
        // Puedes deserializar payload según tu contrato y llamar PrinterService.PrintRaw
        Dispatcher.Invoke(() =>
        {
            System.Windows.MessageBox.Show($"Print request received: {type}");
        });
    }

    /// <summary>
    /// Verifica cuand se minimiza la app y la oculta para que se ejecute en segundo plano
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStateChanged(System.EventArgs e)
    {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Minimized)
        {
            Hide(); // Oculta la ventana pero la app sigue viva
        }
    }

    
}