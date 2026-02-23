using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services
{
    public interface ITransactionService
    {
        Task<string> DepositAsync(int tenantId, int accountId, decimal amount, string reference, string narration);

        Task<string> WithdrawAsync(int tenantId, int accountId, decimal amount, string reference, string narration);

        Task<string> TransferAsync(int tenantId, int fromAccountId, int toAccountId, decimal amount, string reference, string narration);
    }
}
