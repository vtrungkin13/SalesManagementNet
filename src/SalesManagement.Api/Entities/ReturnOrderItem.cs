using System;

namespace SalesManagement.Api.Entities
{
    public class ReturnOrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public Guid ReturnOrderId { get; set; }
        public ReturnOrder? ReturnOrder { get; set; }

        public Guid VariantId { get; set; }
        public ProductVariant? Variant { get; set; }
    }
}
