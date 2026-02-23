using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int TenantId { get; set; }           // Multi-tenancy
        public string PlanName { get; set; }        // e.g., Basic, Premium
        public decimal Amount { get; set; }         // Plan price
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
