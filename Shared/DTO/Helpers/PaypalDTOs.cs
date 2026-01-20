using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Helpers
{
    public class WithdrawalRequestDTO
    {
        public decimal Amount { get; set; }
        public string PayPalEmail { get; set; } = string.Empty;
    }

    public class WithdrawalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public string PaypalEmail { get; set; } = string.Empty;
        public string PayoutBatchId { get; set; } = string.Empty;
        public string PayoutStatus { get; set; } = string.Empty;
        public int TransactionId { get; set; }
    }

    public class WithdrawalPendingResponse
    {
        public bool Success { get; set; }
        public bool Pending { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PayoutBatchId { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }

    public class CreateOrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string ApprovalUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CaptureOrderResponse
    {
        public bool Success { get; set; }
        public decimal Amount { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
