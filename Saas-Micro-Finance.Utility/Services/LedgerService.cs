using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services
{
    public class LedgerService : ILedgerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LedgerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> SubmitTransactionAsync(List<LedgerEntryDto> entries)
        {
            if (entries == null || entries.Count < 2)
                return "Transaction must have at least 2 entries.";

            var totalDebit = entries
                .Where(e => e.EntryType == EntryType.Debit)
                .Sum(e => e.Amount);

            var totalCredit = entries
                .Where(e => e.EntryType == EntryType.Credit)
                .Sum(e => e.Amount);

            if (totalDebit != totalCredit)
                return "Debit and Credit must balance.";

            var transaction = new Transaction
            {
                Status = TransactionStatus.Pending,
                Created_At = DateTime.UtcNow
            };

            _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveAsync();

            var ledgerEntries = entries.Select(e => new LedgerEntry
            {
                
                TransactionId = transaction.Id,
                AccountId = e.AccountId,
                Amount = e.Amount,
                EntryType = e.EntryType,
                CreatedAt = DateTime.UtcNow
            });

            _unitOfWork.LedgerEntries.AddRangeAsync(ledgerEntries);
            await _unitOfWork.SaveAsync();

            return "Transaction submitted for approval.";
        }

        public async Task<string> ApproveTransactionAsync(int transactionId)
        {
            var transaction = await _unitOfWork.Transactions
                .GetFirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return "Transaction not found.";

            if (transaction.Status != TransactionStatus.Pending)
                return "Only pending transactions can be approved.";

            transaction.Status = TransactionStatus.Approved;

            await _unitOfWork.SaveAsync();

            return "Transaction approved and posted to ledger.";
        }

        public async Task<string> RejectTransactionAsync(int transactionId)
        {
            var transaction = await _unitOfWork.Transactions
                .GetFirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return "Transaction not found.";

            if (transaction.Status != TransactionStatus.Pending)
                return "Only pending transactions can be rejected.";

            transaction.Status = TransactionStatus.Rejected;

            await _unitOfWork.SaveAsync();

            return "Transaction rejected.";
        }

        public async Task<List<LedgerEntryDto>> GetLedgerEntriesAsync(int accountId)
        {
            return await _unitOfWork.LedgerEntries
                .Query()
                .Where(l => l.AccountId == accountId)
                .Select(l => new LedgerEntryDto
                {
                    Id = l.Id,
                    
                    TransactionId = l.TransactionId,
                    AccountId = l.AccountId,
                    Amount = l.Amount,
                    EntryType = l.EntryType,
                    CreatedAt = l.CreatedAt
                }).ToListAsync();
        }
    }
}
