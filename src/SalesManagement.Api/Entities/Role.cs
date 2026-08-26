using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
