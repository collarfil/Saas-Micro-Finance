using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.Utility.Services
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto);
        Task<List<SubscriptionDto>> GetSubscriptionsAsync();
        Task<SubscriptionDto> GetByIdAsync(int id);
        Task<bool> CancelSubscriptionAsync(int id);
    }
}
