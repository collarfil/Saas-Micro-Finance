using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class GLAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } // e.g. 1001, 2001
        public GLAccountType Type { get; set; }

        public decimal Balance { get; set; }

        public int? ParentId { get; set; } // for COA hierarchy
        public GLAccount Parent { get; set; }
    }
    public enum GLAccountType
    {
        Asset = 1,
        Liability = 2,
        Equity = 3,
        Income = 4,
        Expense = 5
    }
}
