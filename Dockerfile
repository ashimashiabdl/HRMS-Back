# syntax=docker/dockerfile:1
# Multi-stage build for HRMS.API (.NET 10, Linux)

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY HRMS.Core.All.sln ./
COPY HRMS.API/HRMS.API.csproj HRMS.API/
COPY HR.SharedKernel/HR.SharedKernel.csproj HR.SharedKernel/
COPY HR.BaseInfo.Infrastructure/HR.BaseInfo.Infrastructure.csproj HR.BaseInfo.Infrastructure/
COPY Hr.Employee.infrastructure/Hr.Employee.infrastructure.csproj Hr.Employee.infrastructure/
COPY HR.FormulaEngine.Infrastructure/HR.FormulaEngine.Infrastructure.csproj HR.FormulaEngine.Infrastructure/
COPY HR.Identity.Infrastructure/HR.Identity.Infrastructure.csproj HR.Identity.Infrastructure/
COPY HR.Organisation.Infrastructure/HR.Organisation.Infrastructure.csproj HR.Organisation.Infrastructure/
COPY HR.Order.Infrastructure/HR.Order.Infrastructure.csproj HR.Order.Infrastructure/
COPY HR.Payroll.Infrastructure/HR.Payroll.Infrastructure.csproj HR.Payroll.Infrastructure/
COPY HR.Report.Infrastructure/HR.Report.Infrastructure.csproj HR.Report.Infrastructure/
COPY HR.WorkFlow.Infrastructure/HR.WorkFlow.Infrastructure.csproj HR.WorkFlow.Infrastructure/
COPY HR.Attendance.Infrastructure/HR.Attendance.Infrastructure.csproj HR.Attendance.Infrastructure/
COPY Hr.SystemSetting.Infrastructure/Hr.SystemSetting.Infrastructure.csproj Hr.SystemSetting.Infrastructure/
COPY HR.BaseInfo.Core/HR.BaseInfo.Core.csproj HR.BaseInfo.Core/
COPY HR.Employee.Core/HR.Employee.Core.csproj HR.Employee.Core/
COPY HR.FormulaEngine.Core/HR.FormulaEngine.Core.csproj HR.FormulaEngine.Core/
COPY HR.Identity.Core/HR.Identity.Core.csproj HR.Identity.Core/
COPY HR.Organisation.Core/HR.Organisation.Core.csproj HR.Organisation.Core/
COPY HR.Order.Core/HR.Order.Core.csproj HR.Order.Core/
COPY HR.Payroll.Core/HR.Payroll.Core.csproj HR.Payroll.Core/
COPY HR.Report.Core/HR.Report.Core.csproj HR.Report.Core/
COPY HR.WorkFlow.Core/HR.WorkFlow.Core.csproj HR.WorkFlow.Core/
COPY HR.Attendance.Core/HR.Attendance.Core.csproj HR.Attendance.Core/
COPY Hr.SystemSetting.Core/Hr.SystemSetting.Core.csproj Hr.SystemSetting.Core/

# Stimulsoft DLLs (required by Order/Report infrastructure)
# Run docker/prepare-stimulsoft.ps1 on host if these files are missing locally.
COPY Assets/StimulDLL/Stimulsoft.Base.dll Assets/StimulDLL/Stimulsoft.Report.dll Assets/StimulDLL/

RUN dotnet restore HRMS.API/HRMS.API.csproj -p:DockerBuild=true

COPY . .

RUN test -f Assets/StimulDLL/Stimulsoft.Base.dll \
    && test -f Assets/StimulDLL/Stimulsoft.Report.dll \
    || (echo "ERROR: Stimulsoft DLLs missing in build context. Run docker/prepare-stimulsoft.ps1 before docker build." && exit 1)

RUN dotnet build HRMS.API/HRMS.API.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    -p:DockerBuild=true \
    --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish HRMS.API/HRMS.API.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:DockerBuild=true \
    --no-restore

FROM base AS final
WORKDIR /app

# curl: health checks | libgdiplus: System.Drawing (captcha / reports)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl libgdiplus \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

RUN mkdir -p /app/logs /app/InsuranceTemp /app/wwwroot \
    && chmod -R 755 /app

# Visual Studio Container Tools
LABEL com.microsoft.visualstudio.devimage.system.runtime="net10.0"

ENV HttpPort=5000 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:5000/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "HRMS.API.dll"]
