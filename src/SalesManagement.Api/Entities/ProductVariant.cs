using System;
using System.Collections.Generic;

namespace SalesManagement.Api.Entities
{
    public class ProductVariant : IMustHaveTenant
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = null!;
        public string? Barcode { get; set; }
        public double CostPrice { get; set; }
        public double SellPrice { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.ACTIVE;

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();
        public ICollection<ReturnOrderItem> ReturnOrderItems { get; set; } = new List<ReturnOrderItem>();
    }
}
