
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saas_Micro_Finance.Models
{
    public class GLAccount
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        public GLAccountType Type { get; set; }

        // Self-referencing hierarchy
        public int? ParentId { get; set; }

        public GLAccount? Parent { get; set; }

        public ICollection<GLAccount> Children { get; set; }
            = new List<GLAccount>();

        public bool IsActive { get; set; } = true;
    }

    public enum GLAccountType
    {
        Asset = 1,
        Liability = 2,
        Equity = 3,
        Income = 4,
        Expense = 5
    }
}