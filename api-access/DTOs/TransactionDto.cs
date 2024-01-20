using api_access.Enums;

namespace api_access.DTOs;

public class TransactionDto
{
    public TransactionTypes Type { get; set; }
    public int? FromWalletId { get; set; }
    public int? ToWalletId { get; set; }
    public decimal Amount { get; set; }
}