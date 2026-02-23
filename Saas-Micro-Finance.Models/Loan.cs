using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int LoanProductId { get; set; }
        public LoanProduct LoanProduct { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal TotalPayable => Principal + Interest;
        public LoanStatus Status { get; set; }
        public DateTime DisbursedAt { get; set; } = DateTime.Now;
    }
    public enum LoanStatus
    {
        Applied = 1,
        Pending=2,
        Approved =3,
        Rejected =4,
        Disbursed =5,
        Repaid =6,
        Defaulted= 7,
        Closed =8
    }
}
