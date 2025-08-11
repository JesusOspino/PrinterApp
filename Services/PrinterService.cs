namespace NovaPrinter.Services;
using System.Drawing.Printing;

public static class PrinterService
{
    public static string[] GetInstalledPrinters()
    {
        return PrinterSettings.InstalledPrinters
            .Cast<string>()
            .ToArray();
    }

    // Aquí deberías implementar la lógica específica para imprimir en tu impresora térmica.
    // Por ejemplo, enviar bytes ESC/POS por puerto serie/USB, o usar un driver de Windows.
    public static void PrintRaw(string printerName, byte[] data)
    {
        // placeholder: implementar método real según tu impresora
        // Puedes usar RawPrinterHelper (Win32) o librerías de terceros para enviar bytes.
        throw new NotImplementedException("Implementa el envío raw a la impresora térmica.");
    }
}
