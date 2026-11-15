using Saas_Micro_Finance.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saas_Micro_Finance.Utility.Services.Interface
{
    public interface IGLAccountService
    {
        Task<IEnumerable<GLAccountDto>> GetAllAsync(bool includeInactive = false);
        Task<GLAccountDto?> GetByIdAsync(int id);
        Task<GLAccountDto> CreateAsync(GLAccountCreateDto dto);
        Task<GLAccountDto?> UpdateAsync(int id, GLAccountUpdateDto dto);
        Task<bool> DeleteAsync(int id);       // soft: IsActive = false
        Task<bool> HardDeleteAsync(int id);   // guarded: no children, no lines
    }
}
