using NovaPrinter.Models;
using NovaPrinter.Services;
using System.Windows;
using System.Windows.Controls;

namespace NovaPrinter.Views;

/// <summary>
/// Lógica de interacción para PrintersPage.xaml
/// </summary>
public partial class PrintersPage : UserControl
{
    private AppSettings _settings;
    private readonly Action<AppSettings> _onSaved;

    public PrintersPage(AppSettings settings, Action<AppSettings> onSaved)
    {
        InitializeComponent();

        _settings = settings;
        _onSaved = onSaved;

        // obtenamos las impresoras
        var printers = PrinterService.GetInstalledPrinters();

        // Asignamos las impresoras al comboBox
        CmbPrinters.ItemsSource = printers;

        // Si ya existe una seleccioinada se marca
        if (!string.IsNullOrWhiteSpace(_settings.PrinterName) && printers.Contains(_settings.PrinterName))
        {
            CmbPrinters.SelectedItem = _settings.PrinterName;
        }

    }

    private void BtnSavePrinter_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var selected = CmbPrinters.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(selected))
        {
            MessageBox.Show("Seleccione una impresora.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _settings.PrinterName = selected;
        _onSaved?.Invoke(_settings);
        MessageBox.Show("Impresora guardada con exito.", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
