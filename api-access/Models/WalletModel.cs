namespace api_access.Models;

public class WalletModel
{
    public int Id { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
    public int UserId { get; set; }
    public ICollection<TransactionModel> Transactions { get; set; }
    
    public double GainPercentage { get; set; }
    public double CurrentRate { get; set; }
    public double ConvertedBalance => Balance * CurrentRate;
}