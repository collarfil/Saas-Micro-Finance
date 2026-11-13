using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class JournalEntryDto
    {
        public int Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime PostingDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string SourceModule { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Financial totals for quick UI verification
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }

        public List<JournalEntryLineDto> Lines { get; set; } = new();
    }

    public class JournalEntryLineDto
    {
        public int Id { get; set; }
        public int GlAccountId { get; set; }
        public string GlAccountCode { get; set; } = string.Empty; // e.g., "1010"
        public string GlAccountName { get; set; } = string.Empty; // e.g., "Cash in Vault"
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public int? CustomerId { get; set; }
    }
}
