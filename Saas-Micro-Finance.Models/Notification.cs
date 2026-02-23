using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationChannel Channel { get; set; } // email, sms, whatsapp
        public DateTime SentAt { get; set; } = DateTime.Now;
      
    }
    public enum NotificationChannel
    {
        Email =1,
        SMS =2,
        WhatsApp =3
    }
}
