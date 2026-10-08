using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; } 

        public ApplicationUser? ApplicationUser { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        public string FirstName { get; set; }=string.Empty;
        public string LastName { get; set; }= string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string StaffNumber { get; set; }= string.Empty;
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public string Position { get; set; }= string.Empty;
        public string Street { get; set; }= string.Empty;

    }
}
