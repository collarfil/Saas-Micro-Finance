using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services.Interface
{
    public interface IWhatsAppService
    {
        Task SendWhatsAppAsync(int TenantId, int customerId, string template, string payload);
    }
}
