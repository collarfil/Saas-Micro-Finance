using Saas_Micro_Finance.DataAccess.Repository.IRepository;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Saas_Micro_Finance.Utility.Services
{
    public class GLAccountService : IGLAccountService
    {
        private readonly IUnitOfWork _uow;

        public GLAccountService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<GLAccountDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = _uow.GLAccounts.Query();

            if (!includeInactive)
                query = query.Where(g => g.IsActive);

            var accounts = await query
                .Include(g => g.Parent)
                .OrderBy(g => g.Code)
                .ToListAsync();

            return accounts.Select(MapToDto).ToList();
        }

        public async Task<GLAccountDto?> GetByIdAsync(int id)
        {
            var account = await _uow.GLAccounts.Query()
                .Include(g => g.Parent)
                .FirstOrDefaultAsync(g => g.Id == id);

            return account == null ? null : MapToDto(account);
        }

        public async Task<GLAccountDto> CreateAsync(GLAccountCreateDto dto)
        {
            var existing = await _uow.GLAccounts
                .GetFirstOrDefaultAsync(g => g.Code == dto.Code);
            if (existing != null)
                throw new InvalidOperationException($"GL Account code '{dto.Code}' already exists.");

            if (dto.ParentId.HasValue)
            {
                var parent = await _uow.GLAccounts.GetByIdAsync(dto.ParentId.Value);
                if (parent == null)
                    throw new InvalidOperationException($"Parent GL Account {dto.ParentId} does not exist.");
                if (parent.Type != dto.Type)
                    throw new InvalidOperationException("Child account type must match parent account type.");
            }

            var account = new GLAccount
            {
                Name = dto.Name.Trim(),
                Code = dto.Code.Trim(),
                Type = dto.Type,
                ParentId = dto.ParentId,
                IsActive = true
            };

            await _uow.GLAccounts.AddAsync(account);
            await _uow.SaveAsync();

            return await GetByIdAsync(account.Id)
                   ?? throw new InvalidOperationException("Created account could not be reloaded.");
        }

        public async Task<GLAccountDto?> UpdateAsync(int id, GLAccountUpdateDto dto)
        {
            var account = await _uow.GLAccounts.GetByIdAsync(id);
            if (account == null) return null;

            var codeCollision = await _uow.GLAccounts
                .GetFirstOrDefaultAsync(g => g.Code == dto.Code && g.Id != id);
            if (codeCollision != null)
                throw new InvalidOperationException($"GL Account code '{dto.Code}' already exists.");

            if (dto.ParentId == id)
                throw new InvalidOperationException("An account cannot be its own parent.");

            if (dto.ParentId.HasValue)
            {
                var parent = await _uow.GLAccounts.GetByIdAsync(dto.ParentId.Value);
                if (parent == null)
                    throw new InvalidOperationException($"Parent GL Account {dto.ParentId} does not exist.");
                if (parent.Type != dto.Type)
                    throw new InvalidOperationException("Child account type must match parent account type.");
                if (parent.ParentId == id)
                    throw new InvalidOperationException("Cannot set parent — would create a cycle.");
            }

            account.Name = dto.Name.Trim();
            account.Code = dto.Code.Trim();
            account.Type = dto.Type;
            account.ParentId = dto.ParentId;
            account.IsActive = dto.IsActive;

            _uow.GLAccounts.Update(account);
            await _uow.SaveAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _uow.GLAccounts.GetByIdAsync(id);
            if (account == null) return false;

            account.IsActive = false;
            _uow.GLAccounts.Update(account);
            await _uow.SaveAsync();
            return true;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var account = await _uow.GLAccounts.Query()
                .Include(g => g.Children)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (account == null) return false;

            if (account.Children.Any())
                throw new InvalidOperationException("Cannot delete an account that has child accounts.");

            var hasLines = await _uow.JournalEntryLines
                .GetFirstOrDefaultAsync(l => l.GlAccountId == id);
            if (hasLines != null)
                throw new InvalidOperationException("Cannot delete an account that has journal entry lines.");

            _uow.GLAccounts.Remove(account);
            await _uow.SaveAsync();
            return true;
        }

        private static GLAccountDto MapToDto(GLAccount g) => new()
        {
            Id = g.Id,
            Name = g.Name,
            Code = g.Code,
            Type = g.Type,
            ParentId = g.ParentId,
            ParentName = g.Parent?.Name,
            IsActive = g.IsActive
        };
    }
}
