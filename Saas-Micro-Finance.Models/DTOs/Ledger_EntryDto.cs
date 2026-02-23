using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class Ledger_EntryDto
    {
            public int Id { get; set; }

            public int TenantId { get; set; }
            

            public int TransactionId { get; set; }
            

            public int AccountId { get; set; }
            

            public decimal Amount { get; set; }

            public EntryType EntryType { get; set; } // Debit or Credit

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        

    }
