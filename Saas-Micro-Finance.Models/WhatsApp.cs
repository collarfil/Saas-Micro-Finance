using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models
{
    public class WhatsApp
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string  Phone { get; set; }
        public string TemplateName { get; set; }
        public string Payload { get; set; }
        public string Status { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;

    }
    public enum WhatsAppStatus
    {
        Sent =1,
        Delivered =2,
        Failed =3
    }
}
