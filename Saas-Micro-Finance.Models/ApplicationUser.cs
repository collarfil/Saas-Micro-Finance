using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Saas_Micro_Finance.Models
{
    public class ApplicationUser: IdentityUser
    {
        public int TenantId { get; set; }
    }
}
