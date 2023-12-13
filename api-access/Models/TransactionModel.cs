namespace api_access.Models;

public class TransactionModel
{
    public int id { get; set; }
    public DateTime date { get; set; }
    public int wallet_id { get; set; }
    public string currency_in { get; set; }
    public decimal amount_in { get; set; }
}