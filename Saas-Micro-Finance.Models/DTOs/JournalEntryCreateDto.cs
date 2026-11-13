using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class JournalEntryCreateDto
    {
        [Required]
        public DateTime PostingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public string? Reference { get; set; }

        public string SourceModule { get; set; } = "Manual";

        public int? BranchId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "A journal entry must have at least two lines (a debit and a credit).")]
        public List<JournalEntryLineCreateDto> Lines { get; set; } = new();
    }

    public class JournalEntryLineCreateDto
    {
        [Required]
        public int GlAccountId { get; set; }

        [Range(0, 1000000000000, ErrorMessage = "Debit amount must be zero or greater.")]
        public decimal Debit { get; set; } = 0.00m;

        [Range(0, 1000000000000, ErrorMessage = "Credit amount must be zero or greater.")]
        public decimal Credit { get; set; } = 0.00m;

        [StringLength(250)]
        public string? Description { get; set; }

        public int? CustomerId { get; set; }
    }
}
