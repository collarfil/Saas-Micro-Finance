using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Customer_KYC
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string NIN { get; set; }
        public string BVN { get; set; }
        public string ID_Type { get; set; }
        public string ID_Number { get; set; }
        public string Passport { get; set; }
        public DateTime Verfied_At { get; set; } = DateTime.Now;
    }
}
