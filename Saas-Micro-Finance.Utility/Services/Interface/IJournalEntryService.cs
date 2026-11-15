using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.Utility.Services.Interface
{
    public interface IJournalEntryService
    {
        Task<IEnumerable<JournalEntryDto>> GetAllAsync(
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sourceModule = null,
            int? branchId = null);

        Task<JournalEntryDto?> GetByIdAsync(int id);

        Task<JournalEntryDto> CreateAsync(JournalEntryCreateDto dto, string userId);

        Task<JournalEntryDto?> PostAsync(int id, string userId);
        Task<JournalEntryDto?> CancelAsync(int id);
        Task<JournalEntryDto?> ReverseAsync(int id, string userId);

        Task<IEnumerable<TrialBalanceRowDto>> GetTrialBalanceAsync(
            DateTime fromDate, DateTime toDate, int? branchId = null);
    }

    public class TrialBalanceRowDto
    {
        public int GlAccountId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Balance { get; set; }   // Debit - Credit
    }
}
