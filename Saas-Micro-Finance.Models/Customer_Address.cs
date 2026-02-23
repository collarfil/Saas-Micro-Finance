using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Customer_Address
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }   
        public string Country { get; set; }

    }
}
