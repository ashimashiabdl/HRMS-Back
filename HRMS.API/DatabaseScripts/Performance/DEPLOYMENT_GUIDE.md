# 🚀 راهنمای استقرار بهینه‌سازی Login Activity Viewer

## 📋 فهرست مطالب
- [پیش‌نیازها](#پیش‌نیازها)
- [مراحل استقرار](#مراحل-استقرار)
- [تست و بررسی](#تست-و-بررسی)
- [عیب‌یابی](#عیب‌یابی)
- [برگشت به نسخه قبل (Rollback)](#برگشت-به-نسخه-قبل)

---

## پیش‌نیازها

### ✅ قبل از شروع:
- [ ] دسترسی به SQL Server (با دسترسی CREATE INDEX)
- [ ] دسترسی به سرور Backend
- [ ] دسترسی به سرور Frontend
- [ ] Backup از دیتابیس گرفته شده است
- [ ] تیم QA آماده تست است
- [ ] زمان deployment در ساعت کاری نیست (اختیاری)

### 🗂️ فایل‌های مورد نیاز:
```
HRMS.API/
├── DatabaseScripts/Performance/
│   ├── UserLoginHistory_Indexes.sql          # ✅ اجباری
│   ├── Test_UserLoginHistory_Performance.sql  # ✅ توصیه می‌شود
│   └── README_UserLoginHistory_Performance.md # 📖 مستندات
├── Controllers/IdentityManager/
│   └── UserLoginHistoryController.cs          # ✅ اجباری (تغییر یافته)
FrontEnd/Dashboard/src/app/
└── modules/auth/components/login-logs-viewer/
    ├── login-logs-viewer.component.ts         # ✅ اجباری (تغییر یافته)
    └── login-logs-viewer.component.html       # ✅ اجباری (تغییر یافته)
```

---

## مراحل استقرار

### 🗓️ زمان‌بندی پیشنهادی:
| مرحله | زمان تخمینی | اولویت |
|-------|-------------|--------|
| Database | 5-10 دقیقه | 🔴 اول |
| Backend | 15-20 دقیقه | 🟠 دوم |
| Frontend | 20-30 دقیقه | 🟡 سوم |
| Testing | 30-45 دقیقه | 🟢 آخر |

**کل زمان:** حدود **1-2 ساعت**

---

## مرحله 1️⃣: Database (اجباری - اجرای اول)

### گام 1: بررسی وضعیت فعلی
```sql
-- اتصال به دیتابیس
USE [CoreHR]
GO

-- بررسی وجود Index های فعلی
SELECT 
    name AS IndexName,
    type_desc AS IndexType
FROM sys.indexes 
WHERE object_id = OBJECT_ID('Identity.User_Login_History')
  AND name LIKE 'IX_%';
GO
```

### گام 2: تست Performance قبل از تغییرات
```bash
# اجرای اسکریپت تست
sqlcmd -S YourServerName -d CoreHR -i Test_UserLoginHistory_Performance.sql -o Before_Results.txt
```

**⚠️ نکته:** نتایج را ذخیره کنید برای مقایسه بعدی!

### گام 3: اجرای Index Script
```bash
# روش 1: از طریق sqlcmd
sqlcmd -S YourServerName -d CoreHR -i UserLoginHistory_Indexes.sql -o Index_Creation.log

# روش 2: از طریق SSMS
# 1. فایل UserLoginHistory_Indexes.sql را باز کنید
# 2. دیتابیس CoreHR را انتخاب کنید
# 3. F5 را بزنید
```

### گام 4: بررسی موفقیت‌آمیز بودن
```sql
-- بررسی ایجاد Index ها
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique,
    i.fill_factor,
    STUFF((
        SELECT ', ' + COL_NAME(ic.object_id, ic.column_id)
        FROM sys.index_columns ic
        WHERE ic.object_id = i.object_id 
          AND ic.index_id = i.index_id
          AND ic.is_included_column = 0
        ORDER BY ic.key_ordinal
        FOR XML PATH('')
    ), 1, 2, '') AS KeyColumns,
    STUFF((
        SELECT ', ' + COL_NAME(ic.object_id, ic.column_id)
        FROM sys.index_columns ic
        WHERE ic.object_id = i.object_id 
          AND ic.index_id = i.index_id
          AND ic.is_included_column = 1
        FOR XML PATH('')
    ), 1, 2, '') AS IncludedColumns
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID('Identity.User_Login_History')
  AND i.name LIKE 'IX_%'
ORDER BY i.name;
GO
```

**✅ انتظار دارید 3 Index جدید ببینید:**
- `IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate`
- `IX_User_Login_History_Success_Logins`
- `IX_User_Login_History_Failed_Logins`

### گام 5: تست Performance بعد از تغییرات
```bash
sqlcmd -S YourServerName -d CoreHR -i Test_UserLoginHistory_Performance.sql -o After_Results.txt
```

### گام 6: مقایسه نتایج
```bash
# Windows PowerShell
Compare-Object (Get-Content Before_Results.txt) (Get-Content After_Results.txt)
```

**✅ نتایج مورد انتظار:**
- ⬇️ کاهش 70-80% در Logical Reads
- ⬇️ کاهش 70-90% در CPU Time
- ✅ Execution Plan باید "Index Seek" نشان دهد (نه "Table Scan")

---

## مرحله 2️⃣: Backend Deployment

### گام 1: Backup از فایل فعلی
```bash
cd HRMS.API/Controllers/IdentityManager
copy UserLoginHistoryController.cs UserLoginHistoryController.cs.backup
```

### گام 2: Build و Test
```bash
cd HRMS.API
dotnet clean
dotnet build

# اجرای Unit Tests (اگر وجود دارد)
dotnet test
```

### گام 3: Publish
```bash
# Development/Staging
dotnet publish -c Release -o ./publish

# Production
dotnet publish -c Release -o ./publish --no-build
```

### گام 4: Deploy به Server
```bash
# روش 1: Manual Copy
xcopy /E /Y ./publish/* \\ProductionServer\HRMS\API\

# روش 2: با استفاده از CI/CD Pipeline
# (بسته به pipeline شما)
```

### گام 5: بررسی سلامت Backend
```bash
# چک کردن سرویس
curl https://your-api-server/api/health

# تست UserLoginHistory endpoint
curl https://your-api-server/api/UserLoginHistory/GetPagedDatasuccess/0/10
```

---

## مرحله 3️⃣: Frontend Deployment

### گام 1: Backup
```bash
cd FrontEnd/Dashboard/src/app/modules/auth/components/login-logs-viewer
copy login-logs-viewer.component.ts login-logs-viewer.component.ts.backup
copy login-logs-viewer.component.html login-logs-viewer.component.html.backup
```

### گام 2: Build
```bash
cd FrontEnd/Dashboard

# نصب dependencies (در صورت نیاز)
npm install

# Build برای Production
npm run build --prod

# یا با configuration مشخص
ng build --configuration production
```

### گام 3: بررسی Build Errors
```bash
# بررسی فایل های build شده
ls dist/

# بررسی سایز bundle ها
ls -lh dist/browser/*.js
```

### گام 4: Deploy به Server
```bash
# روش 1: Manual
xcopy /E /Y dist/* \\ProductionServer\HRMS\Frontend\

# روش 2: با استفاده از CI/CD
# (بسته به pipeline شما)
```

---

## تست و بررسی

### ✅ Checklist تست

#### 1. تست Manual (User Acceptance)
```
□ لاگین به سیستم
□ باز کردن Login Logs Viewer Dialog
□ زمان بارگذاری < 1 ثانیه است
□ تب "تلاش‌های ناموفق" به درستی نمایش داده می‌شود
□ تب "ورودهای موفق" به درستی نمایش داده می‌شود
□ تعویض بین تب‌ها سریع است
□ دکمه "تایید و بستن" بعد از مشاهده هر دو تب فعال می‌شود
□ هیچ error در Console مرورگر وجود ندارد
□ داده‌ها به درستی Paginate می‌شوند
```

#### 2. تست Performance (از Developer Tools)
```javascript
// در Console مرورگر
// قبل از باز کردن Dialog
performance.mark('dialog-start');

// بعد از باز شدن کامل Dialog
performance.mark('dialog-end');
performance.measure('dialog-load', 'dialog-start', 'dialog-end');
console.table(performance.getEntriesByType('measure'));
```

**✅ نتیجه مورد انتظار:** < 1000ms

#### 3. تست Network (Developer Tools > Network Tab)
```
□ تعداد API calls اولیه = 1 (نه 2)
□ API response time < 200ms
□ Response size معقول است
□ هیچ 500 Error وجود ندارد
```

#### 4. تست Database
```sql
-- بررسی استفاده از Index
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- تست query اصلی
DECLARE @UserId BIGINT = 123; -- یک UserId واقعی

SELECT TOP 5 *
FROM [Identity].[User_Login_History]
WHERE AspNetUserId = @UserId 
  AND IsSuccess = 1
  AND IsDeleted = 0
ORDER BY CreateDate DESC;

-- چک کردن Execution Plan (Ctrl+M در SSMS)
```

**✅ انتظار:**
- Logical Reads < 50
- CPU Time < 50ms
- Execution Plan: "Index Seek" ✅ (نه "Table Scan" ❌)

---

## عیب‌یابی

### ❌ مشکل: Index ها ایجاد نشدند

**راه‌حل:**
```sql
-- بررسی permissions
SELECT HAS_PERMS_BY_NAME('Identity.User_Login_History', 'OBJECT', 'ALTER');

-- اگر 0 است، به DBA مراجعه کنید برای دسترسی CREATE INDEX
```

---

### ❌ مشکل: Query هنوز کند است

**راه‌حل:**
```sql
-- 1. Update Statistics
UPDATE STATISTICS [Identity].[User_Login_History] WITH FULLSCAN;

-- 2. بررسی Fragmentation
SELECT 
    avg_fragmentation_in_percent,
    page_count
FROM sys.dm_db_index_physical_stats(
    DB_ID(), 
    OBJECT_ID('Identity.User_Login_History'), 
    NULL, NULL, 'DETAILED'
)
WHERE index_id > 0;

-- 3. اگر fragmentation > 30%، rebuild کنید
ALTER INDEX ALL ON [Identity].[User_Login_History] 
REBUILD WITH (ONLINE = ON);
```

---

### ❌ مشکل: Frontend Component لود نمی‌شود

**راه‌حل:**
```bash
# 1. Clear Browser Cache
# 2. Hard Reload (Ctrl + Shift + R)

# 3. بررسی Console Errors
# F12 > Console Tab

# 4. بررسی Network Tab
# F12 > Network Tab > فیلتر کنید روی "login"
```

---

### ❌ مشکل: Backend Error 500

**راه‌حل:**
```bash
# 1. بررسی Logs
tail -f /path/to/logs/application.log

# 2. بررسی Connection String
# appsettings.json > ConnectionStrings

# 3. تست مستقیم API
curl -X GET "https://your-api/api/UserLoginHistory/GetPagedDatasuccess/0/5" \
     -H "Authorization: Bearer YOUR_TOKEN"
```

---

## برگشت به نسخه قبل (Rollback)

### 🔙 اگر مشکلی پیش آمد:

#### مرحله 1: Frontend Rollback
```bash
cd FrontEnd/Dashboard/src/app/modules/auth/components/login-logs-viewer

# بازگرداندن backup
copy login-logs-viewer.component.ts.backup login-logs-viewer.component.ts
copy login-logs-viewer.component.html.backup login-logs-viewer.component.html

# Build مجدد
npm run build --prod

# Deploy
# (طبق روش deployment شما)
```

#### مرحله 2: Backend Rollback
```bash
cd HRMS.API/Controllers/IdentityManager

# بازگرداندن backup
copy UserLoginHistoryController.cs.backup UserLoginHistoryController.cs

# Build و Deploy مجدد
dotnet build
dotnet publish -c Release -o ./publish
```

#### مرحله 3: Database Rollback (⚠️ توصیه نمی‌شود)
```sql
-- Drop Index ها (فقط در صورت ضرورت)
USE [CoreHR]
GO

DROP INDEX IF EXISTS [IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate] 
ON [Identity].[User_Login_History];

DROP INDEX IF EXISTS [IX_User_Login_History_Success_Logins] 
ON [Identity].[User_Login_History];

DROP INDEX IF EXISTS [IX_User_Login_History_Failed_Logins] 
ON [Identity].[User_Login_History];

GO
```

**⚠️ توجه:** Drop کردن Index ها performance را به حالت قبل بر می‌گرداند!

---

## 🎯 Checklist نهایی

### قبل از Production:
- [ ] تمام تست‌های Manual انجام شد
- [ ] تست Performance قابل قبول است
- [ ] هیچ Error در Logs وجود ندارد
- [ ] Backup از دیتابیس گرفته شد
- [ ] Backup از Backend گرفته شد
- [ ] Backup از Frontend گرفته شد
- [ ] تیم Support مطلع شده است
- [ ] مستندات به‌روز شده است

### بعد از Production:
- [ ] تست Smoke Test انجام شد
- [ ] Performance monitoring فعال است
- [ ] Error monitoring فعال است
- [ ] تیم QA تاییدیه داده است
- [ ] Rollback plan آماده است

---

## 📞 پشتیبانی

در صورت بروز مشکل:
1. ✅ ابتدا بخش [عیب‌یابی](#عیب‌یابی) را بخوانید
2. ✅ Log ها را بررسی کنید
3. ✅ با تیم Development تماس بگیرید

---

## 📚 مستندات مرتبط

- `README_UserLoginHistory_Performance.md` - جزئیات فنی کامل
- `Test_UserLoginHistory_Performance.sql` - اسکریپت تست
- `PERFORMANCE_IMPROVEMENTS.md` - خلاصه تغییرات

---

**آخرین بروزرسانی:** 1404/08/30 (2025-11-20)  
**نسخه:** 1.0.0  
**وضعیت:** ✅ آماده استقرار

