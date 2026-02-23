using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationChannel Channel { get; set; } 
        public DateTime SentAt { get; set; }
    }
    
}
