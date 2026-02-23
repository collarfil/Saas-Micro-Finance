using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class Account_TypeDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public decimal Minimum_Balance { get; set; }
        public decimal Interest_Rate { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
    }
}
