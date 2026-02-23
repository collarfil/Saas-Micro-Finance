using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public WalletOwnerType OwnerType { get; set; } // customer, agent, staff
        public int OwnerId { get; set; }
        public decimal Balance { get; set; }
           
    }
    public enum WalletOwnerType
    {
        Customer =1,
        Agent =2,
        Staff = 3
    }
}
