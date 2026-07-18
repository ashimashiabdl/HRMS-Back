# راهنمای تنظیمات IIS برای DPAPI

## مشکل: ApplicationPoolIdentity و خطای رمزنگاری

### تشخیص مشکل

اگر در لاگ‌ها این موارد را می‌بینید:
```
UserName: "IIS APPPOOL\\YourAppPoolName"
```

یعنی Application Pool شما روی `ApplicationPoolIdentity` است و احتمالاً:
- ❌ CurrentUser DPAPI کار نمی‌کند
- ⚠️ LocalMachine ممکن است مشکل داشته باشد

---

## راه‌حل 1: فعال کردن Load User Profile ⭐ (توصیه می‌شود)

### مراحل در IIS Manager:

1. **باز کردن IIS Manager**
   ```
   Start → Run → inetmgr
   ```

2. **رفتن به Application Pools**
   ```
   Server → Application Pools → [نام Application Pool شما]
   ```

3. **باز کردن Advanced Settings**
   ```
   Right Click → Advanced Settings
   ```

4. **تغییر Load User Profile**
   ```
   Process Model → Load User Profile → True
   ```

5. **Recycle کردن Application Pool**
   ```
   Right Click → Recycle
   ```

### با PowerShell (برای اتوماسیون):

```powershell
Import-Module WebAdministration

# تنظیم Load User Profile
Set-ItemProperty "IIS:\AppPools\YourAppPoolName" -Name processModel.loadUserProfile -Value $true

# Recycle
Restart-WebAppPool -Name "YourAppPoolName"

# بررسی تنظیمات
Get-ItemProperty "IIS:\AppPools\YourAppPoolName" -Name processModel.loadUserProfile
```

### نتیجه:
- ✅ CurrentUser DPAPI کار می‌کند
- ✅ User Profile لود می‌شود
- ✅ Master Keys در دسترس است

---

## راه‌حل 2: استفاده فقط از LocalMachine

اگر نمی‌خواهید Load User Profile را فعال کنید:

### در کد خود:
```csharp
// فقط از LocalMachine استفاده کنید
var encrypted = ConnectionStringProtector.ProtectWithLocalMachine(connectionString);
```

### تنظیمات دسترسی:

```powershell
# دادن دسترسی به IIS_IUSRS
$path = "$env:ALLUSERSPROFILE\Microsoft\Crypto\RSA\MachineKeys"
icacls $path /grant "IIS_IUSRS:(RX)"

# یا دادن دسترسی به Application Pool خاص
icacls $path /grant "IIS APPPOOL\YourAppPoolName:(RX)"
```

### نتیجه:
- ✅ LocalMachine DPAPI کار می‌کند
- ❌ CurrentUser کار نمی‌کند (اما نیازی نیست)

---

## راه‌حل 3: تغییر Identity به حساب کاربری خاص

### مراحل:

1. **ایجاد یک Service Account**
   ```
   Computer Management → Local Users and Groups → Users
   → New User → "IIS_ServiceAccount"
   ```

2. **تنظیم در IIS**
   ```
   Application Pool → Advanced Settings
   → Process Model → Identity → Custom Account
   → Username: .\IIS_ServiceAccount
   → Password: [رمز عبور]
   ```

3. **دادن دسترسی‌های لازم**
   ```powershell
   # دسترسی به وب سایت
   icacls "C:\inetpub\wwwroot\YourSite" /grant "IIS_ServiceAccount:(OI)(CI)F"
   
   # دسترسی به MachineKeys
   icacls "$env:ALLUSERSPROFILE\Microsoft\Crypto\RSA\MachineKeys" /grant "IIS_ServiceAccount:(RX)"
   ```

### مزایا:
- ✅ کنترل کامل روی دسترسی‌ها
- ✅ هر دو LocalMachine و CurrentUser کار می‌کنند

### معایب:
- ❌ پیچیده‌تر برای مدیریت
- ❌ نیاز به مدیریت رمز عبور

---

## مقایسه روش‌ها

