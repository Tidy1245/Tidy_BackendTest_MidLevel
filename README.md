# Tidy_BackendTest_MidLevel

後端工程師技術測試 — .NET Core 8 Web API + SQL Server CRUD

## 專案架構

```
Tidy_BackendTest_MidLevel.sln
├── src/
│   ├── Domain/          Entity（MyOfficeAcpd）、SidGeneratorService
│   ├── Application/     DTOs、IMyOfficeAcpdRepository 介面、MyOfficeAcpdService
│   ├── Infrastructure/  EF Core DbContext、MyOfficeAcpdRepository、DI 擴充方法
│   └── API/             MyOfficeAcpdController、Program.cs、Swagger 設定
├── tests/
│   └── Application.Tests/  xUnit 單元測試（Domain + Application Service）
└── TSQLScript/          SQL 建表、種子資料、NEWSID 預存程序
```

### 分層相依關係（DDD）
```
API → Application + Infrastructure
Application → Domain
Infrastructure → Application + Domain
Tests → Application + Domain
```

---

## 先決條件

| 工具 | 版本 |
|------|------|
| .NET SDK | 10.x |
| SQL Server | 2019+ 或 Express / LocalDB |
| Visual Studio | 2022 |

---

## 資料庫設定

### 方法一：執行 SQL Scripts（建議）

1. 開啟 **SQL Server Management Studio (SSMS)**
2. 依序執行以下腳本：
   ```
   TSQLScript/01_CreateDatabase_And_Table.sql   ← 建立資料庫與資料表
   TSQLScript/02_SeedData.sql                   ← 新增 10 筆測試資料
   TSQLScript/03_StoredProc_NEWSID.sql          ← 建立 NEWSID SP（選用）
   ```

### 方法二：還原備份檔

1. 在 SSMS 中對 **資料庫** 按右鍵 → **還原資料庫**
2. 選取 `Myoffice_ACPD.bak`（若存在）
3. 按確定完成還原

---

## 連線字串設定

編輯 `src/API/Tidy_BackendTest_MidLevel.API/appsettings.Development.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Myoffice_ACPD;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

將 `YOUR_SERVER_NAME` 替換為實際 SQL Server 執行個體名稱，例如：
- `localhost` 或 `(local)` — 本機預設執行個體
- `localhost\SQLEXPRESS` — SQL Server Express
- `(localdb)\MSSQLLocalDB` — LocalDB

---

## 執行專案

1. 以 **Visual Studio 2022** 開啟 `Tidy_BackendTest_MidLevel.sln`
2. 設定啟動專案為 `Tidy_BackendTest_MidLevel.API`
3. 按 **F5** 啟動 → 瀏覽器自動開啟 `https://localhost:7100/swagger`

或使用命令列：
```bash
cd src/API/Tidy_BackendTest_MidLevel.API
dotnet run
```
然後開啟 `https://localhost:7100/swagger`

---

## 執行測試

```bash
dotnet test
```

測試涵蓋：
- `SidGeneratorServiceTests` — Domain Service 單元測試（11 個測試）
- `MyOfficeAcpdServiceTests` — Application Service 單元測試（10 個測試）

---

## API 端點

| HTTP Method | URL | 說明 | 回應碼 |
|-------------|-----|------|--------|
| GET | `/api/myofficeacpd` | 查詢所有人員帳號 | 200 |
| GET | `/api/myofficeacpd/{id}` | 依 SID 查詢單筆 | 200 / 404 |
| POST | `/api/myofficeacpd` | 新增人員帳號（SID 自動產生）| 201 / 400 |
| PUT | `/api/myofficeacpd/{id}` | 依 SID 更新人員帳號 | 200 / 400 / 404 |
| DELETE | `/api/myofficeacpd/{id}` | 依 SID 刪除人員帳號 | 204 / 404 |

### POST 測試 JSON 範例

```json
{
  "ACPD_Cname": "王小明",
  "ACPD_Ename": "Wang Xiao Ming",
  "ACPD_Sname": "WangXM",
  "ACPD_Email": "wang@myoffice.com",
  "ACPD_Status": 0,
  "ACPD_Stop": false,
  "ACPD_StopMemo": "",
  "ACPD_LoginID": "wangxm01",
  "ACPD_LoginPWD": "P@ssw0rd123",
  "ACPD_Memo": "測試帳號"
}
```

### PUT 測試 JSON 範例

```json
{
  "ACPD_Cname": "王小明（更新）",
  "ACPD_Ename": "Wang Xiao Ming Updated",
  "ACPD_Email": "wang.updated@myoffice.com",
  "ACPD_Status": 1,
  "ACPD_Stop": false,
  "ACPD_LoginPWD": "NewP@ssw0rd456",
  "ACPD_Memo": "更新後備註"
}
```

---

## Git 分支策略

```
main          ← 最終版本（提交用）
  └── develop ← 開發主線
        └── feature/crud-api ← 功能開發分支
```

---

## 關鍵設計說明

| 項目 | 說明 |
|------|------|
| **SID 產生** | `SidGeneratorService` 以 C# 實作與 NEWSID SP 相同邏輯，可單元測試 |
| **char(20) Trim** | SQL Server `char` 欄位右補空白，Repository 讀取時呼叫 `.Trim()` 防止 404 |
| **AsNoTracking()** | 讀取查詢使用 `AsNoTracking()` 提升效能 |
| **稽核欄位** | 建立/更新時自動填入 `NowDateTime`、`NowID`、`UPDDateTime`、`UPDID` |
| **HTTP 狀態碼** | POST→201, PUT/GET→200, DELETE→204, 找不到→404, 驗證失敗→400 |
