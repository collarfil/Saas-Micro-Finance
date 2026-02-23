using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Flutterwave
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
