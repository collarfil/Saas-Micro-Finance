using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public ICollection<Employee> Employees { get; set; }=new List<Employee>();
    }
}
