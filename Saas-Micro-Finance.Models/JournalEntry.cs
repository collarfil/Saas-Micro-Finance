using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        // Unique human-readable identifier (e.g., "JE-2026-00001")
        public string EntryNumber { get; set; } = string.Empty;

        // The official accounting date for financial reporting
        public DateTime PostingDate { get; set; } = DateTime.UtcNow;

        // Narration or description explaining why the entry was made
        public string Description { get; set; } = string.Empty;

        // External reference (e.g., Bank teller ref, Loan ID, Invoice #)
        public string? Reference { get; set; }

        // Source module that generated this entry (e.g., "Loans", "Savings", "Manual", "Payroll")
        public string SourceModule { get; set; } = "Manual";

        // Status of the entry
        public JournalStatus Status { get; set; } = JournalStatus.Draft;

        // Optional: Multi-branch support if your microfinance handles branch segregation
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        // Audit & Traceability Fields
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? PostedByUserId { get; set; }
        public DateTime? PostedAt { get; set; }
        public int? ReversalOfJournalEntryId { get; set; }
        public JournalEntry? ReversalOfJournalEntry { get; set; }

        // Navigation Property: The individual ledger legs (At least 2 lines required)
        public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
    }

    public enum JournalStatus
    {
        Draft = 0,
        Posted = 1,
        Reversed = 2,
        Cancelled = 3
    }
}
