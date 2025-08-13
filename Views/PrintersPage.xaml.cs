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
    // Accedemos directamente al servicio estático
    private AppSettings Settings => SettingsService.Current;

    public PrintersPage()
    {
        InitializeComponent();
        InitializePrinters();
    }

    /// <summary>
    /// Metodo que inicializa los datos de la pagina
    /// </summary>
    private void InitializePrinters()
    {
        // obtenamos las impresoras
        var printers = PrinterService.GetInstalledPrinters();

        // Asignamos las impresoras al comboBox
        CmbPrinters.ItemsSource = printers;

        // Si ya existe una seleccioinada se marca
        if (!string.IsNullOrWhiteSpace(Settings.PrinterName) && printers.Contains(Settings.PrinterName))
        {
            CmbPrinters.SelectedItem = Settings.PrinterName;
        }
    }

    /// <summary>
    /// Metodo que se ejecuta al hacer click en el boton BtnSavePrinter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSavePrinter_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Save();
    }

    /// <summary>
    /// Guarda los datos del formulario
    /// </summary>
    private void Save()
    {
        var selected = CmbPrinters.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(selected))
        {
            MessageBox.Show("Seleccione una impresora.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Settings.PrinterName = selected;
        SettingsService.Save();

        MessageBox.Show("Impresora guardada con exito.", "Ok", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
