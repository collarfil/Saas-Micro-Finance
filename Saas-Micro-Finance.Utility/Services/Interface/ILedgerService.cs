using Saas_Micro_Finance.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services.Interface
{
    public interface ILedgerService
    {
        Task<string> SubmitTransactionAsync(List<LedgerEntryDto> entries);
        Task<string> ApproveTransactionAsync(int transactionId);
        Task<string> RejectTransactionAsync(int transactionId);
        Task<List<LedgerEntryDto>> GetLedgerEntriesAsync(int accountId);
    }
}
