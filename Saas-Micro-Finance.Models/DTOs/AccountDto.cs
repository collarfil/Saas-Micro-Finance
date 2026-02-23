using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public int CustomerId { get; set; }
        
        public int Account_TypeId { get; set; }
        
        public int Account_Number { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime Opened_At { get; set; } = DateTime.Now;
    }
   
}
