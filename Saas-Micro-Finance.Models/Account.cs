using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int Account_TypeId { get; set; }
        public Account_Type Account_Type { get; set; }
        public int Account_Number { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime Opened_At { get; set; } = DateTime.Now;

    }
    public enum AccountStatus
    {
        Active =1,
        Inactive =2,
        Closed =3,
        Suspended =4
    }
}
