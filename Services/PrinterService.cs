namespace NovaPrinter.Services;

using NovaPrinter.Models;
using System.Drawing.Printing;
using System.Windows;

public static class PrinterService
{

    private static AppSettings Settings => SettingsService.Current;

    /// <summary>
    /// Obtiene las impresoras instaladas en el PC del cliente
    /// </summary>
    /// <returns>
    /// Lista de impresoras instaladas
    /// </returns>
    public static string[] GetInstalledPrinters()
    {
        return PrinterSettings.InstalledPrinters
            .Cast<string>()
            .ToArray();
    }

    // Aquí deberías implementar la lógica específica para imprimir en tu impresora térmica.
    // Por ejemplo, enviar bytes ESC/POS por puerto serie/USB, o usar un driver de Windows.
    public static void PrintRaw(byte[] data, InvoiceDto data1)
    {
        if(string.IsNullOrWhiteSpace(Settings.PrinterName))
        {
            MessageBox.Show("No ha selecionado una impresora", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // placeholder: implementar método real según tu impresora
        // Puedes usar RawPrinterHelper (Win32) o librerías de terceros para enviar bytes.
        //throw new NotImplementedException("Implementa el envío raw a la impresora térmica.");
        NotificationService.ShowToast("Imprimir", data1.Customer.Name);
    }
}
