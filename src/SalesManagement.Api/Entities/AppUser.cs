using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class AppUser : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public UserStatus Status { get; set; } = UserStatus.ACTIVE;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
