using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string StaffNumber { get; set; }
        public int DepartmentId { get; set; }
        
        public string Position { get; set; }
    }
}
