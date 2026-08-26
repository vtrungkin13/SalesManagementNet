using System;

namespace SalesManagement.Api.Security
{
    public class TenantContext
    {
        public Guid? TenantId { get; set; }
    }
}
