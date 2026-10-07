namespace Barks.Models;

public class Transaction
{
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; } = "";

    public int Quantity { get; set; }

    public DateTime Date { get; set; }

    public Product? Product { get; set; }
}
