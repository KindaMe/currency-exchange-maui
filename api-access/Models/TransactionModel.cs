namespace api_access.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int WalletId { get; set; }
    public string CurrencyIn { get; set; }
    public decimal AmountIn { get; set; }
}