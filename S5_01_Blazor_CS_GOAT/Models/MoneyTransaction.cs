namespace S5_01_Blazor_CS_GOAT.Models;

public class MoneyTransaction
{
    public int PaymentMethodId { get; set; }
    public string? PaymentMethod { get; set; }
    public int TransactionId {get; set;}
    public DateTime TransactionDate  {get; set;}
    public double WalletValue { get; set; }
    public DateTime CancelledOn { get; set; }
    public int UserId { get; set; }
    public int? NotificationId { get; set; }
    public User? User { get; set; }
    public int? Notification { get; set; } // TODO use notification model
    public int DependantUserId {get; set;}
}