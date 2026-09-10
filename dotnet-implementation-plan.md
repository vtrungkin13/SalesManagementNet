# .NET / C# Implementation Plan — Sales Management System

## 1. Mục tiêu

Triển khai **Multi-Tenant Sales Management System** theo `project-specification.md` bằng **C# / ASP.NET Core Web API + Entity Framework Core + SQL Server**, với Redis, JWT authentication, RBAC, Docker và các chức năng quản lý bán hàng, tồn kho, mua hàng.

Nguyên tắc triển khai:

- Không tạo lại những thành phần đã tồn tại trong workspace.
- Triển khai từng phase, sau mỗi phase phải build và kiểm tra trước khi sang phase tiếp theo.
- Ưu tiên hoàn thiện từng feature end-to-end: DTO → Controller → Service → EF Core → Validation → Authorization → Test.
- Multi-tenancy và transaction/inventory integrity phải được thiết kế ngay từ đầu, không bổ sung ở cuối.
- Dùng `decimal` cho các trường tiền tệ thay vì `double`.

---

# 2. Kiến trúc project

```text
SalesManagement/
│
├── API/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Extensions/
│   └── Program.cs
│
├── Core/
│   ├── Models/
│   ├── DTOs/
│   │   ├── Requests/
│   │   └── Responses/
│   ├── Enums/
│   ├── Interfaces/
│   └── Exceptions/
│
├── Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Repositories/
│   ├── Services/
│   ├── Authentication/
│   ├── Caching/
│   └── Seed/
│
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## Java → .NET mapping

| Java / Spring | C# / .NET |
|---|---|
| Spring Boot | ASP.NET Core Web API |
| JPA / Hibernate | Entity Framework Core |
| Spring Data Repository | EF Core / Repository |
| Spring Security | ASP.NET Core Authentication / Authorization |
| JJWT | ASP.NET Core JwtBearer |
| BCrypt | BCrypt.Net |
| MapStruct | Manual Mapper / AutoMapper |
| Jakarta Validation | DataAnnotations / FluentValidation |
| Redis | StackExchange.Redis / Distributed Cache |
| Maven | `.csproj` + NuGet |

---

# 3. Phase 0 — Chuẩn bị project

## Step 0.1 — Inspect project hiện tại

Kiểm tra:

- `.sln`
- `.csproj`
- `Program.cs`
- `appsettings.json`
- `Core/`
- `API/`
- `Infrastructure/`
- Các file `.cs` đã tồn tại

Không ghi đè file hiện có nếu chưa đọc và hiểu nội dung.

## Step 0.2 — Setup NuGet packages

Các package chính cần có:

```text
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
Microsoft.AspNetCore.Authentication.JwtBearer
Swashbuckle.AspNetCore
BCrypt.Net-Next
Microsoft.Extensions.Caching.StackExchangeRedis
StackExchange.Redis
```

Có thể bổ sung FluentValidation hoặc AutoMapper nếu thực sự cần.

## Step 0.3 — Verify build

```bash
dotnet build
dotnet run
```

Kiểm tra Swagger hoạt động.

### Checkpoint

- [ ] Project build thành công
- [ ] API chạy được
- [ ] Swagger hoạt động
- [ ] SQL Server connection đã được chuẩn bị

---

# 4. Phase 1 — Database foundation + EF Core

## Step 1.1 — Tạo Enums

```text
Core/Enums/
├── TenantStatus.cs
├── UserStatus.cs
├── ProductStatus.cs
├── PurchaseStatus.cs
├── TransactionType.cs
└── ImportJobStatus.cs
```

Giá trị:

```text
TenantStatus: ACTIVE | INACTIVE
UserStatus: ACTIVE | INACTIVE
ProductStatus: ACTIVE | INACTIVE
PurchaseStatus: PENDING | APPROVED | DELIVERY | RECEIVED | CANCELLED
TransactionType: IN | OUT
ImportJobStatus: PENDING | PROCESSING | COMPLETED | FAILED | CANCELLED
```

## Step 1.2 — Tạo Entities theo dependency

### Level 1

```text
Tenant
Role
```

### Level 2

```text
AppUser
RefreshToken
Category
Customer
Supplier
Warehouse
```

### Level 3

```text
Product
ProductImage
ProductVariant
```

### Level 4

```text
Inventory
InventoryTransaction
```

### Level 5

```text
SalesOrder
SalesOrderItem
PurchaseOrder
PurchaseOrderItem
GoodsReceipt
GoodsReceiptItem
```

### Level 6

```text
ReturnOrder
ReturnOrderItem
Notification
ImportJob
ImportJobError
```

## Step 1.3 — Tạo AppDbContext

```text
Infrastructure/Data/AppDbContext.cs
```

DbContext phải expose toàn bộ DbSet cần thiết và apply entity configurations.

---

# 5. Phase 2 — EF Core configuration

Tạo:

```text
Infrastructure/Data/Configurations/
```

Mỗi entity có configuration riêng khi cần:

```text
AppUserConfiguration.cs
TenantConfiguration.cs
ProductConfiguration.cs
ProductVariantConfiguration.cs
InventoryConfiguration.cs
...
```

Sử dụng:

```csharp
IEntityTypeConfiguration<TEntity>
```

và:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(
    typeof(AppDbContext).Assembly);
```

