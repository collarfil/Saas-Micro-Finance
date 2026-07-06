using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saas_Micro_Finance.Models;

namespace Saas_Micro_Finance.Utility.Services.Interface
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string title, string message, NotificationChannel channel);
    }
}
