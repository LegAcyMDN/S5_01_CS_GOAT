namespace Shared.DTO.Helpers;

/// <summary>
/// DTO for creating a payment
/// </summary>
public class CreatePaymentDTO
{
    /// <summary>
    /// Amount to add to wallet in EUR
    /// </summary>
    public decimal Amount { get; set; }
}