## Step 2.1 — Configure relationships

Đảm bảo đúng các quan hệ:

- Tenant → Users
- Tenant → Products / Categories / Customers / Suppliers / Warehouses
- Product → Category / Tenant / ProductImage / ProductVariants
- ProductVariant → Product / Tenant / Inventory / PurchaseOrderItems / SalesOrderItems / ReturnOrderItems
- Warehouse → Inventory / GoodsReceipts
- SalesOrder → SalesOrderItems / Customer / Tenant
- PurchaseOrder → PurchaseOrderItems / Supplier / Tenant / GoodsReceipts

## Step 2.2 — Configure indexes và unique constraints

Các unique constraint chính:

```text
Tenant.code
AppUser.email
Product (tenant_id + code)
ProductVariant (tenant_id + sku)
SalesOrder.order_number
PurchaseOrder.po_number
GoodsReceipt.receipt_number
ReturnOrder.return_number
RefreshToken.token
Role.name
```

## Step 2.3 — Money và numeric fields

Dùng `decimal` cho:

```text
CostPrice
SellPrice
UnitPrice
UnitCost
Subtotal
Discount
Total
Amount
TotalRefund
```

---

# 6. Phase 3 — Migration + Database

## Step 3.1 — Connection string

Cấu hình SQL Server trong `appsettings.json` và environment variables.

## Step 3.2 — Tạo migration

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Step 3.3 — Verify database

Kiểm tra:

- Tables
- Primary keys
- Foreign keys
- Indexes
- Unique constraints
- Column types
- Nullable / required fields

### Checkpoint

- [ ] EF Core migration thành công
- [ ] SQL Server database được tạo
- [ ] Schema khớp specification

---

# 7. Phase 4 — Seed data

Tạo:

```text
Infrastructure/Seed/DbSeeder.cs
```

Seed:

```text
Roles:
ADMIN
USER

Tenant:
TS1 / Tenant Store 1

Users:
admin@gmail.com
user@gmail.com
```

Quy tắc:

- ADMIN không thuộc tenant.
- USER thuộc TS1.
- Password phải được hash bằng BCrypt.
- Seeder phải idempotent, không tạo duplicate khi chạy nhiều lần.

### Checkpoint

- [ ] Database startup có ADMIN
- [ ] Database startup có USER
- [ ] Password không lưu plaintext
- [ ] Roles và tenant được seed đúng

---

# 8. Phase 5 — Authentication

Đây là feature đầu tiên phải hoàn thiện end-to-end.

