using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class LoanRepayment
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }= DateTime.Now;
        public RepaymentStatus Status { get; set; }
       
    }
    public enum RepaymentStatus
    {
        Paid =1,
        Pending=2,
        Completed =3,
        Overdue=4
    }
}
