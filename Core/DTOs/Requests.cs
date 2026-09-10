namespace Core.DTOs;

public record RegisterRequest(string Email, string Password, string FullName, string TenantCode, string TenantName);
public record LoginRequest(string Email, string Password);
public record RefreshRequest(string RefreshToken);
public record TokenResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);
public record CreateTenantRequest(string Code, string Name);
public record CreateUserRequest(string Email, string Password, string FullName, string? Phone, string Role = "USER");
public record CreateCategoryRequest(string Code, string Name, string? Description);
public record CreateProductRequest(string Code, string Name, Guid CategoryId, string? Description, decimal CostPrice, decimal SellingPrice, string? Sku);
public record CreatePartnerRequest(string Code, string Name, string? Phone, string? Email, string? Address);
public record CreateWarehouseRequest(string Code, string Name, string? Address);
public record StockAdjustmentRequest(Guid WarehouseId, Guid ProductVariantId, decimal Quantity, string? Note);
public record PurchaseItemRequest(Guid ProductVariantId, decimal Quantity, decimal UnitPrice, decimal Discount = 0, decimal Tax = 0);
public record CreatePurchaseRequest(Guid SupplierId, Guid WarehouseId, List<PurchaseItemRequest> Items, string? Note);
public record ReceiptItemRequest(Guid ProductVariantId, decimal Quantity, decimal UnitCost);
public record CreateReceiptRequest(Guid PurchaseOrderId, Guid WarehouseId, List<ReceiptItemRequest> Items, string? Note);
public record SalesItemRequest(Guid ProductVariantId, decimal Quantity, decimal UnitPrice, decimal Discount = 0, decimal Tax = 0);
public record CreateSalesRequest(Guid CustomerId, Guid WarehouseId, List<SalesItemRequest> Items, string? Note);
public record ApiError(string Code, string Message);


