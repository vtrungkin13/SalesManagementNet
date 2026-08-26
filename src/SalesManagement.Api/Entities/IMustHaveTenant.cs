using System;

namespace SalesManagement.Api.Entities
{
    public interface IMustHaveTenant
    {
        Guid TenantId { get; set; }
    }
}
