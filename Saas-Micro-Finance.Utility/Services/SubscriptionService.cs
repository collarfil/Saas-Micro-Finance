using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;

namespace Saas_Micro_Finance.Utility.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SaasBankDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubscriptionService(SaasBankDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetTenantId() => int.Parse(_httpContextAccessor.HttpContext.Items["TenantId"].ToString());

        public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto dto)
        {
            var subscription = new Subscription
            {
                TenantId = GetTenantId(),
                PlanName = dto.PlanName,
                Amount = dto.Amount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return new SubscriptionDto
            {
                Id = subscription.Id,
                PlanName = subscription.PlanName,
                Amount = subscription.Amount,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                IsActive = subscription.IsActive
            };
        }

        public async Task<List<SubscriptionDto>> GetSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Select(s => new SubscriptionDto
                {
                    Id = s.Id,
                    PlanName = s.PlanName,
                    Amount = s.Amount,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<SubscriptionDto> GetByIdAsync(int id)
        {
            var s = await _context.Subscriptions.FindAsync(id);
            if (s == null) return null;

            return new SubscriptionDto
            {
                Id = s.Id,
                PlanName = s.PlanName,
                Amount = s.Amount,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = s.IsActive
            };
        }

        public async Task<bool> CancelSubscriptionAsync(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null) return false;

            subscription.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
