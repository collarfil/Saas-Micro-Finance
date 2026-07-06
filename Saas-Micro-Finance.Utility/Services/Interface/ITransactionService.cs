using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services.Interface
{
    public interface ITransactionService
    {
        Task<string> DepositAsync(int accountId, decimal amount, string reference, string narration, string channel);

        Task<string> WithdrawAsync(int TenantId, int accountId, decimal amount, string reference, string narration, string channel);

        Task<string> TransferAsync(int TenantId, int fromAccountId, int toAccountId, decimal amount, string reference, string narration, string channel);
    }
}
