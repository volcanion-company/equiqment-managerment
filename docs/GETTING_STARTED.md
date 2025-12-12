# 🚀 Getting Started

## Equipment Management System - Quick Setup Guide

> **Platform:** .NET 9.0  
> **Database:** PostgreSQL 17  
> **IDE:** Visual Studio 2022 / VS Code / Rider

Welcome! This guide will help you set up and run the Equipment Management System on your local machine in **under 15 minutes**.

---

## 📋 Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Testing](#testing)
- [Docker Setup](#docker-setup)
- [Troubleshooting](#troubleshooting)
- [Next Steps](#next-steps)

---

## ✅ Prerequisites

### Required Software

| Software | Version | Download Link |
|----------|---------|---------------|
| .NET SDK | 9.0+ | [Download](https://dotnet.microsoft.com/download/dotnet/9.0) |
| PostgreSQL | 17+ | [Download](https://www.postgresql.org/download/) |
| Git | Latest | [Download](https://git-scm.com/downloads) |

### Optional but Recommended

| Tool | Purpose | Download Link |
|------|---------|---------------|
| Visual Studio 2022 | Full IDE | [Download](https://visualstudio.microsoft.com/) |
| VS Code | Lightweight editor | [Download](https://code.visualstudio.com/) |
| JetBrains Rider | Premium IDE | [Download](https://www.jetbrains.com/rider/) |
| Docker Desktop | Containerization | [Download](https://www.docker.com/products/docker-desktop) |
| Postman | API testing | [Download](https://www.postman.com/downloads/) |
| pgAdmin 4 | PostgreSQL GUI | [Download](https://www.pgadmin.org/download/) |

### Verify Installation

Open terminal and verify:

```bash
# Check .NET version
dotnet --version
# Expected: 9.0.0 or higher

# Check PostgreSQL
psql --version
# Expected: psql (PostgreSQL) 17.x

# Check Git
git --version
# Expected: git version 2.x.x
```

---

## 📥 Installation

### 1. Clone the Repository

```bash
# Clone via HTTPS
git clone https://github.com/volcanion-company/volcanion-device-management.git

# Or via SSH
git clone git@github.com:volcanion-company/volcanion-device-management.git

# Navigate to project directory
cd volcanion-device-management
```

### 2. Restore Dependencies

```bash
# Restore NuGet packages
dotnet restore

# Verify all projects build
dotnet build
```

**Expected output**:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.23
```

---

## ⚙️ Configuration

### 1. Database Connection String

**Option A: Using appsettings.json** (Recommended for development)

Edit `src/presentations/EquipmentManagement.WebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EquipmentManagementDB;Username=postgres;Password=your_password_here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Option B: Using User Secrets** (Recommended for security)

```bash
# Navigate to WebAPI project
cd src/presentations/EquipmentManagement.WebAPI

# Initialize user secrets
dotnet user-secrets init

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=EquipmentManagementDB;Username=postgres;Password=your_password_here"

# Verify
dotnet user-secrets list
```

**Option C: Using Environment Variables**

```bash
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=EquipmentManagementDB;Username=postgres;Password=your_password"

# Linux/macOS
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=EquipmentManagementDB;Username=postgres;Password=your_password"
```

### 2. PostgreSQL Setup

**Create Database**:

```sql
-- Connect to PostgreSQL
psql -U postgres

-- Create database
CREATE DATABASE "EquipmentManagementDB";

-- Create user (optional)
CREATE USER equipment_admin WITH PASSWORD 'secure_password';

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE "EquipmentManagementDB" TO equipment_admin;

-- Exit
\q
```

**Or use pgAdmin**:
1. Open pgAdmin 4
2. Right-click **Databases** → **Create** → **Database**
3. Name: `EquipmentManagementDB`
4. Owner: `postgres`
5. Click **Save**

---

## 🗄️ Database Setup

### Option 1: Using EF Core Migrations (Recommended)

```bash
# Navigate to Infrastructure project
cd src/libs/EquipmentManagement.Infrastructure

# Create initial migration
dotnet ef migrations add InitialCreate --startup-project ../../presentations/EquipmentManagement.WebAPI

# Apply migration to database
dotnet ef database update --startup-project ../../presentations/EquipmentManagement.WebAPI
```

**Expected output**:
```
Build started...
Build succeeded.
Applying migration '20250112000000_InitialCreate'.
Done.
```

### Option 2: Using Docker Compose (Easiest)

```bash
# From project root
docker-compose up -d

# This will:
# 1. Start PostgreSQL container
# 2. Create database
# 3. Apply migrations
# 4. Start API container
```

### Verify Database

```sql
-- Connect to database
psql -U postgres -d EquipmentManagementDB

-- List tables
\dt

-- Expected output:
-- Equipments
-- Assignments
-- Warehouses
-- WarehouseItems
-- WarehouseTransactions
-- MaintenanceRequests
-- LiquidationRequests
-- AuditRecords
```

---

## ▶️ Running the Application

### Option 1: Using .NET CLI

```bash
# Navigate to WebAPI project
cd src/presentations/EquipmentManagement.WebAPI

# Run the application
dotnet run
```

**Expected output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7072
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5072
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Option 2: Using Visual Studio 2022

1. Open `volcanion-device-management.sln`
2. Set `EquipmentManagement.WebAPI` as startup project
3. Press **F5** or click **Run**

### Option 3: Using VS Code

1. Open project folder in VS Code
2. Install **C# Dev Kit** extension
3. Press **F5** to run with debugger
4. Or use terminal: `dotnet run --project src/presentations/EquipmentManagement.WebAPI`

### Option 4: Using Docker

```bash
# Build and run with Docker Compose
docker-compose up --build

# Or run individual container
docker build -t equipment-management-api .
docker run -p 7072:8080 equipment-management-api
```

### Access the Application

| Service | URL | Description |
|---------|-----|-------------|
| API | https://localhost:7072 | Main API endpoint |
| Swagger UI | https://localhost:7072/swagger | Interactive API docs |
| Health Check | https://localhost:7072/health | Health status |

**Test the API**:
```bash
# Using curl
curl https://localhost:7072/api/equipments

# Using PowerShell
Invoke-RestMethod -Uri https://localhost:7072/api/equipments
```

---

## 🧪 Testing

### Run Unit Tests

```bash
# From project root
dotnet test

# With detailed output
dotnet test --logger "console;verbosity=detailed"

# With code coverage
dotnet test --collect:"XPlat Code Coverage"
```

**Expected output**:
```
Passed!  - Failed:     0, Passed:    75, Skipped:     0, Total:    75, Duration: 1.2 s
```

### Run Specific Test Project

```bash
cd tests/EquipmentManagement.UnitTests
dotnet test
```

### Run Tests in Watch Mode

```bash
dotnet watch test
```

---

## 🐳 Docker Setup

### Quick Start with Docker Compose

**1. Start all services**:
```bash
docker-compose up -d
```

**2. View logs**:
```bash
docker-compose logs -f
```

**3. Stop services**:
```bash
docker-compose down
```

**4. Rebuild after code changes**:
```bash
docker-compose up --build
```

### What Docker Compose Provides

The `docker-compose.yml` includes:

- **PostgreSQL 17**: Database server (port 5432)
- **API Service**: .NET 9 Web API (port 7072)
- **Redis** (optional): Caching layer (port 6379)
- **pgAdmin** (optional): Database management UI (port 5050)

### Docker Compose Configuration

```yaml
services:
  postgres:
    image: postgres:17
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: EquipmentManagementDB
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    volumes:
      - postgres-data:/var/lib/postgresql/data

  api:
    build: .
    ports:
      - "7072:8080"
    depends_on:
      - postgres
    environment:
      ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=EquipmentManagementDB;Username=postgres;Password=postgres"
```

---

## 🔍 Troubleshooting

### Common Issues

#### Issue: "The type or namespace name 'MediatR' could not be found"

**Solution**:
```bash
dotnet restore
dotnet build
```

#### Issue: "Unable to connect to database"

**Check PostgreSQL is running**:
```bash
# Windows
Get-Service -Name postgresql*

# Linux/macOS
sudo systemctl status postgresql

# Docker
docker ps | grep postgres
```

**Verify connection string**:
- Check username/password
- Verify port (default: 5432)
- Ensure database exists

#### Issue: "Migration already applied"

**Solution**:
```bash
# Check migration status
dotnet ef migrations list --startup-project src/presentations/EquipmentManagement.WebAPI

# Remove last migration (if needed)
dotnet ef migrations remove --startup-project src/presentations/EquipmentManagement.WebAPI

# Recreate migration
dotnet ef migrations add InitialCreate --startup-project src/presentations/EquipmentManagement.WebAPI
```

#### Issue: "Port 7072 already in use"

**Solution**:
```bash
# Windows - Find process using port
netstat -ano | findstr :7072
taskkill /PID <process_id> /F

# Linux/macOS
lsof -i :7072
kill -9 <process_id>

# Or change port in launchSettings.json
```

#### Issue: SSL Certificate errors

**Solution**:
```bash
# Trust development certificate
dotnet dev-certs https --trust

# Or disable HTTPS redirect in Program.cs (development only)
```

### Enable Detailed Logging

Edit `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.AspNetCore": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

### Reset Database

```bash
# Drop and recreate database
dotnet ef database drop --force --startup-project src/presentations/EquipmentManagement.WebAPI
dotnet ef database update --startup-project src/presentations/EquipmentManagement.WebAPI
```

---

## 📚 Next Steps

### 1. Explore the API

- **Swagger UI**: https://localhost:7072/swagger
- **Import Postman collections** from `/postman/` directory
- **Try CRUD operations** on Equipments

### 2. Read Documentation

- [Architecture Guide](ARCHITECTURE.md) - Understand system design
- [Development Guidelines](GUIDELINES.md) - Coding standards
- [API Reference](API_REFERENCE.md) - Detailed endpoint documentation

### 3. Set Up IDE

**Visual Studio 2022**:
- Install **C# extensions**
- Configure **code style** (EditorConfig)
- Set up **live unit testing**

**VS Code**:
- Install extensions:
  - C# Dev Kit
  - .NET Extension Pack
  - REST Client
  - PostgreSQL

**JetBrains Rider**:
- Import code style settings
- Configure database tools
- Set up run configurations

### 4. Create Sample Data

```bash
# Run seed script (if available)
dotnet run --project src/presentations/EquipmentManagement.WebAPI -- seed

# Or use Postman collection:
# Import "Sample-Data-Creation.postman_collection.json"
# Run collection to create test data
```

### 5. Start Development

**Create a new feature**:
```bash
# Create feature branch
git checkout -b feature/my-new-feature

# Make changes
# ... code ...

# Run tests
dotnet test

# Commit
git add .
git commit -m "feat(module): add new feature"

# Push
git push origin feature/my-new-feature
```

### 6. Join the Community

- **Report bugs**: [GitHub Issues](https://github.com/volcanion-company/volcanion-device-management/issues)
- **Contribute**: See [CONTRIBUTING.md](../CONTRIBUTING.md)
- **Ask questions**: [Discussions](https://github.com/volcanion-company/volcanion-device-management/discussions)

---

## 🎯 Quick Reference

### Essential Commands

```bash
# Build
dotnet build

# Run
dotnet run --project src/presentations/EquipmentManagement.WebAPI

# Test
dotnet test

# Clean
dotnet clean

# Restore
dotnet restore

# Watch (auto-rebuild on changes)
dotnet watch run --project src/presentations/EquipmentManagement.WebAPI

# Create migration
dotnet ef migrations add MigrationName --startup-project src/presentations/EquipmentManagement.WebAPI

# Update database
dotnet ef database update --startup-project src/presentations/EquipmentManagement.WebAPI
```

### Project Structure Quick Reference

```
src/
├── libs/                              # Business logic libraries
│   ├── EquipmentManagement.Domain/    # Entities, enums, interfaces
│   ├── EquipmentManagement.Application/ # CQRS handlers, validators
│   └── EquipmentManagement.Infrastructure/ # EF Core, repositories
└── presentations/                     # API layer
    └── EquipmentManagement.WebAPI/    # Controllers, middleware

tests/
└── EquipmentManagement.UnitTests/    # Unit tests
```

---

## ✅ Checklist

Before you start coding, ensure:

- [x] .NET 9 SDK installed
- [x] PostgreSQL 17 running
- [x] Repository cloned
- [x] Dependencies restored (`dotnet restore`)
- [x] Database created
- [x] Migrations applied
- [x] Application runs successfully
- [x] Swagger UI accessible
- [x] Tests passing (`dotnet test`)
- [x] IDE configured

---

## 🆘 Need Help?

- **Documentation**: Check [Architecture](ARCHITECTURE.md), [Guidelines](GUIDELINES.md), [API Reference](API_REFERENCE.md)
- **Issues**: [GitHub Issues](https://github.com/volcanion-company/volcanion-device-management/issues)
- **Discussions**: [GitHub Discussions](https://github.com/volcanion-company/volcanion-device-management/discussions)
- **Email**: support@volcanion-company.com

---

**Happy Coding! 🎉**

---

**Version**: 1.0.0  
**Last Updated**: December 12, 2025  
**Maintainer**: Development Team
