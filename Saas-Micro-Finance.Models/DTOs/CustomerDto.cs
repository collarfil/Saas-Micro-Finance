using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AdminPassword { get; set; } = string.Empty;
        public CustomerStatus Status { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;

    }
    
}
