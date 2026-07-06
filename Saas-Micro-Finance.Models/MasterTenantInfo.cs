using Finbuckle.MultiTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class MasterTenantInfo : ITenantInfo
    {
        public string? Id { get; set; }           
        public string? Identifier { get; set; }
        public string? Name { get; set; }         
        public string? ConnectionString { get; set; }
    }
}
