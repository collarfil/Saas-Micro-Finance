using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.Utility.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> DepositAsync(int tenantId, int accountId, decimal amount, string reference, string narration)
        {
            if (amount <= 0)
                throw new Exception("Invalid deposit amount.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var account = await _unitOfWork.Accounts
                    .GetFirstOrDefaultAsync(x => x.Id == accountId && x.TenantId == tenantId);

                if (account == null)
                    throw new Exception("Account not found.");

                // Create transaction record
                var transaction = new Transaction
                {
                    TenantId = tenantId,
                    AccountId = accountId,
                    Type = TransactionType.Credit,
                    Amount = amount,
                    Reference = reference,
                    Narration = narration,
                    Created_At = DateTime.UtcNow
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveAsync();

                // Ledger Entries (Debit & Credit)
                var debitEntry = new LedgerEntry
                {
                    TenantId = tenantId,
                    TransactionId = transaction.Id,
                    AccountId = accountId,
                    Amount = amount,
                    EntryType = EntryType.Credit
                };

                await _unitOfWork.LedgerEntries.AddAsync(debitEntry);

                // Update balance (Hybrid strategy C)
                account.Balance += amount;
                _unitOfWork.Accounts.Update(account);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return "Deposit successful";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<string> WithdrawAsync(int tenantId, int accountId, decimal amount, string reference, string narration)
        {
            if (amount <= 0)
                throw new Exception("Invalid withdrawal amount.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var account = await _unitOfWork.Accounts
                    .GetFirstOrDefaultAsync(x => x.Id == accountId && x.TenantId == tenantId);

                if (account == null)
                    throw new Exception("Account not found.");

                if (account.Balance < amount)
                    throw new Exception("Insufficient balance.");

                var transaction = new Transaction
                {
                    TenantId = tenantId,
                    AccountId = accountId,
                    Type = TransactionType.Debit,
                    Amount = amount,
                    Reference = reference,
                    Narration = narration,
                    Created_At = DateTime.UtcNow
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveAsync();

                var ledgerEntry = new LedgerEntry
                {
                    TenantId = tenantId,
                    TransactionId = transaction.Id,
                    AccountId = accountId,
                    Amount = amount,
                    EntryType = EntryType.Debit
                };

                await _unitOfWork.LedgerEntries.AddAsync(ledgerEntry);

                account.Balance -= amount;
                _unitOfWork.Accounts.Update(account);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return "Withdrawal successful";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<string> TransferAsync(int tenantId, int fromAccountId, int toAccountId, decimal amount, string reference, string narration)
        {
            if (amount <= 0)
                throw new Exception("Invalid transfer amount.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var fromAccount = await _unitOfWork.Accounts
                    .GetFirstOrDefaultAsync(x => x.Id == fromAccountId && x.TenantId == tenantId);

                var toAccount = await _unitOfWork.Accounts
                    .GetFirstOrDefaultAsync(x => x.Id == toAccountId && x.TenantId == tenantId);

                if (fromAccount == null || toAccount == null)
                    throw new Exception("Invalid accounts.");

                if (fromAccount.Balance < amount)
                    throw new Exception("Insufficient balance.");

                var transaction = new Transaction
                {
                    TenantId = tenantId,
                    AccountId = fromAccountId,
                    Type = TransactionType.Debit,
                    Amount = amount,
                    Reference = reference,
                    Narration = narration,
                    Created_At = DateTime.UtcNow
                };

                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveAsync();

                await _unitOfWork.LedgerEntries.AddAsync(new LedgerEntry
                {
                    TenantId = tenantId,
                    TransactionId = transaction.Id,
                    AccountId = fromAccountId,
                    Amount = amount,
                    EntryType = EntryType.Debit
                });

                await _unitOfWork.LedgerEntries.AddAsync(new LedgerEntry
                {
                    TenantId = tenantId,
                    TransactionId = transaction.Id,
                    AccountId = toAccountId,
                    Amount = amount,
                    EntryType = EntryType.Credit
                });

                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                _unitOfWork.Accounts.Update(fromAccount);
                _unitOfWork.Accounts.Update(toAccount);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return "Transfer successful";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
