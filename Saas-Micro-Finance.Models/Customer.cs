using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public CustomerStatus Status { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;

        // Navigation properties
        
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Customer_Address> Addresses { get; set; } = new List<Customer_Address>();
        public Customer_KYC? KYC { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    }
    public enum CustomerStatus
    {
        Active = 1,
        Inactive =2,
        Blacklisted = 3
    }
}
