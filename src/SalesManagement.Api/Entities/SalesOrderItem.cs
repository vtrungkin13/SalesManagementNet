using System;

namespace SalesManagement.Api.Entities
{
    public class SalesOrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Discount { get; set; }

        public Guid SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }

        public Guid VariantId { get; set; }
        public ProductVariant? Variant { get; set; }
    }
}
