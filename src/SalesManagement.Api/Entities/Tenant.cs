using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public TenantStatus Status { get; set; } = TenantStatus.ACTIVE;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
