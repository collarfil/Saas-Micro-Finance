using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public TransactionType Type { get; set; }= TransactionType.Credit; // credit, debit
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string Narration { get; set; }
        public bool IsReversed { get; set; }
        public int? ReversalTransactionId { get; set; }
        public string Channel { get; set; } // cash, transfer, POS
        public DateTime Created_At { get; set; } = DateTime.Now;

    }
    public enum TransactionType
    {
        Credit =1,
        Debit =2
    }
    public enum TransactionStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
