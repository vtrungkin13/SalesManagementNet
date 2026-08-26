using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesManagement.Api.Data;
using SalesManagement.Api.Dtos;
using SalesManagement.Api.Entities;
using SalesManagement.Api.Security;

namespace SalesManagement.Api.Services
{
    public class TenantService
    {
        private readonly SalesDbContext _db;

        public TenantService(SalesDbContext db) { _db = db; }

        public async Task<TenantResponse> CreateTenantAsync(CreateTenantRequest request)
        {
            var tenant = new Tenant { Code = request.Code, Name = request.Name };
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();
            return ToResponse(tenant);
        }

        public async Task<List<TenantResponse>> GetAllTenantsAsync()
        {
            return await _db.Tenants.Select(t => ToResponse(t)).ToListAsync();
        }

        private static TenantResponse ToResponse(Tenant t) =>
            new(t.Id, t.Code, t.Name, t.Status.ToString(), t.CreatedAt);
    }

    public class ProductService
    {
        private readonly SalesDbContext _db;
        private readonly TenantContext _tenantCtx;

        public ProductService(SalesDbContext db, TenantContext tenantCtx)
        {
            _db = db;
            _tenantCtx = tenantCtx;
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest req)
        {
            var tenantId = _tenantCtx.TenantId ?? throw new InvalidOperationException("Tenant Context not found");
            var tenant = await _db.Tenants.FindAsync(tenantId) ?? throw new InvalidOperationException("Tenant not found");
            var category = await _db.Categories.FindAsync(req.CategoryId) ?? throw new InvalidOperationException("Category not found");

            var product = new Product
            {
                Code = req.Code, Name = req.Name, Description = req.Description,
                CategoryId = req.CategoryId, TenantId = tenantId
            };
            _db.Products.Add(product);

            if (!string.IsNullOrEmpty(req.ImageUrl))
            {
                _db.ProductImages.Add(new ProductImage { ImageUrl = req.ImageUrl, ProductId = product.Id, Product = product });
            }

            var variant = new ProductVariant
            {
                Sku = req.Sku, SellPrice = req.SellPrice, CostPrice = req.CostPrice,
                ProductId = product.Id, TenantId = tenantId
            };
            _db.ProductVariants.Add(variant);
            await _db.SaveChangesAsync();

            return new ProductResponse(product.Id, product.Code, product.Name, product.Description, req.ImageUrl, category.Name);
        }

        public async Task<List<ProductResponse>> ImportProductsAsync(List<CreateProductRequest> requests)
        {
            var results = new List<ProductResponse>();
            foreach (var req in requests)
            {
                results.Add(await CreateProductAsync(req));
            }
            return results;
        }
    }

    public class SalesOrderService
    {
        private readonly SalesDbContext _db;
        private readonly TenantContext _tenantCtx;

        public SalesOrderService(SalesDbContext db, TenantContext tenantCtx) { _db = db; _tenantCtx = tenantCtx; }

        public async Task<SalesOrderResponse> CreateSalesOrderAsync(CreateSalesOrderRequest request)
        {
            var tenantId = _tenantCtx.TenantId ?? throw new InvalidOperationException("Tenant Context not found");

            var customer = await _db.Customers.FindAsync(request.CustomerId)
                ?? throw new InvalidOperationException("Customer not found");

            var salesOrder = new SalesOrder
            {
                OrderNumber = "SO-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CustomerId = request.CustomerId,
                TenantId = tenantId
            };

            var items = new List<SalesOrderItem>();
            double subtotal = 0, discount = 0;

            foreach (var itemReq in request.SalesOrderItemRequests)
            {
                var variant = await _db.ProductVariants.FindAsync(itemReq.VariantId)
                    ?? throw new InvalidOperationException("Product variant not found");

                var inventory = await _db.Inventories
                    .FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.VariantId == itemReq.VariantId)
                    ?? throw new InvalidOperationException($"Sản phẩm SKU {variant.Sku} không có tồn kho tại kho được chọn");

                if (inventory.Quantity < itemReq.Quantity)
                    throw new InvalidOperationException($"SKU {variant.Sku} không đủ hàng. Kho: {inventory.Quantity}, YC: {itemReq.Quantity}");

                inventory.Quantity -= itemReq.Quantity;

                _db.InventoryTransactions.Add(new InventoryTransaction
                {
                    TransactionType = TransactionType.OUT,
                    Quantity = itemReq.Quantity,
                    InventoryId = inventory.Id,
                    ReferenceId = salesOrder.Id
                });

                var item = new SalesOrderItem
                {
                    VariantId = itemReq.VariantId,
                    UnitPrice = variant.SellPrice,
                    Quantity = itemReq.Quantity,
                    Discount = itemReq.Discount,
                    SalesOrderId = salesOrder.Id
                };
                items.Add(item);
                subtotal += item.UnitPrice * item.Quantity;
                discount += item.Discount;
            }

            salesOrder.Subtotal = subtotal;
            salesOrder.Discount = discount;
            salesOrder.Total = Math.Max(0.0, subtotal - discount);

            _db.SalesOrders.Add(salesOrder);
            _db.SalesOrderItems.AddRange(items);
            await _db.SaveChangesAsync();

            return await GetSalesOrderByIdAsync(salesOrder.Id);
        }

        public async Task<SalesOrderResponse> GetSalesOrderByIdAsync(Guid id)
        {
            var order = await _db.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.SalesOrderItems).ThenInclude(i => i.Variant)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new InvalidOperationException("Order not found");

            return MapToResponse(order);
        }

        public async Task DeleteSalesOrderAsync(Guid id)
        {
            var order = await _db.SalesOrders
                .Include(o => o.SalesOrderItems)
                .FirstOrDefaultAsync(o => o.Id == id)
                ?? throw new InvalidOperationException("Order not found");

            foreach (var item in order.SalesOrderItems)
            {
                var inventories = await _db.Inventories.Where(i => i.VariantId == item.VariantId).ToListAsync();
                if (inventories.Any())
                {
                    inventories.First().Quantity += item.Quantity;
                }
            }

            var txns = await _db.InventoryTransactions.Where(t => t.ReferenceId == id).ToListAsync();
            _db.InventoryTransactions.RemoveRange(txns);
            _db.SalesOrders.Remove(order);
            await _db.SaveChangesAsync();
        }

        private static SalesOrderResponse MapToResponse(SalesOrder o) =>
            new(o.Id, o.OrderNumber, o.Subtotal, o.Discount, o.Total, o.CreatedAt,
                o.Customer?.Name ?? "", o.SalesOrderItems.Select(i =>
                    new SalesOrderItemResponse(i.Id, i.Quantity, i.UnitPrice, i.Discount, i.VariantId, i.Variant?.Sku ?? "")
                ).ToList());
    }
}
