using System.Text;
using System.Text.Json.Serialization;
using api_access.Enums;

namespace api_access.Models;

public class TransactionModel
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionTypes Type { get; set; }

    public int? WalletFromId { get; set; }
    public int? WalletToId { get; set; }
    public virtual ICollection<Conversion> Conversions { get; set; } = new List<Conversion>();

    public string ConversionHistory
    {
        get
        {
            var sortedConversions = Conversions.OrderBy(x => x.Order).ToList();

            var history = new StringBuilder();

            if (sortedConversions.Count == 0) return string.Empty;

            string before;
            string after;

            if (sortedConversions[0].AmountBefore == null || sortedConversions[0].CurrencyBefore == null)
            {
                before = "Deposit";
            }
            else
            {
                before = $"{sortedConversions[0].AmountBefore:N2} {sortedConversions[0].CurrencyBefore}";
            }

            if (sortedConversions[0].AmountAfter == null || sortedConversions[0].CurrencyAfter == null)
            {
                after = "Withdraw";
            }
            else
            {
                after = $"{sortedConversions[0].AmountAfter:N2} {sortedConversions[0].CurrencyAfter}";
            }
            
            history.Append($"{before} -> {after}");

            for (var i = 1; i < sortedConversions.Count; i++)
            {
                var conAfter = sortedConversions[i].AmountAfter == null || sortedConversions[i].CurrencyAfter == null
                    ? "Withdraw"
                    : $"{sortedConversions[i].AmountAfter:N2} {sortedConversions[i].CurrencyAfter}";
                
                history.Append($" -> {conAfter}");
            }

            return history.ToString();
        }
    }
}