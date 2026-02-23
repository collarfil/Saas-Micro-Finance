using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public int AccountId { get; set; }
        
        public TransactionType Type { get; set; } // credit, debit   
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string Narration { get; set; }
        public string Channel { get; set; } 
        public DateTime Created_At { get; set; } = DateTime.Now;

    }
    
}
