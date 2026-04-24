# CLAUDE.md — 專案開發紀錄

## 專案資訊

| 項目 | 說明 |
|------|------|
| 專案名稱 | Tidy BackendTest MidLevel |
| 框架 | .NET 10 · ASP.NET Core · EF Core 9.0.4 |
| 資料庫 | SQL Server (LocalDB / SQLEXPRESS) |
| 測試框架 | xUnit · Moq · FluentAssertions |
| 架構風格 | DDD (Domain-Driven Design) + TDD |

---

## 開發進度

### ✅ Phase 1 — CRUD Web API（完成）

**目標：** 實作 `MyOfficeAcpd`（人員帳號）完整 CRUD，遵循 DDD 四層架構。

**已完成項目：**
- `Domain/Entities/MyOfficeAcpd.cs` — 人員帳號實體（20 個欄位）
- `Domain/Services/SidGeneratorService.cs` — 20 字元 SID 產生器（Base-36 編碼）
- `Application/Interfaces/IMyOfficeAcpdRepository.cs` — Repository 契約
- `Application/Services/MyOfficeAcpdService.cs` — 業務邏輯（CRUD + 碰撞重試）
- `Application/DTOs/` — 3 個 DTO（Query / Create / Update）
- `Infrastructure/Data/AppDbContext.cs` — EF Core DbContext
- `Infrastructure/Repositories/MyOfficeAcpdRepository.cs` — Repository 實作
- `API/Controllers/MyOfficeAcpdController.cs` — 5 個 REST 端點
- **測試：** 25 個單元測試（SidGeneratorService 8 + MyOfficeAcpdService 12 + SidFormat 5）

---

### ✅ Phase 2 — 多租戶隔離方案（完成）

**目標：** 共享資料庫、共享 Schema，所有資料表加 `TenantId`，查詢自動帶 `WHERE TenantId = ?`。

**架構機制：**
```
X-Tenant-Id Header
    → TenantMiddleware（Scoped ITenantContext.TenantId = header value）
    → AppDbContext（注入 ITenantContext）
    → HasQueryFilter(e => e.TenantId == _tenantContext.TenantId)
    → SQL: WHERE TenantId = 'xxx'
```

**新增/修改檔案（17 個）：**

| 層 | 動作 | 檔案 |
|----|------|------|
| Domain | 新建 | `Entities/Tenant.cs` |
| Domain | 新建 | `Enums/SubscriptionStatus.cs`（Trial/Active/Inactive） |
| Domain | 修改 | `Entities/MyOfficeAcpd.cs`（加 TenantId FK） |
| Application | 新建 | `Interfaces/ITenantContext.cs` |
| Application | 新建 | `Interfaces/ITenantRepository.cs` |
| Application | 新建 | `DTOs/TenantDto.cs` |
| Application | 新建 | `DTOs/CreateTenantRequest.cs` |
| Application | 新建 | `DTOs/UpdateTenantRequest.cs` |
| Application | 新建 | `Services/TenantService.cs` |
| Infrastructure | 新建 | `Data/TenantContext.cs`（ITenantContext 實作） |
| Infrastructure | 修改 | `Data/AppDbContext.cs`（Global Query Filter + Tenant DbSet） |
| Infrastructure | 新建 | `Repositories/TenantRepository.cs` |
| Infrastructure | 修改 | `DependencyInjection.cs`（ITenantContext、ITenantRepository 註冊） |
| API | 新建 | `Middleware/TenantMiddleware.cs` |
| API | 新建 | `Controllers/TenantController.cs`（5 個端點） |
| API | 修改 | `Program.cs`（TenantService 註冊、UseMiddleware） |
| Tests | 新建 | `Services/TenantServiceTests.cs`（12 個測試） |

**測試結果：** 37/37 通過，0 錯誤，0 警告

---

## API 端點總覽

### 租戶管理（無需 Header）

| Method | URL | 說明 |
|--------|-----|------|
| GET | `/api/tenants` | 查詢所有租戶 |
| GET | `/api/tenants/{id}` | 查詢單筆租戶 |
| POST | `/api/tenants` | 新增租戶 |
| PUT | `/api/tenants/{id}` | 更新租戶 |
| DELETE | `/api/tenants/{id}` | 刪除租戶 |

### 人員帳號（需 `X-Tenant-Id` Header）

| Method | URL | 說明 |
|--------|-----|------|
| GET | `/api/myofficeacpd` | 查詢該租戶所有人員 |
| GET | `/api/myofficeacpd/{id}` | 查詢單筆（租戶範圍） |
| POST | `/api/myofficeacpd` | 新增人員（自動綁定 TenantId） |
| PUT | `/api/myofficeacpd/{id}` | 更新人員 |
| DELETE | `/api/myofficeacpd/{id}` | 刪除人員 |

---

## 關鍵設計決策

| 決策 | 說明 |
|------|------|
| **Scoped ITenantContext** | Middleware 寫入、DbContext 讀取，同一 Request 共享狀態 |
| **Global Query Filter** | `HasQueryFilter` 在 `OnModelCreating` 以 lambda 動態綁定，零侵入 Repository |
| **TenantId null 行為** | 不帶 Header → TenantId = null → filter 條件為 false → 查詢返回空集合 |
| **Tenant 表無 Filter** | TenantRepository 查詢 Tenants 表，為管理層操作，不套用租戶篩選 |
| **SID 格式共用** | TenantId 同樣採用 `SidGeneratorService` 產生，格式與 ACPD_SID 一致 |

---

## 報告文件

- `docs/overview.html` — 系統總覽頁面（簡介 / 功能 / 架構圖 / API 端點）

---

## 測試摘要

```
dotnet test → 37/37 passed, 0 failed (2026-04-24)

SidGeneratorServiceTests    — 8 tests
SidGeneratorFormatTests     — 5 tests
MyOfficeAcpdServiceTests    — 12 tests
TenantServiceTests          — 12 tests
```

---

## 待辦（可選擇擴充）

- [ ] EF Core Migration — 建立 Tenants 表與 MyOffice_ACPD.TenantId 欄位
- [ ] 更新 TSQLScript — 加入 Tenants 建表 SQL
- [ ] 驗證隔離 — Swagger 實測不同 TenantId 的資料分離
- [ ] 密碼雜湊 — 目前 ACPD_LoginPWD 為明文
