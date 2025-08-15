using NovaPrinter.Models;
using NovaPrinter.Services;
using NovaPrinter.Views;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace NovaPrinter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SignalRService _signalRService = new();
    
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
        _signalRService.OnConnectionStateChanged += OnReciveStatusConnection;

        // Actualiza el estado de conección
        UpdateStatus();
    }

    /// <summary>
    /// Metodo que abre la pagina de ApiConfig
    /// </summary>
    private void ShowDefaultApiConfigPage()
    {
        var page = new ApiConfigPage();
        MainContent.Content = page;
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
        UpdateStatus();
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnPrinters
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnPrinters_Click(object sender, RoutedEventArgs e)
    {
        NotificationService.ShowToast("Prueba", "Jesus Ospino");
        var page = new PrintersPage();
        MainContent.Content = page;
        UpdateStatus();
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
        UpdateStatus();
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
        UpdateStatus();
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
            if (_signalRService.CurrentState == ConnectionState.Connected)
            {
                await _signalRService.DisconnectAsync();
            }
            else
            {
                await _signalRService.ConnectAsync();
            }

            UpdateStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al conectar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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

    #endregion Metodos o eventos de controles de la app

    #region Metodos de cambio de estado

    private void OnReciveStatusConnection(ConnectionState state)
    {
        Dispatcher.Invoke(() =>
        {
            UpdateStatus();
        });
    }

    /// <summary>
    /// Verifica el estado de conexión y actualiza la parte visual
    /// </summary>
    private void UpdateStatus()
    {
        if (_signalRService.CurrentState == ConnectionState.Connecting)
        {
            TxtStatus.Text = "Conectando...";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0)); // Amarillo
            BtnConnect.IsEnabled = false;
        }
        else if (_signalRService.CurrentState == ConnectionState.Connected)
        {
            TxtStatus.Text = "Conectado";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(52, 168, 83)); // Verde
            BtnConnect.Content = "Desconectar";
            BtnConnect.IsEnabled = true;
        }
        else if (_signalRService.CurrentState == ConnectionState.Reconnecting)
        {
            TxtStatus.Text = "Reconectando...";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0)); // Amarillo
            BtnConnect.Content = "Conectar";
            BtnConnect.IsEnabled = false;
        }
        else if (_signalRService.CurrentState == ConnectionState.Disconnecting)
        {
            TxtStatus.Text = "Desconectando...";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 0)); // Amarillo
            BtnConnect.Content = "Conectar";
            BtnConnect.IsEnabled = false;
        }
        else if (_signalRService.CurrentState == ConnectionState.Disconnected)
        {
            TxtStatus.Text = "Desconectado";
            StatusIndicator.Fill = new SolidColorBrush(Color.FromRgb(255, 68, 68)); // Rojo
            BtnConnect.Content = "Conectar";
            BtnConnect.IsEnabled = true;
        }
    }

    #endregion Metodos de cambio de estado

    #region Metodos para imprimir

    /// <summary>
    /// verifica cuando hay una facura y despara un evento para imprimirla
    /// </summary>
    /// <param name="type"></param>
    /// <param name="payload"></param>
    private void OnPrintRequestReceived(object payload)
    {
        // Aquí recibes el evento desde SignalR; en prototipo solo logueamos.
        // Puedes deserializar payload según tu contrato y llamar PrinterService.PrintRaw
        Dispatcher.Invoke(() =>
        {
            var invoice = JsonSerializer.Deserialize<InvoiceDto>(payload.ToString()!);
            TxtStatus.Text = invoice?.Customer.Name;
            PrinterService.PrintRaw(GetInvoiceAsBytes(invoice!));
        });
    }

    // Método auxiliar para convertir la factura a bytes para impresión
    private byte[] GetInvoiceAsBytes(InvoiceDto invoice)
    {
        // Implementa según tu lógica de impresión
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(invoice));
    }

    #endregion Metodos para imprimir

}