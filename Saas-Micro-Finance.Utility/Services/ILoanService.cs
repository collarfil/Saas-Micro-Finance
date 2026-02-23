using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services
{
    public interface ILoanService
    {
        Task<string> ApplyLoanAsync(int tenantId, int customerId, int loanProductId, decimal principal);

        Task<string> ApproveLoanAsync(int loanId);

        Task<string> DisburseLoanAsync(int loanId);

        Task<string> RepayLoanAsync(int loanId, decimal amount);
    }
}
