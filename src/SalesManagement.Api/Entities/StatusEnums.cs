namespace SalesManagement.Api.Entities
{
    public enum TenantStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum UserStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum ProductStatus
    {
        ACTIVE,
        INACTIVE
    }

    public enum TransactionType
    {
        IN,
        OUT
    }

    public enum PurchaseStatus
    {
        PENDING,
        APPROVED,
        DELIVERY,
        RECEIVED,
        CANCELLED
    }

    public enum ImportJobStatus
    {
        PENDING,
        PROCESSING,
        COMPLETED,
        FAILED,
        CANCELLED
    }
}
