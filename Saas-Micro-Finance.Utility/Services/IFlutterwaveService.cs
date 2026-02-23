using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.Utility.Services
{
    public interface IFlutterwaveService
    {
        Task<PaymentDto> InitializePaymentAsync(CreatePaymentDto dto);
        Task<PaymentDto> ConfirmPaymentAsync(string transactionReference, bool isSuccess);
    }
}
