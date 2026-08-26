using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class GoodsReceipt : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string ReceiptNumber { get; set; } = null!;
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        public Guid WarehouseId { get; set; }
        public WareHouse? Warehouse { get; set; }

        public ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();
    }
}
