using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public Guid WarehouseId { get; set; }
        public WareHouse? Warehouse { get; set; }

        public Guid VariantId { get; set; }
        public ProductVariant? Variant { get; set; }

        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }
}