| روش | سادگی | امنیت | CurrentUser | LocalMachine |
|-----|--------|-------|-------------|--------------|
| Load User Profile = True | ⭐⭐⭐ | ⭐⭐⭐ | ✅ | ✅ |
| فقط LocalMachine | ⭐⭐⭐ | ⭐⭐ | ❌ | ✅ |
| Custom Account | ⭐ | ⭐⭐⭐ | ✅ | ✅ |

---

## تست تنظیمات

بعد از اعمال هر تغییر، از endpoint زیر استفاده کنید:

```http
POST /api/OrderAdmin/ProtectConnectionString
Content-Type: application/json

{
  "plain": "Server=.;Database=TestDB;Integrated Security=true;"
}
```

### پاسخ موفق:
```json
{
  "dpapi_lm": "enc::dpapi-lm::AQAAANCMnd...",
  "dpapi_cu": "enc::dpapi-cu::AQAAANCMnd...",
  "success_lm": true,
  "success_cu": true,
  "errors": null,
  "diagnostics": {
    "MachineName": "SERVER02",
    "UserName": "IIS APPPOOL\\YourAppPool",
    "LoadUserProfile": "Enabled"
  }
}
```

### پاسخ با مشکل:
```json
{
  "dpapi_lm": "enc::dpapi-lm::AQAAANCMnd...",
  "dpapi_cu": null,
  "success_lm": true,
  "success_cu": false,
  "errors": [
    "CurrentUser DPAPI Error: Key not valid for use in specified state | HResult: 0x80090016"
  ]
}
```

---

## عیب‌یابی رایج

### خطا: 0x80090016 (NTE_BAD_KEYSET)
**علت:** User Profile لود نشده  
**راه‌حل:** Load User Profile = True

### خطا: 0x80090005 (NTE_BAD_DATA)
**علت:** دسترسی به Master Keys نیست  
**راه‌حل:** بررسی دسترسی‌های فایل سیستم

### خطا: 0x80070005 (E_ACCESSDENIED)
**علت:** دسترسی به MachineKeys نیست  
**راه‌حل:** 
```powershell
icacls "$env:ALLUSERSPROFILE\Microsoft\Crypto\RSA\MachineKeys" /grant "IIS_IUSRS:(RX)"
```

---

## توصیه نهایی برای Production

### برای سرور شما (سرور 2):

1. ✅ **فعال کردن Load User Profile** (ساده‌ترین راه)
2. ✅ **Recycle کردن Application Pool**
3. ✅ **تست با endpoint**
4. ✅ **بررسی لاگ‌ها برای اطمینان**

### اگر Load User Profile نمی‌توانید فعال کنید:

1. ✅ **فقط از LocalMachine استفاده کنید**
2. ✅ **دسترسی‌های MachineKeys را بررسی کنید**
3. ✅ **در appsettings فقط از `dpapi-lm` استفاده کنید**

---

## اسکریپت کامل برای تنظیم سریع

```powershell
# نام Application Pool خود را وارد کنید
$appPoolName = "YourAppPoolName"

# فعال کردن Load User Profile
Import-Module WebAdministration
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name processModel.loadUserProfile -Value $true

# دادن دسترسی به MachineKeys
$machineKeysPath = "$env:ALLUSERSPROFILE\Microsoft\Crypto\RSA\MachineKeys"
icacls $machineKeysPath /grant "IIS APPPOOL\$($appPoolName):(RX)"

# Recycle
Restart-WebAppPool -Name $appPoolName

Write-Host "✅ تنظیمات اعمال شد. Application Pool را Recycle کردم." -ForegroundColor Green
Write-Host "🔍 لطفاً endpoint را تست کنید." -ForegroundColor Yellow
```

---

## لاگ‌های مفید برای بررسی

با تغییرات جدید، این اطلاعات در پاسخ API و لاگ‌ها موجود است:

```
[a3f2b1c4] اطلاعات محیط: {
  "MachineName": "SERVER02",
  "OSVersion": "Microsoft Windows NT 10.0.17763.0",
  "UserName": "IIS APPPOOL\\DefaultAppPool",
  "UserDomainName": "SERVER02",
  "ProcessId": 12345
}
```

این اطلاعات به شما کمک می‌کند تا دقیقاً بفهمید چه کاربری در حال اجرای کد است.

