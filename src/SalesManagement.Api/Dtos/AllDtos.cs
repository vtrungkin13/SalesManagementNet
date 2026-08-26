using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Api.Dtos
{
    // ===== Request DTOs =====
    public record RefreshTokenRequest([Required] string RefreshToken);
    public record LogoutRequest([Required] string RefreshToken);

    public record CreateTenantRequest([Required] string Code, [Required] string Name);

    public record CreateAppUserRequest(
        [Required][EmailAddress] string Email,
        [Required][MinLength(8)] string Password,
        [Required] string Name,
        [Required] string Phone,
        [Required] Guid TenantId,
        [Required] List<Guid> RolesId
    );

    public record UpdateAppUserRequest(
        [Required] Guid Id,
        [Required][EmailAddress] string Email,
        [Required] string Name,
        [Required] string Phone
    );

    public record AssignRolesRequest([Required] Guid Id, [Required] List<Guid> RolesId);

    public record CreateCategoryRequest(string Name);

    public record CreateCustomerRequest(
        [Required][MaxLength(100)] string Name,
        [Required][MaxLength(20)] string Phone,
        [EmailAddress][MaxLength(100)] string? Email,
        [MaxLength(255)] string? Address
    );

    public record CreateSupplierRequest(
        [Required][MaxLength(50)] string Code,
        [Required][MaxLength(100)] string Name,
        [Required][MaxLength(20)] string Phone,
        [EmailAddress][MaxLength(100)] string? Email,
        [MaxLength(255)] string? Address
    );

    public record CreateWarehouseRequest(
        [Required][MaxLength(50)] string Code,
        [Required][MaxLength(100)] string Name,
        [MaxLength(255)] string? Address
    );

    public record CreateProductRequest(
        [Required] string Code,
        [Required] string Name,
        string? Description,
        string? ImageUrl,
        [Required] Guid CategoryId,
        [Required] string Sku,
        double SellPrice,
        double CostPrice
    );

    public record CreateSalesOrderItemRequest(Guid VariantId, int Quantity, double Discount);
    public record CreateSalesOrderRequest(Guid CustomerId, Guid WarehouseId, List<CreateSalesOrderItemRequest> SalesOrderItemRequests);
    public record UpdateSalesOrderRequest(Guid? CustomerId, List<CreateSalesOrderItemRequest>? SalesOrderItemRequests);

    public record CreatePurchaseOrderItemRequest(Guid VariantId, int Quantity, double UnitCost);
    public record CreatePurchaseOrderRequest(Guid SupplierId, List<CreatePurchaseOrderItemRequest> Items);

    public record CreateGoodsReceiptItemRequest(Guid VariantId, int Quantity);
    public record CreateGoodsReceiptRequest(Guid PurchaseOrderId, Guid WarehouseId, List<CreateGoodsReceiptItemRequest> Items);

    // ===== Response DTOs =====
    public record TenantResponse(Guid Id, string Code, string Name, string Status, DateTime CreatedAt);

    public record AppUserResponse(Guid Id, string Email, string Name, string Phone, string Status, DateTime CreatedAt, string TenantName, List<string> RolesName);

    public record RoleResponse(Guid Id, string Name, string? Description);

    public record CategoryResponse(Guid Id, string Name, string TenantName);

    public record CustomerResponse(Guid Id, string Name, string Phone, string? Email, string? Address, double LoyaltyPoint, double TotalSpent, string TenantName);

    public record SupplierResponse(Guid Id, string Code, string Name, string Phone, string? Email, string? Address, string TenantName);

    public record WarehouseResponse(Guid Id, string Code, string Name, string? Address, string TenantName);

    public record ProductResponse(Guid Id, string Code, string Name, string? Description, string? ImageUrl, string CategoryName);

    public record ProductDetailResponse(Guid Id, string Code, string Name, string? Description, string? ImageUrl, string CategoryName, int CurrentInventory, double Price, string Sku);

    public record InventoryResponse(Guid InventoryId, Guid WarehouseId, string WarehouseName, Guid ProductId, string ProductName, Guid VariantId, string Sku, int Quantity, double CostPrice, double SellPrice);

    public record InventoryStatsResponse(long TotalItems, long TotalUniqueVariants, double TotalCostValue, double TotalSellValue, long OutOfStockCount, long LowStockCount);

    public record SalesOrderItemResponse(Guid Id, int Quantity, double UnitPrice, double Discount, Guid VariantId, string Sku);
    public record SalesOrderResponse(Guid Id, string OrderNumber, double Subtotal, double Discount, double Total, DateTime CreatedAt, string CustomerName, List<SalesOrderItemResponse> OrderItems);

    public record PurchaseOrderItemResponse(Guid Id, Guid VariantId, string Sku, int Quantity, double UnitCost);
    public record PurchaseOrderResponse(Guid Id, string PoNumber, string Status, double Amount, DateTime CreatedAt, string SupplierName, List<PurchaseOrderItemResponse> Items);

    public record GoodsReceiptItemResponse(Guid Id, Guid VariantId, string Sku, int Quantity);
    public record GoodsReceiptResponse(Guid Id, string ReceiptNumber, DateTime ReceiptDate, Guid PurchaseOrderId, string PoNumber, string WarehouseName, List<GoodsReceiptItemResponse> Items);

    public record ImportJobErrorResponse(long Id, int RowNumber, string? ErrorMessage, string? RawData);
    public record ImportJobResponse(Guid Id, string Status, int TotalRows, int ProcessedOffset, double ProgressPercentage, DateTime CreatedAt, DateTime UpdatedAt);
}
