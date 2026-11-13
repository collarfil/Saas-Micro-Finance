using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Micro_Finance.DataAccess.Data;
using Saas_Micro_Finance.Models;
using Saas_Micro_Finance.Models.DTOs;
using Saas_Micro_Finance.Utility.Services.Interface;

namespace Saas_Micro_Finance.Utility.Services
{
    public class FlutterwaveService : IFlutterwaveService
    {
        private readonly SaasBankDbContext _context;

        public FlutterwaveService(SaasBankDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentDto> InitializePaymentAsync(CreatePaymentDto dto)
        {
            var reference = Guid.NewGuid().ToString("N"); // Simulated reference

            var payment = new Flutterwave
            {
                SubscriptionId = dto.SubscriptionId,
                Amount = dto.Amount,
                Currency = "NGN", // Set default currency
                PaymentStatus = "Pending", // <-- FIX: Added PaymentStatus to prevent null constraint violation
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
            payment.PaymentStatus = isSuccess ? "Successful" : "Failed";

            // ✅ UNLOCK THE SUBSCRIPTION ON SUCCESS
            if (isSuccess && payment.SubscriptionId > 0)
            {
                var sub = await _context.Subscriptions.FindAsync(payment.SubscriptionId);
                if (sub != null)
                {
                    sub.IsActive = true;
                }
            }

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