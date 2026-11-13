using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class GLAccountCreateDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(GLAccountType))]
        public GLAccountType Type { get; set; }

        // Null means this is a top-level account.
        public int? ParentId { get; set; }
    }
}
