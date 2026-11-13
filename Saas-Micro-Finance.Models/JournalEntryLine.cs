using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class JournalEntryLine
    {
        public int Id { get; set; }

        // Foreign Key to Parent Journal Entry
        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; } = null!;

        // Foreign Key to your General Ledger Account (GL Account)
        public int GlAccountId { get; set; }
        public GLAccount GLAccount { get; set; } = null!;

        // Financial Amounts (Strictly mapped to precision 18,2 in DbContext)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0.00m;

        // Optional line-level description if different from header narration
        public string? Description { get; set; }

        // Optional: Link to a specific customer or loan if tracking sub-ledgers
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
