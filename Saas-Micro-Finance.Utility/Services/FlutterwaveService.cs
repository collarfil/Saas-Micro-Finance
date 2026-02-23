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
    public class FlutterwaveService : IFlutterwaveService
    {
        private readonly SaasBankDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FlutterwaveService(SaasBankDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetTenantId() => int.Parse(_httpContextAccessor.HttpContext.Items["TenantId"].ToString());

        public async Task<PaymentDto> InitializePaymentAsync(CreatePaymentDto dto)
        {
            var reference = Guid.NewGuid().ToString("N"); // Simulated

            var payment = new Flutterwave
            {
                SubscriptionId = dto.SubscriptionId,
                TenantId = GetTenantId(),
                Amount = dto.Amount,
                TransactionReference = reference,
                IsSuccessful = false
            };

            _context.Flutterwaves.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentDto
            {
                Id = payment.Id,
                SubscriptionId = payment.SubscriptionId,
                Amount = payment.Amount,
                TransactionReference = payment.TransactionReference,
                IsSuccessful = payment.IsSuccessful,
                CreatedAt = payment.CreatedAt
            };
        }

        public async Task<PaymentDto> ConfirmPaymentAsync(string transactionReference, bool isSuccess)
        {
            var payment = await _context.Flutterwaves
                .FirstOrDefaultAsync(p => p.TransactionReference == transactionReference);

            if (payment == null) return null;

            payment.IsSuccessful = isSuccess;
            await _context.SaveChangesAsync();

            return new PaymentDto
            {
                Id = payment.Id,
                SubscriptionId = payment.SubscriptionId,
                Amount = payment.Amount,
                TransactionReference = payment.TransactionReference,
                IsSuccessful = payment.IsSuccessful,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}