using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class ApplyLoanDto
    {
        public int CustomerId { get; set; }

        public int LoanProductId { get; set; }

        public decimal Principal { get; set; }
    }
}
