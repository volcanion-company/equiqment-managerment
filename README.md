# Equipment Management System

🚀 **Hệ thống quản lý trang thiết bị** được xây dựng với **.NET 9**, **Clean Architecture**, **DDD**, **CQRS**, **PostgreSQL** và **Redis**.

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D)](https://redis.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📋 Mục lục

- [Tổng quan](#-tổng-quan)
- [Tính năng](#-tính-năng)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Cài đặt](#-cài-đặt)
- [Sử dụng](#-sử-dụng)
- [API Documentation](#-api-documentation)
- [Testing](#-testing)
- [Contributing](#-contributing)

---

## 🎯 Tổng quan

Equipment Management System là một hệ thống quản lý toàn diện cho việc theo dõi, bảo trì và thanh lý trang thiết bị trong tổ chức. Hệ thống được thiết kế theo kiến trúc Clean Architecture, đảm bảo tính mở rộng, bảo trì và kiểm thử dễ dàng.

### Điểm nổi bật

✅ **Clean Architecture** - Tách biệt rõ ràng các tầng nghiệp vụ  
✅ **CQRS Pattern** - Tách biệt Read/Write operations  
✅ **Domain-Driven Design** - Tập trung vào nghiệp vụ cốt lõi  
✅ **Microservices Ready** - Dễ dàng tách thành microservices  
✅ **High Performance** - Cache với Redis, Pagination, Async/Await  
✅ **Production Ready** - Docker, Logging, Exception Handling  

---

## ✨ Tính năng

### 1. 📦 Quản lý Thiết bị (Equipment Management)
- ✏️ CRUD thiết bị đầy đủ
- 📸 Upload và quản lý hình ảnh thiết bị
- 🔲 Tự động generate QR code cho mỗi thiết bị
- 🔍 Tìm kiếm nâng cao (theo loại, trạng thái, từ khóa)
- 📄 Phân trang kết quả
- 🗑️ Soft delete - không mất dữ liệu

### 2. 🏢 Quản lý Kho (Warehouse Management)
- 📥 Nhập kho thiết bị
- 📤 Xuất kho thiết bị
- 📊 Theo dõi tồn kho theo loại thiết bị
- ⚠️ Cảnh báo khi tồn kho thấp hơn ngưỡng
- 📝 Ghi log đầy đủ lịch sử nhập/xuất

### 3. 👥 Cấp phát - Thu hồi (Assignment Management)
- ✅ Cấp phát thiết bị cho user hoặc department
- 🔄 Thu hồi thiết bị
- 📋 Xem lịch sử cấp phát
- 🔍 Tra cứu thiết bị đang được cấp phát cho ai

### 4. ✔️ Kiểm kê (Audit/Inventory Checking)
- 📱 API hỗ trợ mobile app quét QR code
- ✍️ Ghi nhận kết quả kiểm kê (Khớp/Không khớp/Thiếu)
- 🔄 Hỗ trợ đồng bộ offline với LastSyncDate
- 📍 Ghi nhận vị trí kiểm kê

### 5. 🔧 Bảo trì - Sửa chữa (Maintenance Management)
- 📝 Tạo yêu cầu sửa chữa
- 👷 Gán kỹ thuật viên phụ trách
- 📈 Cập nhật tiến độ sửa chữa
- 💰 Ghi nhận chi phí sửa chữa
- 📜 Lịch sử bảo trì đầy đủ

### 6. 🗑️ Thanh lý (Liquidation Management)
- 📄 Tạo yêu cầu thanh lý
- ✔️ Quy trình phê duyệt
- 💵 Ghi nhận giá trị thanh lý
- 📊 Báo cáo thiết bị đã thanh lý

---

## 🛠️ Công nghệ sử dụng

### Backend Framework
- **.NET 9** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API

### Architecture & Patterns
- **Clean Architecture** - 4-layer architecture
- **Domain-Driven Design (DDD)** - Rich domain models
- **CQRS** - Command Query Responsibility Segregation
- **Mediator Pattern** - MediatR library
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management

### Database & Caching
- **PostgreSQL 17** - Primary database
- **Entity Framework Core 9** - ORM, Code-First
- **Redis 7** - Distributed caching (TTL: 30 minutes)

### Libraries & Tools
- **MediatR** - In-process messaging
- **FluentValidation** - Input validation
- **Mapster** - Object-to-object mapping
- **Serilog** - Structured logging
- **QRCoder** - QR code generation
- **Swashbuckle** - Swagger/OpenAPI documentation

### DevOps & Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration
- **xUnit** - Unit testing
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

---

## 📁 Cấu trúc dự án

```
EquipmentManagement/
│
├── src/
│   ├── EquipmentManagement.Domain/              # 🎯 Core Domain Layer
│   │   ├── Common/                               # Base entities
│   │   ├── Entities/                             # Domain entities
│   │   ├── Enums/                                # Domain enumerations
│   │   └── Repositories/                         # Repository interfaces
│   │
│   ├── EquipmentManagement.Application/          # 💼 Application Layer
│   │   ├── Common/                               # Shared application code
│   │   │   ├── Behaviors/                        # MediatR pipeline behaviors
│   │   │   ├── Exceptions/                       # Custom exceptions
│   │   │   ├── Interfaces/                       # Application interfaces
│   │   │   └── Models/                           # DTOs, View Models
│   │   ├── Features/                             # Feature-based organization
│   │   │   ├── Equipments/                       # Equipment feature
│   │   │   │   ├── Commands/                     # Write operations
│   │   │   │   ├── Queries/                      # Read operations
│   │   │   │   └── DTOs/                         # Data transfer objects
│   │   │   ├── Warehouses/                       # Warehouse feature
│   │   │   ├── Assignments/                      # Assignment feature
│   │   │   ├── Audits/                           # Audit feature
│   │   │   ├── Maintenances/                     # Maintenance feature
│   │   │   └── Liquidations/                     # Liquidation feature
│   │   └── DependencyInjection.cs                # DI registration
│   │
│   ├── EquipmentManagement.Infrastructure/       # 🔧 Infrastructure Layer
│   │   ├── Persistence/                          # Database related
│   │   │   ├── Configurations/                   # EF Core configurations
│   │   │   ├── Migrations/                       # Database migrations
│   │   │   └── ApplicationDbContext.cs           # DbContext
│   │   ├── Repositories/                         # Repository implementations
│   │   ├── Services/                             # External services
│   │   │   ├── RedisCacheService.cs              # Redis cache
│   │   │   └── QRCodeService.cs                  # QR generation
│   │   └── DependencyInjection.cs                # DI registration
│   │
│   └── EquipmentManagement.WebAPI/               # 🌐 Presentation Layer
│       ├── Controllers/                          # API Controllers
│       ├── Middleware/                           # Custom middleware
│       │   ├── GlobalExceptionHandlingMiddleware.cs
│       │   └── RequestResponseLoggingMiddleware.cs
│       ├── appsettings.json                      # Configuration
│       └── Program.cs                            # Application entry point
│
├── tests/
│   └── EquipmentManagement.UnitTests/            # 🧪 Unit Tests
│       ├── Application/                          # Application layer tests
│       └── Domain/                               # Domain layer tests
│
├── docs/                                         # 📚 Documentation
│   ├── ARCHITECTURE.md                           # Architecture documentation
│   ├── API_USAGE.md                              # API usage guide
│   └── postman/                                  # Postman collections
│
├── docker-compose.yml                            # Docker orchestration
├── Dockerfile                                    # Docker image definition
├── .dockerignore                                 # Docker ignore file
├── .gitignore                                    # Git ignore file
├── EquipmentManagement.sln                       # Solution file
└── README.md                                     # This file
```

---

## 🚀 Cài đặt

### Yêu cầu hệ thống

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (khuyến nghị)
- [PostgreSQL 17](https://www.postgresql.org/download/) (nếu chạy local)
- [Redis](https://redis.io/download) (nếu chạy local)

### Cách 1: Chạy với Docker (Khuyến nghị) 🐳

```bash
# Clone repository
git clone https://github.com/your-username/equipment-management.git
cd equipment-management

# Chạy tất cả services (API + PostgreSQL + Redis)
docker-compose up -d

# Xem logs
docker-compose logs -f api

# Dừng tất cả services
docker-compose down

# Dừng và xóa volumes
docker-compose down -v
```

**API sẽ chạy tại:** http://localhost:8080  
**Swagger UI:** http://localhost:8080/swagger

### Cách 2: Chạy Local Development

#### Bước 1: Cài đặt dependencies

```bash
# Restore NuGet packages
dotnet restore
```

#### Bước 2: Cấu hình Connection Strings

Tạo file `appsettings.Development.json` trong `src/EquipmentManagement.WebAPI/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EquipmentManagementDb;Username=postgres;Password=your_password",
    "Redis": "localhost:6379"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

#### Bước 3: Tạo Database

```bash
cd src/EquipmentManagement.WebAPI

# Tạo migration (nếu chưa có)
dotnet ef migrations add InitialCreate \
  --project ../EquipmentManagement.Infrastructure \
  --startup-project . \
  --output-dir Persistence/Migrations

# Apply migration
dotnet ef database update \
  --project ../EquipmentManagement.Infrastructure \
  --startup-project .
```

#### Bước 4: Chạy ứng dụng

```bash
# Chạy API
dotnet run --project src/EquipmentManagement.WebAPI

# Hoặc với watch mode (auto-reload)
dotnet watch --project src/EquipmentManagement.WebAPI
```

---

## 📖 Sử dụng

### Truy cập Swagger UI

Mở trình duyệt và truy cập:
- **Development:** http://localhost:5000/swagger (hoặc port được config)
- **Docker:** http://localhost:8080/swagger

### Import Postman Collection

1. Mở Postman
2. Click **Import**
3. Chọn file `docs/postman/Equipment-Management-API.postman_collection.json`
4. Collection sẽ được import với tất cả endpoints

### Ví dụ sử dụng cơ bản

#### 1. Tạo thiết bị mới

```bash
POST /api/equipments
Content-Type: application/json

{
  "code": "LAP001",
  "name": "Dell Latitude 7420",
  "type": "Laptop",
  "description": "Business laptop",
  "specification": "i7-11th, 16GB RAM, 512GB SSD",
  "purchaseDate": "2024-01-15T00:00:00Z",
  "supplier": "Dell Vietnam",
  "price": 25000000,
  "warrantyEndDate": "2027-01-15T00:00:00Z",
  "status": 1
}
```

#### 2. Lấy danh sách thiết bị (có phân trang)

```bash
GET /api/equipments?pageNumber=1&pageSize=10&type=Laptop&status=1
```

#### 3. Cấp phát thiết bị

```bash
POST /api/assignments
Content-Type: application/json

{
  "equipmentId": "guid-of-equipment",
  "assignedToUserId": "user123",
  "assignedDate": "2024-11-25T00:00:00Z",
  "notes": "Cấp phát laptop cho nhân viên mới"
}
```

---

## 📚 API Documentation

Chi tiết API documentation xem tại:
- [API Usage Guide](docs/API_USAGE.md)
- [Swagger UI](http://localhost:8080/swagger) (khi chạy ứng dụng)

### Endpoints chính

| Module | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| **Equipments** | GET | `/api/equipments` | Lấy danh sách thiết bị (phân trang) |
| | GET | `/api/equipments/{id}` | Lấy thiết bị theo ID |
| | POST | `/api/equipments` | Tạo thiết bị mới |
| | PUT | `/api/equipments/{id}` | Cập nhật thiết bị |
| | DELETE | `/api/equipments/{id}` | Xóa thiết bị (soft delete) |
| **Warehouses** | POST | `/api/warehouses/transactions` | Nhập/xuất kho |
| **Assignments** | POST | `/api/assignments` | Cấp phát thiết bị |
| **Audits** | POST | `/api/audits` | Tạo phiếu kiểm kê |
| **Maintenances** | POST | `/api/maintenances` | Tạo yêu cầu bảo trì |
| **Liquidations** | POST | `/api/liquidations` | Tạo yêu cầu thanh lý |

---

## 🧪 Testing

### Chạy Unit Tests

```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Chạy tests cho một project cụ thể
dotnet test tests/EquipmentManagement.UnitTests/EquipmentManagement.UnitTests.csproj

# Chạy tests với filter
dotnet test --filter "FullyQualifiedName~Equipment"
```

### Test Coverage

Dự án bao gồm:
- ✅ Unit tests cho Application layer (Commands, Queries)
- ✅ Unit tests cho Domain entities
- ✅ Validator tests với FluentValidation

---

## 🔧 Configuration

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Production | Môi trường (Development/Production) |
| `ASPNETCORE_URLS` | http://+:8080 | URLs bind |
| `ConnectionStrings__DefaultConnection` | - | PostgreSQL connection string |
| `ConnectionStrings__Redis` | - | Redis connection string |

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=EquipmentManagementDb;Username=postgres;Password=postgres123",
    "Redis": "redis:6379"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

---

## 📊 Database Schema

### Main Tables

- **Equipments** - Thông tin thiết bị
- **WarehouseItems** - Quản lý kho theo loại thiết bị
- **WarehouseTransactions** - Lịch sử nhập/xuất kho
- **Assignments** - Cấp phát thiết bị
- **AuditRecords** - Phiếu kiểm kê
- **MaintenanceRequests** - Yêu cầu bảo trì
- **LiquidationRequests** - Yêu cầu thanh lý

### Indexes

- Equipment.Code (Unique)
- Equipment.Type
- Equipment.Status
- Equipment.PurchaseDate
- All tables: IsDeleted (Query filter)

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Authors

- **Your Name** - *Initial work*

---

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Microsoft .NET Team

---

## 📞 Support

Nếu bạn gặp vấn đề hoặc có câu hỏi, vui lòng:
- Tạo [Issue](https://github.com/your-username/equipment-management/issues)
- Liên hệ: your.email@example.com

---

**Made with ❤️ using .NET 9**
