using api_access.Enums;

namespace api_access.Models;

public class WalletModel
{
    public int Id { get; set; }
    public string Currency { get; set; }
    public int UserId { get; set; }
    public ICollection<TransactionModel> TransactionWalletTos { get; set; }
    public ICollection<TransactionModel> TransactionWalletFroms { get; set; }

    public decimal CurrentBalance { get; private set; } //current balance
    public decimal CurrentBalanceInLocalCurrency { get; private set; } //current balance in PLN

    private ExchangeRateTableModel _table;
    private decimal _currentBalance;
    private decimal _currentBalanceInLocalCurrency;

    public string FlagUrl { get; set; }

    public ICollection<TransactionModel> CombinedTransactions
    {
        get
        {
            var combinedTransactions = new List<TransactionModel>();
            combinedTransactions.AddRange(TransactionWalletTos);
            combinedTransactions.AddRange(TransactionWalletFroms);

            var sortedTransactions = combinedTransactions.OrderByDescending(x => x.Date).ToList();

            return sortedTransactions;
        }
    }
    
    public IEnumerable<IGrouping<DateOnly, TransactionModel>> GroupedTransactions
    {
        get
        {
            var combinedTransactions = new List<TransactionModel>();
            combinedTransactions.AddRange(TransactionWalletTos);
            combinedTransactions.AddRange(TransactionWalletFroms);

            var groupedTransactions = combinedTransactions
                .GroupBy(x => x.Date)
                .OrderByDescending(g => g.Key);

            return groupedTransactions;
        }
    }

    public void InitWalletDetails(ExchangeRateTableModel table)
    {
        _table = table;

        _currentBalance = 0;
        _currentBalanceInLocalCurrency = 0;

        TransactionCalculator(CombinedTransactions);

        CurrentBalance = _currentBalance;
        CurrentBalanceInLocalCurrency = _currentBalanceInLocalCurrency;
    }

    private Rate GetCurrentRate(string currency)
    {
        if (currency == "PLN")
        {
            return new Rate
            {
                code = "PLN",
                mid = 1,
                ask = 1,
                bid = 1,
                effectiveDate = DateTime.Now.ToString("yyyy-MM-dd")
            };
        }

        return _table.rates.FirstOrDefault(x => x.code == currency);
    }

    private void TransactionCalculator(IEnumerable<TransactionModel> transactions)
    {
        foreach (var transaction in transactions)
        {
            if (transaction.Conversions.Count == 0) continue;

            if (transaction.Type == TransactionTypes.Deposit)
            {
                var conversion = transaction.Conversions.MaxBy(x => x.Order);

                if (conversion.AmountAfter != null)
                {
                    _currentBalance += conversion.AmountAfter.Value;
                }
            }
            else if (transaction.Type == TransactionTypes.Withdrawal)
            {
                var conversion = transaction.Conversions.MinBy(x => x.Order);

                if (conversion.AmountBefore != null)
                {
                    _currentBalance -= conversion.AmountBefore.Value;
                }
            }
            else if (transaction.Type == TransactionTypes.Transfer)
            {
                foreach (var conversion in transaction.Conversions)
                {
                    if (conversion.Order == 0)
                    {
                        if (transaction.Conversions.Count == 1)
                        {
                            if (transaction.WalletToId == Id)
                            {
                                if (conversion.AmountAfter != null)
                                {
                                    _currentBalance += conversion.AmountAfter.Value;
                                }
                            }
                            else if (transaction.WalletFromId == Id)
                            {
                                if (conversion.AmountBefore != null)
                                {
                                    _currentBalance -= conversion.AmountBefore.Value;
                                }
                            }
                        }
                        else if (transaction.Conversions.Count == 2)
                        {
                            if (transaction.WalletFromId == Id)
                            {
                                if (conversion.AmountBefore != null)
                                    _currentBalance -= conversion.AmountBefore.Value;
                            }
                        }
                    }
                    else if (conversion.Order == 1)
                    {
                        if (transaction.WalletToId == Id)
                        {
                            if (conversion.AmountAfter != null)
                                _currentBalance += conversion.AmountAfter.Value;
                        }
                    }
                }
            }
        }

        _currentBalanceInLocalCurrency = _currentBalance * GetCurrentRate(Currency).bid;
    }

    public override string ToString()
    {
        return $"{Currency.ToUpper()} {CurrentBalance:N2}";
    }
}