## Step 5.1 — Auth DTOs

Tạo:

```text
LoginRequest
RefreshTokenRequest
LogoutRequest
AuthResponse
```

## Step 5.2 — Login

```http
POST /api/auth/login
```

Flow:

```text
Request
  ↓
Validate
  ↓
Find user
  ↓
Verify BCrypt
  ↓
Generate JWT access token
  ↓
Create refresh token
  ↓
Response
```

## Step 5.3 — JWT

JWT cần chứa tối thiểu:

```json
{
  "sub": "user@email.com",
  "tenantId": "uuid"
}
```

Access token expiry: 15 phút.

Refresh token expiry: 7 ngày.

## Step 5.4 — Refresh

```http
POST /api/auth/refresh
```

Flow:

```text
Validate refresh token
        ↓
Check expiry
        ↓
Delete old refresh token
        ↓
Create new refresh token
        ↓
Create new access token
```

Nếu refresh token expired: xóa toàn bộ refresh token của user và trả 401.

## Step 5.5 — Logout

```http
POST /api/auth/logout
```

Xóa toàn bộ refresh token của user.

## Step 5.6 — Login rate limiter

```text
5 attempts / 60 seconds / IP
```

Nếu vượt giới hạn:

```http
429 Too Many Requests
```

### Checkpoint

- [ ] Login thành công
- [ ] Sai password bị reject
- [ ] JWT validate được
- [ ] Refresh rotation hoạt động
- [ ] Logout revoke token
- [ ] Rate limiting hoạt động

---

# 9. Phase 6 — Multi-tenancy

Đây là phần security-critical.

Architecture:

```text
Request
  ↓
JWT Authentication
  ↓
TenantId
  ↓
TenantContext
  ↓
Service
  ↓
EF Core query với tenant filter
```

Tạo:

```text
Core/Interfaces/ITenantContext.cs
Infrastructure/.../TenantContext.cs
```

Service lấy tenant từ authenticated context, không nhận tenantId tùy ý từ client cho các thao tác tenant-scoped.

Mọi query tenant-scoped phải filter theo current tenant.

Ví dụ:

```csharp
query.Where(x => x.TenantId == tenantId)
```

Khi lấy single entity phải verify entity thuộc current tenant.

### Checkpoint

- [ ] TenantContext hoạt động
- [ ] Tenant A không đọc được dữ liệu Tenant B
- [ ] Tenant A không sửa/xóa được dữ liệu Tenant B
- [ ] ADMIN có behavior global đúng specification

---

# 10. Phase 7 — RBAC / Authorization

Dùng ASP.NET Core Authorization.

Ví dụ:

```csharp
[Authorize(Roles = "ADMIN")]
```

và:

```csharp
[Authorize(Roles = "USER")]
```

## ADMIN

- Tenant
- Role
- AppUser

## USER

- Category
- Product
- Customer
- Supplier
- Warehouse
- Inventory
- SalesOrder
- PurchaseOrder
- GoodsReceipt

### Checkpoint

- [ ] Anonymous request → 401
- [ ] User không có quyền → 403
- [ ] ADMIN endpoint đúng quyền
- [ ] USER endpoint đúng quyền

---

# 11. Phase 8 — Tenant + User + Role

## Tenant API

```text
POST   /api/tenant/create
GET    /api/tenant/get-all
GET    /api/tenant/{id}
PUT    /api/tenant/{id}
DELETE /api/tenant/{id}
```

## AppUser API

```text
POST /api/app-user/create
GET  /api/app-user/get-all
GET  /api/app-user/{id}
PUT  /api/app-user/update
PUT  /api/app-user/activate/{id}
PUT  /api/app-user/deactivate/{id}
POST /api/app-user/assign-roles
```

## Role API

```text
GET /api/role/get-all
```

Mỗi endpoint phải có:

```text
DTO
Controller
Service
EF Core
Validation
Authorization
Error handling
```

