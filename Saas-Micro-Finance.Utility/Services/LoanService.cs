using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.Utility.Services
{
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> ApplyLoanAsync(int tenantId, int customerId, int loanProductId, decimal principal)
        {
            var product = await _unitOfWork.LoanProducts
                .GetFirstOrDefaultAsync(x => x.Id == loanProductId && x.TenantId == tenantId);

            if (product == null)
                throw new Exception("Invalid loan product.");

            var interest = principal * product.InterestRate / 100;

            var loan = new Loan
            {
                TenantId = tenantId,
                LoanProductId = loanProductId,
                Principal = principal,
                Interest = interest,
                Status = LoanStatus.Pending
            };

            await _unitOfWork.Loans.AddAsync(loan);
            await _unitOfWork.SaveAsync();

            return "Loan application submitted.";
        }

        public async Task<string> ApproveLoanAsync(int loanId)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);

            if (loan == null)
                throw new Exception("Loan not found.");

            loan.Status = LoanStatus.Approved;

            await _unitOfWork.SaveAsync();

            return "Loan approved.";
        }

        public async Task<string> DisburseLoanAsync(int loanId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);

                if (loan == null || loan.Status != LoanStatus.Approved)
                    throw new Exception("Loan not approved.");

                loan.Status = LoanStatus.Disbursed;
                loan.DisbursedAt = DateTime.UtcNow;

                // Ledger entry logic here
                // Debit: Loan Receivable
                // Credit: Customer Account

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return "Loan disbursed.";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<string> RepayLoanAsync(int loanId, decimal amount)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);

                if (loan == null)
                    throw new Exception("Loan not found.");

                var repayment = new LoanRepayment
                {
                    TenantId = loan.TenantId,
                    LoanId = loan.Id,
                    Amount = amount,
                    PaidAt = DateTime.UtcNow,
                    Status = RepaymentStatus.Completed
                };

                await _unitOfWork.LoanRepayments.AddAsync(repayment);

                // Ledger entry for repayment

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return "Loan repayment successful.";
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
