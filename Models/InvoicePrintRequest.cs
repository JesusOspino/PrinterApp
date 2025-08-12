namespace NovaPrinter.Models;

public class InvoiceLine
{
    public string Text { get; set; } = "";
    public bool Bold { get; set; } = false;
    public int FontSize { get; set; } = 12; // opcional
}

internal class InvoicePrintRequest
{
    public string InvoiceId { get; set; } = "";
    public string Company { get; set; } = "";
    public string? PrinterName { get; set; } // opcional: override
    public int Copies { get; set; } = 1;
    public List<InvoiceLine> Lines { get; set; } = new();
    public Dictionary<string, object>? Meta { get; set; } // cualquier dato extra
}
