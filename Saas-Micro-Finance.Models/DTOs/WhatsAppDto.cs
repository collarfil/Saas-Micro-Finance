using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class WhatsAppDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        
        public int CustomerId { get; set; }
        
        public string Phone { get; set; }
        public string TemplateName { get; set; }
        public string Payload { get; set; }
        public WhatsAppStatus Status { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
    
}
