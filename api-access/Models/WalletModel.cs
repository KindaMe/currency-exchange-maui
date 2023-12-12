namespace api_access.Models;

public class WalletModel
{
    public int id { get; set; }

    public double balance { get; set; }

    public string currency { get; set; }

    public int user_id { get; set; }

    public double CurrentPlnRate { get; set; }

    public double ConvertedBalance => balance * CurrentPlnRate;
}