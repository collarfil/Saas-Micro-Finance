using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class WalletDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public WalletOwnerType OwnerType { get; set; } // customer, agent, staff
        public int OwnerId { get; set; }
        public decimal Balance { get; set; }

    }
    
}
