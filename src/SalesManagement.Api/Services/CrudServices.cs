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
    public class CrudServices
    {
        private readonly SalesDbContext _db;
        private readonly TenantContext _tenantCtx;

        public CrudServices(SalesDbContext db, TenantContext tenantCtx)
        {
            _db = db;
            _tenantCtx = tenantCtx;
        }

        // ===== Category =====
        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest req)
        {
            var tenantId = _tenantCtx.TenantId ?? throw new InvalidOperationException("Tenant Context not found");
            var tenant = await _db.Tenants.FindAsync(tenantId) ?? throw new InvalidOperationException("Tenant not found");
            var cat = new Category { Name = req.Name, TenantId = tenantId };
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();
            return new CategoryResponse(cat.Id, cat.Name, tenant.Name);
        }

        public async Task<List<CategoryResponse>> GetCategoriesAsync()
        {
            return await _db.Categories.Include(c => c.Tenant)
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Tenant != null ? c.Tenant.Name : "")).ToListAsync();
        }

        // ===== Customer =====
        public async Task<CustomerResponse> CreateCustomerAsync(CreateCustomerRequest req)
        {
            var tenantId = _tenantCtx.TenantId ?? throw new InvalidOperationException("Tenant Context not found");
            var tenant = await _db.Tenants.FindAsync(tenantId) ?? throw new InvalidOperationException("Tenant not found");
            var cust = new Customer { Name = req.Name, Phone = req.Phone, Email = req.Email, Address = req.Address, TenantId = tenantId };
            _db.Customers.Add(cust);
            await _db.SaveChangesAsync();
            return new CustomerResponse(cust.Id, cust.Name, cust.Phone, cust.Email, cust.Address, cust.LoyaltyPoint, cust.TotalSpent, tenant.Name);
        }

        public async Task<List<CustomerResponse>> GetCustomersAsync()
        {
            return await _db.Customers.Include(c => c.Tenant)
                .Select(c => new CustomerResponse(c.Id, c.Name, c.Phone, c.Email, c.Address, c.LoyaltyPoint, c.TotalSpent, c.Tenant != null ? c.Tenant.Name : "")).ToListAsync();
        }

        // ===== Supplier =====
        public async Task<SupplierResponse> CreateSupplierAsync(CreateSupplierRequest req)
        {
            var tenantId = _tenantCtx.TenantId ?? throw new InvalidOperationException("Tenant Context not found");
            var tenant = await _db.Tenants.FindAsync(tenantId) ?? throw new InvalidOperationException("Tenant not found");
            var sup = new Supplier { Code = req.Code, Name = req.Name, Phone = req.Phone, Email = req.Email, Address = req.Address, TenantId = tenantId };
            _db.Suppliers.Add(sup);
            await _db.SaveChangesAsync();
            return new SupplierResponse(sup.Id, sup.Code, sup.Name, sup.Phone, sup.Email, sup.Address, tenant.Name);
        }

        public async Task<List<SupplierResponse>> GetSuppliersAsync()
        {
            return await _db.Suppliers.Include(s => s.Tenant)
                .Select(s => new SupplierResponse(s.Id, s.Code, s.Name, s.Phone, s.Email, s.Address, s.Tenant != null ? s.Tenant.Name : "")).ToListAsync();
        }

        // ===== Warehouse =====
        public async Task<WarehouseResponse> CreateWarehouseAsync(CreateWarehouseRequest req)
        {
            var tenantId = _tenantCtx.TenantId ?? throw new InvalidOperationException("Tenant Context not found");
            var tenant = await _db.Tenants.FindAsync(tenantId) ?? throw new InvalidOperationException("Tenant not found");
            var wh = new WareHouse { Code = req.Code, Name = req.Name, Address = req.Address, TenantId = tenantId };
            _db.Warehouses.Add(wh);
            await _db.SaveChangesAsync();
            return new WarehouseResponse(wh.Id, wh.Code, wh.Name, wh.Address, tenant.Name);
        }

        public async Task<List<WarehouseResponse>> GetWarehousesAsync()
        {
            return await _db.Warehouses.Include(w => w.Tenant)
                .Select(w => new WarehouseResponse(w.Id, w.Code, w.Name, w.Address, w.Tenant != null ? w.Tenant.Name : "")).ToListAsync();
        }

        // ===== Role =====
        public async Task<List<RoleResponse>> GetRolesAsync()
        {
            return await _db.Roles
                .Select(r => new RoleResponse(r.Id, r.Name, r.Description)).ToListAsync();
        }

        // ===== Inventory =====
        public async Task<List<InventoryResponse>> GetInventoriesAsync()
        {
            return await _db.Inventories
                .Include(i => i.Warehouse).Include(i => i.Variant).ThenInclude(v => v!.Product)
                .Select(i => new InventoryResponse(
                    i.Id, i.WarehouseId, i.Warehouse != null ? i.Warehouse.Name : "",
                    i.Variant != null && i.Variant.Product != null ? i.Variant.ProductId : Guid.Empty,
                    i.Variant != null && i.Variant.Product != null ? i.Variant.Product.Name : "",
                    i.VariantId, i.Variant != null ? i.Variant.Sku : "",
                    i.Quantity,
                    i.Variant != null ? i.Variant.CostPrice : 0,
                    i.Variant != null ? i.Variant.SellPrice : 0
                )).ToListAsync();
        }

        // ===== AppUser =====
        public async Task<AppUserResponse> CreateAppUserAsync(CreateAppUserRequest req)
        {
            var tenant = await _db.Tenants.FindAsync(req.TenantId) ?? throw new InvalidOperationException("Tenant not found");
            var roles = await _db.Roles.Where(r => req.RolesId.Contains(r.Id)).ToListAsync();
            var user = new AppUser
            {
                Email = req.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Name = req.Name, Phone = req.Phone,
                TenantId = req.TenantId, Roles = roles
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return new AppUserResponse(user.Id, user.Email, user.Name, user.Phone, user.Status.ToString(), user.CreatedAt, tenant.Name, roles.Select(r => r.Name).ToList());
        }

        public async Task<List<AppUserResponse>> GetAppUsersAsync()
        {
            return await _db.Users.Include(u => u.Tenant).Include(u => u.Roles)
                .Select(u => new AppUserResponse(u.Id, u.Email, u.Name, u.Phone, u.Status.ToString(), u.CreatedAt,
                    u.Tenant != null ? u.Tenant.Name : "", u.Roles.Select(r => r.Name).ToList())).ToListAsync();
        }
    }
}
