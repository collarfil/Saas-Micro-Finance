using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class RegisterTenantAdminDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        // Tenant fields
        [Required]
        public string TenantName { get; set; }

        public string LicenseNumber { get; set; }

        public string CBNCode { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
