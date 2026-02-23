using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int CustomerId { get; set; }
        public int LoanProductId { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime DisbursedAt { get; set; }
    }
    
}
