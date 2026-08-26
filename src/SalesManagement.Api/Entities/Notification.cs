using System;

namespace SalesManagement.Api.Entities
{
    public class Notification : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; } = false;

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }
    }
}
