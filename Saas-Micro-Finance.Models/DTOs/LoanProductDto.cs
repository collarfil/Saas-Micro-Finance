using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class LoanProductDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public string Name { get; set; }
        public decimal InterestRate { get; set; }
        public int Duration { get; set; } // Duration in months
        public decimal PenaltyRate { get; set; }
    }
}
