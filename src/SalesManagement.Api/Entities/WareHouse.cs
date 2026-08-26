using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class WareHouse : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Address { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
