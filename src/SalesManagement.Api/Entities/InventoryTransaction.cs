using System;

namespace SalesManagement.Api.Entities
{
    public class InventoryTransaction
    {
        public Guid Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }

        public Guid InventoryId { get; set; }
        public Inventory? Inventory { get; set; }

        public Guid ReferenceId { get; set; }
    }
}