### Checkpoint

ADMIN có thể:

- [ ] Tạo tenant
- [ ] Quản lý tenant
- [ ] Tạo user
- [ ] Activate/deactivate user
- [ ] Assign roles

---

# 12. Phase 9 — Category

Implement CRUD:

```text
POST   /api/category/create
GET    /api/category
GET    /api/category/{id}
PUT    /api/category/{id}
DELETE /api/category/{id}
```

Yêu cầu:

- USER authorization
- Pagination
- Tenant isolation
- Validation
- DTO mapping

### Checkpoint

- [ ] Category CRUD
- [ ] Pagination
- [ ] Tenant isolation

---

# 13. Phase 10 — Product

Entity/module:

```text
Product
ProductImage
ProductVariant
```

## Create Product

Flow:

```text
Validate category
      ↓
Create Product
      ↓
Create ProductImage nếu có
      ↓
Create exactly one default ProductVariant
```

API:

```text
POST   /api/product/create
POST   /api/product/import
GET    /api/product
GET    /api/product/{id}
PUT    /api/product/{id}
DELETE /api/product/{id}
```

Unique:

```text
Product: tenant_id + code
Variant: tenant_id + sku
```

Chưa làm async import ở bước này; async import sẽ là một phase riêng.

### Checkpoint

- [ ] Product CRUD
- [ ] ProductVariant được tạo đúng
- [ ] Category ownership được kiểm tra
- [ ] Tenant isolation
- [ ] Pagination/filtering

---

# 14. Phase 11 — Customer / Supplier / Warehouse

Triển khai lần lượt:

## Customer

```text
create
list
get
update
delete
```

## Supplier

```text
create
list
get
update
delete
```

## Warehouse

```text
create
list
get
update
delete
```

Tất cả đều:

- USER only
- Tenant scoped
- Validated
- DTO based
- Có global error handling

### Checkpoint

- [ ] Customer CRUD
- [ ] Supplier CRUD
- [ ] Warehouse CRUD
- [ ] Tenant isolation

---

# 15. Phase 12 — Inventory

API:

```text
GET  /api/inventory
GET  /api/inventory/stats
POST /api/inventory/adjust
GET  /api/inventory/transactions
```

## Stock operations

```text
IN
OUT
```

## Atomic stock deduction

Không sử dụng flow nguy hiểm:

```text
SELECT stock
↓
if stock >= quantity
↓
UPDATE stock
```

Thay vào đó cần atomic update với điều kiện quantity đủ.

Ví dụ concept:

```sql
UPDATE inventory
SET quantity = quantity - @quantity
WHERE id = @id
  AND quantity >= @quantity
```

## Inventory transaction

Mọi stock movement phải tạo `InventoryTransaction` phù hợp.

### Checkpoint

- [ ] Inventory list
- [ ] Inventory stats
- [ ] IN adjustment
- [ ] OUT adjustment
- [ ] Transaction history
- [ ] Không cho stock âm
- [ ] Atomic deduction

---

# 16. Phase 13 — Purchase Order

Flow:

```text
Supplier
   ↓
Purchase Order
   ↓
PENDING
   ↓
Goods Receipt
   ↓
Inventory IN
```

API:

```text
POST   /api/purchase-order
GET    /api/purchase-order
GET    /api/purchase-order/{id}
PUT    /api/purchase-order/{id}
DELETE /api/purchase-order/{id}
PATCH  /api/purchase-order/{id}/status
```

Tính amount:

```text
amount = Σ(quantity × unitCost)
```

### Checkpoint

- [ ] Purchase Order CRUD
- [ ] Supplier validation
- [ ] Amount calculation
- [ ] Tenant isolation
- [ ] Status API

---

# 17. Phase 14 — Goods Receipt

Flow:

```text
PurchaseOrder
      ↓
GoodsReceipt
      ↓
Inventory
      ↓
InventoryTransaction(IN)
```

API:

