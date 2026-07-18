# راهنمای اجرای HRMS با Docker

این راهنما نحوه اجرای **HRMS.API** (.NET 10) و **CoreHr.Dashboard** (Angular 21) با Docker Compose را توضیح می‌دهد.

## پیش‌نیازها

- **Docker Engine** (ویندوز/لینوکس) یا **Docker Desktop**
- **Docker Compose** v2 (`docker compose`)
- حداقل 4GB RAM آزاد
- **Stimulsoft DLLها**: قبل از build اجرا کنید: `powershell -File docker/prepare-stimulsoft.ps1`
  (یا DLLها را دستی در `Assets/StimulDLL/` قرار دهید)

---

## ساختار فایل‌های Docker

| فایل | توضیح |
|------|--------|
| `Dockerfile` | ساخت image لینوکسی HRMS.API (multi-stage, .NET 10) |
| `FrontEnd/Dashboard/Dockerfile` | ساخت Dashboard (Angular + nginx) |
| `docker-compose.yml` | SQL Server + API + Dashboard |
| `docker-compose.api-only.yml` | فقط API |
| `docker-compose.dashboard-only.yml` | فقط Dashboard |
| `.dockerignore` | فایل‌های حذف‌شده از build context API |
| `.env.example` | نمونه متغیرهای محیطی — کپی به `.env` |

تنظیمات Visual Studio / Rider در `HRMS.API/HRMS.API.csproj`:
- `DockerfileFile` → Dockerfile ریشه solution
- `DockerBuild=true` → build لینوکسی با `net10.0` (بدون WinForms)

---

## راه‌اندازی سریع

### 1. تنظیم `.env`

```powershell
cd C:\HRMS
copy .env.example .env
# SA_PASSWORD، JWT_KEY و HRMS_CONNECTION را ویرایش کنید
```

### 2. آماده‌سازی Stimulsoft (یک‌بار)

```powershell
powershell -File docker/prepare-stimulsoft.ps1
```

### 3. اجرای کامل (SQL + API + Dashboard)

```powershell
docker compose up -d --build
docker compose ps
docker compose logs -f hrms.api
docker compose logs -f hrms.dashboard
```

### 3. فقط API (SQL Server جدا)

```powershell
docker compose -f docker-compose.api-only.yml up -d --build
```

Connection string پیش‌فرض به `host.docker.internal` (SQL روی ویندوز host) اشاره می‌کند.

### 4. فقط Dashboard (API جدا)

```powershell
docker compose -f docker-compose.dashboard-only.yml up -d --build
```

جزئیات بیشتر: `FrontEnd/Dashboard/DOCKER.md`

---

## دسترسی

| سرویس | آدرس |
|--------|------|
| Dashboard | http://localhost:4201 |
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| SQL Server | localhost:1433 (user: `sa`, password: `SA_PASSWORD`) |

---

## دستورات مفید

```powershell
# توقف
docker compose down

# توقف + حذف volume دیتابیس
docker compose down -v

# بازسازی image
docker compose build --no-cache hrms.api
docker compose up -d hrms.api

# ورود به container
docker exec -it hrms-api bash
docker exec -it hrms-sqlserver bash
```

---

## اجرا با Visual Studio

1. پروفایل **Container (Dockerfile)** یا **Docker** را از dropdown Debug انتخاب کنید.
2. **Fast Mode** غیرفعال است (`ContainerDevelopmentMode=Regular`) — build داخل Linux container با `net10.0` انجام می‌شود.
3. اگر خطای `CoreCLR started event` دیدید: Rebuild Solution، Docker Desktop را restart کنید، و `docker system prune` (اختیاری).
4. SQL Server روی host باید در دسترس باشد (`host.docker.internal`) یا `docker compose up sqlserver` را جدا اجرا کنید.
5. پورت container: **5000** (در `launchSettings.json` تنظیم شده).

---

## ساخت مستقیم با Dockerfile

```powershell
# روش پیشنهادی (Stimulsoft را خودکار آماده می‌کند)
powershell -File docker/build-api.ps1

# یا دستی:
powershell -File docker/prepare-stimulsoft.ps1
docker build -t hr-payroll:local -f Dockerfile .
```

---

## ایجاد دیتابیس

پس از بالا آمدن SQL Server:

```powershell
docker exec -it hrms-sqlserver /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -P "YourStrong@Password123" -C `
  -Q "IF DB_ID('CoreHR') IS NULL CREATE DATABASE CoreHR"
```

---

## Volumeها

| مسیر host | مسیر container | کاربرد |
|-----------|----------------|--------|
| `hrms_sql_data` (named volume) | `/var/opt/mssql` | داده SQL Server |
| `./HRMS.API/wwwroot` | `/app/wwwroot` | فایل‌های آپلود |
| `./logs` | `/app/logs` | لاگ NLog |
| `./HRMS.API/InsuranceTemp` | `/app/InsuranceTemp` | فایل‌های موقت بیمه |

---

## عیب‌یابی

### Docker daemon در دسترس نیست

```
failed to connect to the docker API at npipe://...
```

Docker Desktop را اجرا کنید یا سرویس Docker Engine را start کنید:

```powershell
Start-Service docker
```

### خطای Stimulsoft در build

DLLها را از `Assets/StimulDLL/*.rar` استخراج کنید تا این فایل‌ها وجود داشته باشند:

- `Assets/StimulDLL/Stimulsoft.Base.dll`
- `Assets/StimulDLL/Stimulsoft.Report.dll`

### API به SQL وصل نمی‌شود

- در `docker-compose.yml` نام سرور باید `sqlserver` باشد.
- منتظر بمانید تا health check SQL سبز شود: `docker compose ps`
- در حالت api-only از `host.docker.internal` استفاده کنید.

### پورت 5000 اشغال است

در `.env`: `API_PORT=5001` (یا پورت دیگر).

### Pull image در ایران

Registry mirror در `%USERPROFILE%\.docker\daemon.json`:

```json
{
  "registry-mirrors": [
    "https://registry.docker.ir",
    "https://docker.arvancloud.ir"
  ]
}
```

Images مورد نیاز:

```powershell
docker pull mcr.microsoft.com/dotnet/aspnet:10.0
docker pull mcr.microsoft.com/dotnet/sdk:10.0
docker pull mcr.microsoft.com/mssql/server:2022-latest
```

---

## نکات امنیتی

- رمز `SA_PASSWORD` و `JWT_KEY` را در Production حتماً تغییر دهید.
- فایل `.env` را commit نکنید (در `.gitignore` است).
- برای Production از reverse proxy با HTTPS استفاده کنید.
