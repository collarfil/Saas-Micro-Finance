using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Models.DTOs
{
    public class GLAccountDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public GLAccountType Type { get; set; }

        public string TypeName => Type.ToString();

        public int? ParentId { get; set; }

        public string? ParentName { get; set; }

        public bool IsActive { get; set; }
    }
}
