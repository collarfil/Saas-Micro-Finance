using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.Utility.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int tenantId, string title, string message, NotificationChannel channel);
    }
}
