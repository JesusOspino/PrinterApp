namespace NovaPrinter.Models;
public class InvoiceDto
{
    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public CustomerDto Customer { get; set; }
    public List<InvoiceItemDto> Items { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string FooterMessage { get; set; }
}

public class CustomerDto
{
    public string Name { get; set; }
}

public class InvoiceItemDto
{
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}