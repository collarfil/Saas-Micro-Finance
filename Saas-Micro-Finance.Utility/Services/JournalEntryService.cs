using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.Utility.Services
{
    public class JournalEntryService : IJournalEntryService
    {
        private readonly IUnitOfWork _uow;

        public JournalEntryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<JournalEntryDto>> GetAllAsync(
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sourceModule = null,
            int? branchId = null)
        {
            var query = _uow.JournalEntries.Query()
                .Include(j => j.Lines).ThenInclude(l => l.GLAccount)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<JournalStatus>(status, true, out var st))
                query = query.Where(j => j.Status == st);

            if (fromDate.HasValue)
                query = query.Where(j => j.PostingDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(j => j.PostingDate <= toDate.Value);

            if (!string.IsNullOrWhiteSpace(sourceModule))
                query = query.Where(j => j.SourceModule == sourceModule);

            if (branchId.HasValue)
                query = query.Where(j => j.BranchId == branchId.Value);

            var entries = await query
                .OrderByDescending(j => j.PostingDate)
                .ThenByDescending(j => j.Id)
                .ToListAsync();

            return entries.Select(MapToDto).ToList();
        }

        public async Task<JournalEntryDto?> GetByIdAsync(int id)
        {
            var entry = await _uow.JournalEntries.Query()
                .Include(j => j.Lines).ThenInclude(l => l.GLAccount)
                .FirstOrDefaultAsync(j => j.Id == id);

            return entry == null ? null : MapToDto(entry);
        }

        public async Task<JournalEntryDto> CreateAsync(JournalEntryCreateDto dto, string userId)
        {
            // ---- Validation ----
            if (dto.Lines == null || dto.Lines.Count < 2)
                throw new InvalidOperationException("A journal entry must have at least 2 lines.");

            var totalDebit = dto.Lines.Sum(l => l.Debit);
            var totalCredit = dto.Lines.Sum(l => l.Credit);

            if (totalDebit != totalCredit)
                throw new InvalidOperationException(
                    $"Entry is not balanced. Total debit = {totalDebit}, total credit = {totalCredit}.");

            if (totalDebit == 0)
                throw new InvalidOperationException("Total debit/credit must be greater than zero.");

            foreach (var line in dto.Lines)
            {
                if (line.Debit < 0 || line.Credit < 0)
                    throw new InvalidOperationException("Line amounts cannot be negative.");

                if ((line.Debit > 0 && line.Credit > 0) || (line.Debit == 0 && line.Credit == 0))
                    throw new InvalidOperationException(
                        $"Line for GL Account {line.GlAccountId} must have exactly one of Debit or Credit set (not both, not neither).");

                var gl = await _uow.GLAccounts.GetByIdAsync(line.GlAccountId);
                if (gl == null)
                    throw new InvalidOperationException($"GL Account {line.GlAccountId} does not exist.");
                if (!gl.IsActive)
                    throw new InvalidOperationException($"GL Account '{gl.Code} - {gl.Name}' is inactive.");
            }

            // ---- Generate EntryNumber ----
            var year = DateTime.UtcNow.Year;
            var lastEntry = await _uow.JournalEntries.Query()
                .Where(j => j.EntryNumber.StartsWith($"JE-{year}-"))
                .OrderByDescending(j => j.EntryNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastEntry != null)
            {
                var lastSeq = lastEntry.EntryNumber.Split('-').LastOrDefault();
                if (int.TryParse(lastSeq, out var parsed)) nextNumber = parsed + 1;
            }

            var entry = new JournalEntry
            {
                EntryNumber = $"JE-{year}-{nextNumber:D5}",
                PostingDate = dto.PostingDate,
                Description = dto.Description,
                Reference = dto.Reference,
                SourceModule = dto.SourceModule,
                Status = JournalStatus.Draft,
                BranchId = dto.BranchId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Lines = dto.Lines.Select(l => new JournalEntryLine
                {
                    GlAccountId = l.GlAccountId,
                    Debit = l.Debit,
                    Credit = l.Credit,
                    Description = l.Description,
                    CustomerId = l.CustomerId
                }).ToList()
            };

            await _uow.JournalEntries.AddAsync(entry);
            await _uow.SaveAsync();

            return await GetByIdAsync(entry.Id)
                   ?? throw new InvalidOperationException("Created entry could not be reloaded.");
        }

        public async Task<JournalEntryDto?> PostAsync(int id, string userId)
        {
            var entry = await _uow.JournalEntries.GetByIdAsync(id);
            if (entry == null) return null;

            if (entry.Status != JournalStatus.Draft)
                throw new InvalidOperationException($"Only Draft entries can be posted. Current status: {entry.Status}.");

            entry.Status = JournalStatus.Posted;
            entry.PostedByUserId = userId;
            entry.PostedAt = DateTime.UtcNow;

            _uow.JournalEntries.Update(entry);
            await _uow.SaveAsync();

            return await GetByIdAsync(id);
        }

        public async Task<JournalEntryDto?> CancelAsync(int id)
        {
            var entry = await _uow.JournalEntries.GetByIdAsync(id);
            if (entry == null) return null;

            if (entry.Status != JournalStatus.Draft)
                throw new InvalidOperationException($"Only Draft entries can be cancelled. Current status: {entry.Status}.");

            entry.Status = JournalStatus.Cancelled;
            _uow.JournalEntries.Update(entry);
            await _uow.SaveAsync();

            return await GetByIdAsync(id);
        }

        public async Task<JournalEntryDto?> ReverseAsync(int id, string userId)
        {
            var original = await _uow.JournalEntries.Query()
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (original == null) return null;

            if (original.Status != JournalStatus.Posted)
                throw new InvalidOperationException($"Only Posted entries can be reversed. Current status: {original.Status}.");

            // Already reversed?
            var existingReversal = await _uow.JournalEntries.Query()
                .FirstOrDefaultAsync(j => j.ReversalOfJournalEntryId == id);
            if (existingReversal != null)
                throw new InvalidOperationException($"Entry already reversed by JE #{existingReversal.EntryNumber}.");

            // Generate reversal number
            var year = DateTime.UtcNow.Year;
            var lastEntry = await _uow.JournalEntries.Query()
                .Where(j => j.EntryNumber.StartsWith($"JE-{year}-"))
                .OrderByDescending(j => j.EntryNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastEntry != null)
            {
                var lastSeq = lastEntry.EntryNumber.Split('-').LastOrDefault();
                if (int.TryParse(lastSeq, out var parsed)) nextNumber = parsed + 1;
            }

            var reversal = new JournalEntry
            {
                EntryNumber = $"JE-{year}-{nextNumber:D5}",
                PostingDate = DateTime.UtcNow,
                Description = $"Reversal of {original.EntryNumber} — {original.Description}",
                Reference = original.Reference,
                SourceModule = original.SourceModule,
                Status = JournalStatus.Posted,
                BranchId = original.BranchId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                PostedByUserId = userId,
                PostedAt = DateTime.UtcNow,
                ReversalOfJournalEntryId = original.Id,
                Lines = original.Lines.Select(l => new JournalEntryLine
                {
                    GlAccountId = l.GlAccountId,
                    Debit = l.Credit,     // swap
                    Credit = l.Debit,     // swap
                    Description = $"Reversal: {l.Description}",
                    CustomerId = l.CustomerId
                }).ToList()
            };

            original.Status = JournalStatus.Reversed;
            _uow.JournalEntries.Update(original);

            await _uow.JournalEntries.AddAsync(reversal);
            await _uow.SaveAsync();

            return await GetByIdAsync(reversal.Id);
        }

        public async Task<IEnumerable<TrialBalanceRowDto>> GetTrialBalanceAsync(
            DateTime fromDate, DateTime toDate, int? branchId = null)
        {
            var query = _uow.JournalEntryLines.Query()
                .Include(l => l.JournalEntry)
                .Include(l => l.GLAccount)
                .Where(l => l.JournalEntry.Status == JournalStatus.Posted
                         && l.JournalEntry.PostingDate >= fromDate
                         && l.JournalEntry.PostingDate <= toDate);

            if (branchId.HasValue)
                query = query.Where(l => l.JournalEntry.BranchId == branchId.Value);

            var lines = await query.ToListAsync();

            var rows = lines
                .GroupBy(l => new { l.GlAccountId, l.GLAccount.Code, l.GLAccount.Name, l.GLAccount.Type })
                .Select(g => new TrialBalanceRowDto
                {
                    GlAccountId = g.Key.GlAccountId,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    TypeName = g.Key.Type.ToString(),
                    TotalDebit = g.Sum(x => x.Debit),
                    TotalCredit = g.Sum(x => x.Credit),
                    Balance = g.Sum(x => x.Debit) - g.Sum(x => x.Credit)
                })
                .OrderBy(r => r.Code)
                .ToList();

            return rows;
        }

        private static JournalEntryDto MapToDto(JournalEntry j) => new()
        {
            Id = j.Id,
            EntryNumber = j.EntryNumber,
            PostingDate = j.PostingDate,
            Description = j.Description,
            Reference = j.Reference,
            SourceModule = j.SourceModule,
            Status = j.Status.ToString(),
            BranchId = j.BranchId,
            CreatedByUserId = j.CreatedByUserId,
            CreatedAt = j.CreatedAt,
            TotalDebit = j.Lines?.Sum(l => l.Debit) ?? 0,
            TotalCredit = j.Lines?.Sum(l => l.Credit) ?? 0,
            Lines = (j.Lines ?? new List<JournalEntryLine>()).Select(l => new JournalEntryLineDto
            {
                Id = l.Id,
                GlAccountId = l.GlAccountId,
                GlAccountCode = l.GLAccount?.Code ?? string.Empty,
                GlAccountName = l.GLAccount?.Name ?? string.Empty,
                Debit = l.Debit,
                Credit = l.Credit,
                Description = l.Description,
                CustomerId = l.CustomerId
            }).ToList()
        };
    }
}