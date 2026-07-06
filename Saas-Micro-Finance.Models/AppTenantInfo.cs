using Finbuckle.MultiTenant;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saas_Micro_Finance.Models
{
    public class AppTenantInfo : ITenantInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty; // e.g., "firstbank"
        public string Name { get; set; } = string.Empty;
        public string? ConnectionString { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? DatabaseName { get; set; }
    }
}