using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public AuditAction Action { get; set; }
        public string IpAddress { get; set; }
     
    }
    public enum AuditAction
    {
        Create =1,
        Update =2,
        Delete =3,
        Login =4,
        Logout=5
    }
}
