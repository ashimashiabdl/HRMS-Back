# راهنمای عیب‌یابی DPAPI (Data Protection API)

## مشکلات رایج در سرورهای مختلف

### مشکل: خطای رمزنگاری در سرور دوم

**علت‌های احتمالی:**

1. **🔴 ApplicationPoolIdentity (مهم‌ترین علت)**
   - اگر Application Pool روی `ApplicationPoolIdentity` است
   - User Profile به صورت پیش‌فرض لود نمی‌شود
   - Master Keys در دسترس نیست
   - **راه‌حل سریع:** Load User Profile = True در IIS
   - **جزئیات کامل:** مشاهده فایل `IIS_CONFIGURATION_GUIDE.md`

2. **تفاوت در User Profile**
   - DPAPI با استفاده از کلیدهای خاص کاربر/ماشین کار می‌کند
   - اگر Application Pool با کاربر متفاوتی اجرا شود، `CurrentUser` کار نمی‌کند
   - راه‌حل: از `LocalMachine` استفاده کنید

3. **مشکل دسترسی به User Profile**
   - IIS Application Pool ممکن است Load User Profile = False باشد
   - راه‌حل: در IIS → Application Pool → Advanced Settings → Load User Profile = True

4. **تفاوت در نسخه Windows**
   - برخی نسخه‌های Windows Server محدودیت‌های امنیتی بیشتری دارند
   - Windows Server Core ممکن است مشکل داشته باشد

5. **مشکل Roaming Profile**
   - اگر کاربر Roaming Profile داشته باشد، ممکن است کلیدها sync نشوند

6. **خطای 0x80090005 (NTE_BAD_DATA)**
   - معمولاً به معنای عدم دسترسی به Master Key است
   - بررسی کنید: `%APPDATA%\Microsoft\Protect` یا `%ALLUSERSPROFILE%\Microsoft\Crypto\RSA\MachineKeys`

## راه‌حل‌های پیشنهادی

### 1. استفاده از LocalMachine (توصیه می‌شود)
```csharp
var encrypted = ConnectionStringProtector.ProtectWithLocalMachine(connectionString);
```

**مزایا:**
- مستقل از کاربر
- در تمام Application Pools کار می‌کند

**معایب:**
- همه کاربران روی ماشین می‌توانند رمزگشایی کنند

### 2. تنظیمات IIS
```
IIS Manager → Application Pools → [Your Pool] → Advanced Settings:
- Load User Profile: True
- Identity: مطمئن شوید کاربر مناسب است
```

### 3. بررسی دسترسی‌ها
```powershell
# بررسی دسترسی به MachineKeys
icacls "%ALLUSERSPROFILE%\Microsoft\Crypto\RSA\MachineKeys"

# باید IIS_IUSRS یا کاربر Application Pool دسترسی داشته باشد
icacls "%ALLUSERSPROFILE%\Microsoft\Crypto\RSA\MachineKeys" /grant "IIS_IUSRS:(RX)"
```

### 4. استفاده از Entropy برای امنیت بیشتر
اگر نیاز به امنیت بیشتر دارید، می‌توانید از entropy استفاده کنید:

```csharp
// در آینده می‌توان این را اضافه کرد:
var entropy = Encoding.UTF8.GetBytes("YourSecretSalt");
var cipher = ProtectedData.Protect(bytes, entropy, DataProtectionScope.LocalMachine);
```

## لاگ‌های مفید

با تغییرات جدید، لاگ‌های زیر در دسترس هستند:

1. **RequestId**: شناسه یکتا برای هر درخواست
2. **Environment Info**: اطلاعات سیستم‌عامل، کاربر، ماشین
3. **HResult Codes**: کد خطای دقیق Windows
4. **Step-by-step logs**: لاگ هر مرحله از رمزنگاری

## کدهای خطای رایج

| HResult | معنی | راه‌حل |
|---------|------|--------|
| 0x80090005 | NTE_BAD_DATA | مشکل دسترسی به Master Key |
| 0x80090016 | NTE_BAD_KEYSET | User Profile لود نشده |
| 0x80070005 | E_ACCESSDENIED | مشکل دسترسی فایل سیستم |

## تست و عیب‌یابی

برای تست، از endpoint زیر استفاده کنید:
```
POST /api/OrderAdmin/ProtectConnectionString
{
  "plain": "Server=.;Database=Test;..."
}
```

پاسخ شامل موارد زیر است:
- `dpapi_lm`: رمزنگاری شده با LocalMachine
- `dpapi_cu`: رمزنگاری شده با CurrentUser
- `success_lm`: آیا LocalMachine موفق بود؟
- `success_cu`: آیا CurrentUser موفق بود؟
- `errors`: فهرست خطاها
- `diagnostics`: اطلاعات محیطی
- `requestId`: شناسه درخواست برای جستجو در لاگ‌ها

## توصیه نهایی

**برای محیط Production:**
1. از `dpapi-lm` (LocalMachine) استفاده کنید
2. مطمئن شوید Application Pool با Identity مناسب اجرا می‌شود
3. Load User Profile را فعال کنید
4. دسترسی‌های لازم را بررسی کنید
5. لاگ‌ها را برای عیب‌یابی نگه دارید

**امنیت:**
- فایل‌های رمزنگاری شده را در appsettings.json یا متغیرهای محیطی نگه دارید
- هرگز ConnectionString خام را در کد یا Git commit نکنید
- برای هر سرور، رمزنگاری را جداگانه انجام دهید