```text
POST /api/goods-receipt
GET  /api/goods-receipt
GET  /api/goods-receipt/{id}
```

Khi nhận hàng:

- Validate PurchaseOrder
- Validate Warehouse
- Validate tenant
- Find/create Inventory
- Increase stock
- Create IN transaction
- Save trong transaction

### Checkpoint

- [ ] Goods Receipt tạo được
- [ ] Inventory tăng đúng
- [ ] IN transaction được tạo
- [ ] Rollback khi có lỗi

---

# 18. Phase 15 — Sales Order

Đây là business feature phức tạp nhất.

Flow:

```text
Customer + Warehouse + Items
            ↓
        Validation
            ↓
       Check Inventory
            ↓
     Atomic stock deduction
            ↓
 InventoryTransaction OUT
            ↓
      Calculate totals
            ↓
       Create Order
```

## Total calculation

```text
subtotal = Σ(unitPrice × quantity)
discount = Σ(item discount)
total = max(0, subtotal - discount)
```

API:

```text
POST   /api/sales-order/create
GET    /api/sales-order
GET    /api/sales-order/{id}
PUT    /api/sales-order/{id}
DELETE /api/sales-order/{id}
```

## Sales Order update

Phải xử lý:

1. Verify tenant.
2. Restore old inventory.
3. Remove old inventory transactions.
4. Remove old items.
5. Validate new items.
6. Deduct inventory cho items mới.
7. Create new OUT transactions.
8. Recalculate totals.
9. Save atomically.

Cần đặc biệt tránh logic phụ thuộc vào “first inventory record” nếu có thể thiết kế chính xác hơn theo warehouse/variant.

### Checkpoint

- [ ] Create order
- [ ] List/detail
- [ ] Update
- [ ] Delete
- [ ] Stock deduction
- [ ] Stock restoration
- [ ] Total calculation
- [ ] Inventory transactions
- [ ] Transaction rollback

---

# 19. Phase 16 — Database Transactions + Concurrency

Các operation quan trọng phải transactional:

```text
SalesOrder create/update/delete
GoodsReceipt
Inventory adjustment
```

Concept:

```csharp
await using var transaction =
    await db.Database.BeginTransactionAsync();

try
{
    // business operations
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

## Concurrency scenario

Ví dụ:

```text
Stock = 10

Request A = buy 7
Request B = buy 7
```

Kết quả hợp lệ:

```text
Một request thành công
Một request bị reject
```

Không được để stock thành `-4`.

---

# 20. Phase 17 — Return Order

Entity đã tồn tại:

```text
ReturnOrder
ReturnOrderItem
```

Nhưng specification chưa định nghĩa đầy đủ ReturnOrder API.

Do đó:

**Không đưa vào MVP core.**

Chỉ triển khai sau khi Sales Order hoàn chỉnh và xác định rõ:

- API
- Authorization
- Refund calculation
- Inventory restoration
- Return status

---

# 21. Phase 18 — Redis

Chỉ thêm Redis sau khi business logic và database đã ổn định.

Các module phù hợp để cache:

```text
Category list
Product list
Customer list
Supplier list
Warehouse list
```

Cấu hình:

```text
REDIS_HOST
REDIS_PORT
TTL = 3600 seconds
```

Cần có cache invalidation khi:

```text
Create
Update
Delete
```

### Checkpoint

- [ ] Redis connection
- [ ] Cache read
- [ ] Cache write
- [ ] Cache invalidation
- [ ] TTL
- [ ] Không trả stale data sau mutation

---

# 22. Phase 19 — Async Product Import

Flow:

```text
Upload CSV/Excel
       ↓
Create ImportJob
       ↓
PROCESSING
       ↓
Background processing
       ↓
Validate rows
       ↓
Batch insert
       ↓
ImportJobError nếu lỗi
       ↓
