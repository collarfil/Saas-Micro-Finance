using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string License_Number { get; set; }
        public string CBN_Code { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
    }
    public enum TenantStatus
    {
        Active =1,
        Inactive =2,
        Suspended =3
    }
}
