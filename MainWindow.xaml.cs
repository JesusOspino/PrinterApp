using NovaPrinter.Models;
using NovaPrinter.Services;
using NovaPrinter.Views;
using System.Windows;

namespace NovaPrinter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SignalRService _signalRService = new();
    private AppSettings _settings;

    /// <summary>
    /// Constructor de la clase
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // Carga settings
        _settings = SettingsService.Load();

        // Eventos del menu
        BtnApiConfig.Click += (_, _) => ShowApiConfig();
        BtnPrinters.Click += (_, _) => ShowPrinters();
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
        var page = new ApiConfigPage(_settings, OnSettingsSaved, _signalRService);
        MainContent.Content = page;
    }

    private void ShowPrinters()
    {
        var page = new PrintersPage(_settings, OnSettingsSaved);
        MainContent.Content = page;
    }

    private void ShowAbout()
    {
        var textBlock = new System.Windows.Controls.TextBlock
        {
            Text = "Nova Printer\nVersión de prototipo\nDiseñado para imprimir en impresoras térmicas via SignalR",
            Margin = new Thickness(10)
        };
        MainContent.Content = textBlock;
    }

    private void OnSettingsSaved(AppSettings settings)
    {
        _settings = settings;
        SettingsService.Save(_settings);
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        TxtStatus.Text = _signalRService.IsConnected ? "Conectado" : "Desconectado";
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

}