COMPLETED / FAILED
```

API:

```text
POST /api/import/products/start
GET  /api/import/products/jobs/{jobId}
POST /api/import/products/jobs/{jobId}/retry
GET  /api/import/products/jobs/{jobId}/errors
```

Hỗ trợ:

```text
processedOffset
```

để resumable processing.

Cần phân biệt rõ endpoint import synchronous `/api/product/import` và async import `/api/import/products/start` trước khi triển khai cả hai.

### Checkpoint

- [ ] Upload file
- [ ] Create ImportJob
- [ ] Background processing
- [ ] Batch processing
- [ ] Error logging
- [ ] Retry
- [ ] Resumable offset

---

# 23. Phase 20 — Global Exception Handling

Tạo:

```text
API/Middleware/ExceptionHandlingMiddleware.cs
```

Response format:

```json
{
  "timestamp": "...",
  "status": 400,
  "error": "Error Type",
  "message": "...",
  "details": {}
}
```

Mapping chính:

```text
Validation → 400
Email already exists → 409
Invalid refresh token → 401
Authentication → 401
Rate limit → 429
Other runtime errors → 400
```

Không trả stack trace hoặc thông tin nhạy cảm cho client production.

---

# 24. Phase 21 — Validation

Request DTO phải validate input.

Ví dụ:

```csharp
[Required]
[EmailAddress]
public string Email { get; set; }
```

Các rule quan trọng:

```text
quantity > 0
price >= 0
discount >= 0
email valid
required fields
phone format
```

Validation phải được xử lý thống nhất qua global error handling.

---

# 25. Phase 22 — Testing

Không đợi đến cuối mới test.

Mỗi feature:

```text
Implement
   ↓
Build
   ↓
Swagger/manual test
   ↓
Unit test
   ↓
Integration test
```

## Authentication tests

- [ ] Correct login
- [ ] Wrong password
- [ ] Expired access token
- [ ] Expired refresh token
- [ ] Refresh rotation
- [ ] Logout
- [ ] Rate limiting

## Multi-tenancy tests

```text
Tenant A user
    ↓
GET Product
    ↓
Không được thấy Tenant B product
```

- [ ] Cross-tenant GET rejected/hidden
- [ ] Cross-tenant UPDATE rejected
- [ ] Cross-tenant DELETE rejected

## Inventory tests

```text
Stock = 10
Order = 7
=> Stock = 3

Order = 5
=> Reject
```

## Concurrency test

```text
Stock = 10
A buys 7
B buys 7
=> only one succeeds
```

---

# 26. Phase 23 — Docker

Sau khi local application ổn định:

```text
docker-compose
├── sqlserver
├── app
└── redis
```

Đảm bảo startup dependency:

```text
SQL Server
    ↓
healthy
    ↓
Application
```

Kiểm tra:

- [ ] Docker image build
- [ ] SQL Server container
- [ ] Redis container
- [ ] API container
- [ ] Environment variables
- [ ] Database initialization
- [ ] Health check

---

# 27. Phase 24 — Production hardening

Hoàn thiện:

```text
CORS
JWT secret management
Environment variables
HTTPS
Swagger
Logging
Health check
Docker
Database migration
```

Không hard-code:

```text
DB_USERNAME
DB_PASSWORD
DB_NAME
JWT_SECRET_KEY
REDIS_HOST
REDIS_PORT
```

Thêm health endpoint để kiểm tra application/database/dependencies khi cần.

---

# 28. Tổng roadmap

```text
0. Project Setup
        ↓
1. EF Core + Database Foundation
        ↓
2. EF Configurations
        ↓
3. Migration + Database
        ↓
4. Seed
        ↓
5. Authentication
        ↓
6. Multi-tenancy
        ↓
7. RBAC
        ↓
8. Tenant / User / Role
        ↓
9. Category
        ↓
10. Product
        ↓
11. Customer / Supplier / Warehouse
        ↓
12. Inventory
        ↓
13. Purchase Order
        ↓
14. Goods Receipt
        ↓
15. Sales Order
        ↓
16. Transactions + Concurrency
        ↓
17. Return Order (later)
        ↓
18. Redis
        ↓
19. Async Product Import
        ↓
20. Global Exception Handling
        ↓
21. Validation
        ↓
22. Testing
        ↓
23. Docker
        ↓
24. Production Hardening
```

---

# 29. Task checklist tổng

- [ ] 01 — Inspect existing project
- [ ] 02 — Setup NuGet
- [ ] 03 — Create Enums
- [ ] 04 — Create Entities
- [ ] 05 — Create EF Configurations
- [ ] 06 — Create AppDbContext
- [ ] 07 — Create Initial Migration
- [ ] 08 — Update Database
- [ ] 09 — Create Seeder
- [ ] 10 — Auth DTOs
- [ ] 11 — JWT Authentication
- [ ] 12 — Refresh Token Rotation
- [ ] 13 — Logout
- [ ] 14 — Login Rate Limiter
- [ ] 15 — TenantContext
- [ ] 16 — RBAC
- [ ] 17 — Tenant API
- [ ] 18 — AppUser API
- [ ] 19 — Role API
- [ ] 20 — Category API
- [ ] 21 — Product API
- [ ] 22 — Customer API
- [ ] 23 — Supplier API
- [ ] 24 — Warehouse API
- [ ] 25 — Inventory API
- [ ] 26 — Purchase Order API
- [ ] 27 — Goods Receipt API
- [ ] 28 — Sales Order API
- [ ] 29 — Transaction handling
- [ ] 30 — Concurrency handling
- [ ] 31 — Return Order design
- [ ] 32 — Redis
- [ ] 33 — Async Product Import
- [ ] 34 — Global Exception Handling
- [ ] 35 — Validation
- [ ] 36 — Unit Tests
- [ ] 37 — Integration Tests
- [ ] 38 — Docker
- [ ] 39 — Health Checks
- [ ] 40 — Production Hardening

---

# 30. Quy tắc triển khai với PalmBridge

Khi triển khai từng task trong workspace:

1. **Inspect trước** — đọc file/project structure liên quan.
2. **Không overwrite mù** — nếu file đã tồn tại phải đọc trước.
3. **Implement một task/feature tại một thời điểm.**
4. **Build sau mỗi thay đổi quan trọng.**
5. **Kiểm tra git diff trước khi chuyển phase.**
6. **Không tự ý thay đổi architecture đã thống nhất nếu chưa có lý do.**
7. **Ưu tiên security và tenant isolation hơn convenience.**
8. **Business operation liên quan inventory phải transactional.**
9. **Không thêm feature ngoài specification vào MVP nếu chưa cần thiết.**
10. **Chỉ đánh dấu task hoàn thành khi code đã build/test được.**

---

# 31. Definition of Done cho mỗi feature

Một feature chỉ được xem là hoàn thành khi có đủ:

```text
[ ] Entity / EF mapping
[ ] DTO Request
[ ] DTO Response
[ ] Controller
[ ] Service
[ ] Repository/query nếu cần
[ ] Validation
[ ] Authorization
[ ] Tenant isolation
[ ] Exception handling
[ ] Swagger/manual verification
[ ] Unit test nếu có business logic
[ ] Integration test nếu liên quan DB/security
[ ] dotnet build thành công
[ ] Git diff đã kiểm tra
```

---

# 32. MVP ưu tiên

Nếu cần một phiên bản chạy được sớm, ưu tiên:

```text
Foundation
  ↓
Auth + RBAC + Multi-tenancy
  ↓
Tenant/User
  ↓
Category/Product
  ↓
Customer/Supplier/Warehouse
  ↓
Inventory
  ↓
Purchase + Goods Receipt
  ↓
Sales Order
  ↓
Testing
  ↓
Docker
```

Sau MVP mới triển khai:

```text
Redis
Async Import
Return Order
Production optimizations
```
