# 🏗️ Architecture Documentation

## Equipment Management System - Technical Architecture

> **Version:** 1.0.0  
> **Last Updated:** December 12, 2025  
> **Framework:** .NET 9.0

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architectural Patterns](#architectural-patterns)
- [Project Structure](#project-structure)
- [Layer Responsibilities](#layer-responsibilities)
- [Data Flow](#data-flow)
- [Technology Stack](#technology-stack)
- [Design Decisions](#design-decisions)
- [Scalability Considerations](#scalability-considerations)

---

## 🎯 Overview

The Equipment Management System is built using **Clean Architecture** principles combined with **Domain-Driven Design (DDD)** and **CQRS** pattern. This architecture ensures:

- ✅ **Separation of Concerns** - Each layer has a clear responsibility
- ✅ **Testability** - Business logic isolated from infrastructure
- ✅ **Maintainability** - Changes in one layer don't affect others
- ✅ **Scalability** - Easy to scale horizontally and vertically
- ✅ **Technology Independence** - Core business logic agnostic to frameworks

### Core Principles

1. **Dependency Rule**: Dependencies point inward (Infrastructure → Application → Domain)
2. **Single Responsibility**: Each component has one reason to change
3. **Open/Closed**: Open for extension, closed for modification
4. **Interface Segregation**: Small, focused interfaces
5. **Dependency Inversion**: Depend on abstractions, not concretions

---

## 🏛️ Architectural Patterns

### 1. Clean Architecture

```
┌─────────────────────────────────────────────────┐
│                   WebAPI Layer                  │
│              (Controllers, Middleware)          │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│            Infrastructure Layer                 │
│     (Database, External Services, Cache)        │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│             Application Layer                   │
│       (CQRS, Handlers, Validators, DTOs)        │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│               Domain Layer                      │
│        (Entities, Enums, Interfaces)            │
│              (Business Rules)                   │
└─────────────────────────────────────────────────┘
```

### 2. CQRS (Command Query Responsibility Segregation)

**Commands** (Write Operations):
- Create, Update, Delete operations
- Validated using FluentValidation
- Return Unit or Created ID

**Queries** (Read Operations):
- Get, List, Search operations
- Return DTOs (never entities)
- Optimized for read performance

```csharp
// Command Example
public class CreateEquipmentCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Query Example
public class GetEquipmentByIdQuery : IRequest<EquipmentDto>
{
    public Guid EquipmentId { get; set; }
}
```

### 3. Repository Pattern

Abstraction over data access layer:

```csharp
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
}
```

### 4. Unit of Work Pattern

Manages database transactions:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

---

## 📁 Project Structure

```
volcanion-device-management/
│
├── src/
│   ├── libs/                                    # Core business logic
│   │   ├── EquipmentManagement.Domain/         # Enterprise business rules
│   │   │   ├── Common/
│   │   │   │   └── BaseEntity.cs              # Base entity with audit fields
│   │   │   ├── Entities/                      # Domain entities
│   │   │   │   ├── Equipment.cs
│   │   │   │   ├── Assignment.cs
│   │   │   │   ├── MaintenanceRequest.cs
│   │   │   │   ├── LiquidationRequest.cs
│   │   │   │   ├── AuditRecord.cs
│   │   │   │   ├── WarehouseItem.cs
│   │   │   │   └── WarehouseTransaction.cs
│   │   │   ├── Enums/                         # Domain enumerations
│   │   │   │   ├── EquipmentStatus.cs
│   │   │   │   ├── AssignmentStatus.cs
│   │   │   │   ├── MaintenanceStatus.cs
│   │   │   │   ├── AuditResult.cs
│   │   │   │   └── WarehouseTransactionType.cs
│   │   │   └── Repositories/                  # Repository interfaces
│   │   │       ├── IRepository.cs
│   │   │       ├── IUnitOfWork.cs
│   │   │       ├── IEquipmentRepository.cs
│   │   │       ├── IAssignmentRepository.cs
│   │   │       ├── IMaintenanceRequestRepository.cs
│   │   │       ├── ILiquidationRequestRepository.cs
│   │   │       ├── IAuditRecordRepository.cs
│   │   │       ├── IWarehouseItemRepository.cs
│   │   │       └── IWarehouseTransactionRepository.cs
│   │   │
│   │   ├── EquipmentManagement.Application/    # Application business rules
│   │   │   ├── Common/
│   │   │   │   ├── Behaviors/                 # MediatR pipeline behaviors
│   │   │   │   │   └── ValidationBehavior.cs  # FluentValidation pipeline
│   │   │   │   ├── Exceptions/                # Application exceptions
│   │   │   │   ├── Interfaces/                # Service interfaces
│   │   │   │   │   ├── IQRCodeService.cs
│   │   │   │   │   └── ICacheService.cs
│   │   │   │   └── Models/                    # Shared models
│   │   │   ├── Features/                      # Feature-based organization
│   │   │   │   ├── Equipments/
│   │   │   │   │   ├── Commands/
│   │   │   │   │   │   ├── CreateEquipment/
│   │   │   │   │   │   ├── UpdateEquipment/
│   │   │   │   │   │   └── DeleteEquipment/
│   │   │   │   │   ├── Queries/
│   │   │   │   │   │   ├── GetEquipments/
│   │   │   │   │   │   └── GetEquipmentById/
│   │   │   │   │   └── DTOs/
│   │   │   │   ├── Assignments/
│   │   │   │   ├── Maintenances/
│   │   │   │   ├── Liquidations/
│   │   │   │   ├── Audits/
│   │   │   │   └── Warehouses/
│   │   │   └── DependencyInjection.cs         # Service registration
│   │   │
│   │   └── EquipmentManagement.Infrastructure/ # External concerns
│   │       ├── Persistence/
│   │       │   ├── ApplicationDbContext.cs    # EF Core DbContext
│   │       │   └── Configurations/            # Entity configurations
│   │       ├── Repositories/                  # Repository implementations
│   │       ├── Services/                      # Service implementations
│   │       │   ├── QRCodeService.cs
│   │       │   └── RedisCacheService.cs
│   │       └── DependencyInjection.cs         # Infrastructure registration
│   │
│   └── presentations/                          # Presentation layer
│       └── EquipmentManagement.WebAPI/        # REST API
│           ├── Controllers/                   # API endpoints
│           ├── Middleware/                    # Custom middleware
│           ├── Properties/
│           ├── appsettings.json
│           └── Program.cs                     # Application entry point
│
├── tests/
│   └── EquipmentManagement.UnitTests/         # Unit tests
│       ├── Application/                       # Application layer tests
│       └── Domain/                            # Domain layer tests
│
├── docs/                                       # Documentation
│   ├── ARCHITECTURE.md
│   ├── GUIDELINES.md
│   ├── API_REFERENCE.md
│   └── GETTING_STARTED.md
│
├── docker-compose.yml                         # Docker orchestration
├── Dockerfile                                 # Container definition
├── README.md
├── LICENSE
└── CONTRIBUTING.md
```

---

## 🎯 Layer Responsibilities

### 1. Domain Layer (`EquipmentManagement.Domain`)

**Purpose**: Core business logic and rules

**Responsibilities**:
- Define domain entities with business rules
- Define domain enumerations
- Define repository interfaces (not implementations)
- Contain zero dependencies on other layers

**Key Components**:
- `BaseEntity`: Audit fields (Id, CreatedAt, UpdatedAt, IsDeleted)
- Entities: Equipment, Assignment, MaintenanceRequest, etc.
- Enums: EquipmentStatus, AssignmentStatus, etc.
- Repository Interfaces: IRepository<T>, IEquipmentRepository, etc.

**Example Entity**:
```csharp
public class Equipment : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; }
    public decimal Price { get; set; }
    
    // Navigation properties
    public virtual ICollection<Assignment> Assignments { get; set; }
    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
}
```

### 2. Application Layer (`EquipmentManagement.Application`)

**Purpose**: Application-specific business rules and use cases

**Responsibilities**:
- Implement CQRS commands and queries
- Handle business workflows
- Validate input using FluentValidation
- Map entities to DTOs using Mapster
- Define service interfaces

**Key Components**:
- **Commands**: Create/Update/Delete operations
- **Queries**: Read operations returning DTOs
- **Handlers**: MediatR request handlers
- **Validators**: FluentValidation validators
- **DTOs**: Data Transfer Objects for API responses
- **Behaviors**: Cross-cutting concerns (validation, logging)

**Example Command Handler**:
```csharp
public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Guid>
{
    private readonly IEquipmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQRCodeService _qrCodeService;

    public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        // Business logic
        var equipment = new Equipment
        {
            Name = request.Name,
            Code = GenerateCode(),
            Status = EquipmentStatus.New
        };
        
        await _repository.AddAsync(equipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return equipment.Id;
    }
}
```

**Validation Pipeline**:
```csharp
public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Equipment name is required")
            .MaximumLength(200);
            
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}
```

### 3. Infrastructure Layer (`EquipmentManagement.Infrastructure`)

**Purpose**: External dependencies and implementations

**Responsibilities**:
- Implement repository interfaces
- Database access with Entity Framework Core
- External service integrations
- Caching with Redis
- File storage
- Third-party APIs

**Key Components**:
- **ApplicationDbContext**: EF Core database context
- **Configurations**: Fluent API entity configurations
- **Repositories**: Concrete repository implementations
- **Services**: QRCodeService, CacheService implementations

**Example Repository**:
```csharp
public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
{
    public EquipmentRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<Equipment?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Code == code && !e.IsDeleted);
    }
}
```

**Entity Configuration**:
```csharp
public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasIndex(e => e.Code).IsUnique();
        
        builder.HasQueryFilter(e => !e.IsDeleted); // Global soft delete filter
    }
}
```

### 4. Presentation Layer (`EquipmentManagement.WebAPI`)

**Purpose**: User interface and API endpoints

**Responsibilities**:
- Expose REST API endpoints
- Handle HTTP requests/responses
- Authentication and authorization
- API documentation (Swagger)
- Error handling middleware
- Logging and monitoring

**Key Components**:
- **Controllers**: API endpoints organized by feature
- **Middleware**: Exception handling, logging, authentication
- **Program.cs**: Dependency injection configuration
- **appsettings.json**: Configuration management

**Example Controller**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> CreateEquipment(
        [FromBody] CreateEquipmentCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetEquipmentById), new { id }, id);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EquipmentDto>> GetEquipmentById(Guid id)
    {
        var query = new GetEquipmentByIdQuery { EquipmentId = id };
        var equipment = await _mediator.Send(query);
        return Ok(equipment);
    }
}
```

---

## 🔄 Data Flow

### Command Flow (Write Operations)

```
┌─────────────┐       ┌──────────────┐      ┌─────────────────┐
│   Client    │─────▶│  Controller  │─────▶│  MediatR        │
│   (HTTP)    │       │  (WebAPI)    │      │  Pipeline       │
└─────────────┘       └──────────────┘      └────────┬────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Validation Behavior           │
                                     │  (FluentValidation)            │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Command Handler               │
                                     │  (Application Layer)           │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Domain Entity                 │
                                     │  (Business Rules)              │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Repository                    │
                                     │  (Infrastructure)              │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Database (PostgreSQL)         │
                                     └────────────────────────────────┘
```

### Query Flow (Read Operations)

```
┌─────────────┐       ┌──────────────┐      ┌─────────────────┐
│   Client    │─────▶│  Controller  │─────▶│  MediatR        │
│   (HTTP)    │       │  (WebAPI)    │      │  Pipeline       │
└─────────────┘       └──────────────┘      └────────┬────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Query Handler                 │
                                     │  (Application Layer)           │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Repository                    │
                                     │  (Read from DB/Cache)          │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Entity → DTO                  │
                                     │  (Mapster Mapping)             │
                                     └───────────────┬────────────────┘
                                                     │
                                     ┌───────────────▼────────────────┐
                                     │  Return DTO to Client          │
                                     └────────────────────────────────┘
```

---

## 🛠️ Technology Stack

### Backend

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 9.0 | Runtime framework |
| ASP.NET Core | 9.0 | Web API framework |
| C# | 12.0 | Programming language |

### Libraries & Frameworks

| Library | Version | Purpose |
|---------|---------|---------|
| MediatR | 12.2.0 | CQRS implementation |
| FluentValidation | 11.9.0 | Input validation |
| Mapster | 7.4.0 | Object mapping |
| Entity Framework Core | 9.0.0 | ORM for data access |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.0 | PostgreSQL provider |
| QRCoder | 1.6.0 | QR code generation |

### Infrastructure

| Technology | Purpose |
|------------|---------|
| PostgreSQL 17 | Primary database |
| Redis | Caching layer |
| Docker | Containerization |
| Swagger/OpenAPI | API documentation |
| Serilog | Structured logging |

### Development & Testing

| Tool | Purpose |
|------|---------|
| xUnit | Unit testing framework |
| Moq | Mocking framework |
| FluentAssertions | Assertion library |
| Coverlet | Code coverage |

---

## 🎯 Design Decisions

### 1. Why CQRS?

**Benefits**:
- ✅ Separate read and write models
- ✅ Optimize queries independently
- ✅ Better scalability
- ✅ Clear separation of concerns

**Trade-offs**:
- ❌ More code (commands + queries)
- ❌ Learning curve
- ✅ Worth it for complex domains

### 2. Why Repository Pattern?

**Benefits**:
- ✅ Abstract data access
- ✅ Easy to test (mock repositories)
- ✅ Centralize data access logic
- ✅ Switch databases easily

**Implementation**:
```csharp
// Generic repository for common operations
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task AddAsync(TEntity entity);
}

// Specific repository for custom queries
public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<Equipment?> GetByCodeAsync(string code);
}
```

### 3. Why Soft Delete?

**Benefits**:
- ✅ Data recovery capability
- ✅ Audit trail maintenance
- ✅ Referential integrity

**Implementation**:
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

// Global query filter
modelBuilder.Entity<Equipment>()
    .HasQueryFilter(e => !e.IsDeleted);
```

### 4. Why Feature-Based Structure?

**Benefits**:
- ✅ Related code together
- ✅ Easy to find files
- ✅ Better team collaboration
- ✅ Modular and maintainable

**Example**:
```
Features/
  └── Equipments/
      ├── Commands/
      │   └── CreateEquipment/
      │       ├── CreateEquipmentCommand.cs
      │       ├── CreateEquipmentCommandHandler.cs
      │       └── CreateEquipmentCommandValidator.cs
      ├── Queries/
      └── DTOs/
```

### 5. Why DTOs Instead of Entities?

**Benefits**:
- ✅ Control API contract
- ✅ Hide internal structure
- ✅ Prevent over-posting
- ✅ Optimize for presentation

**Example**:
```csharp
// Entity (internal)
public class Equipment : BaseEntity
{
    public string Code { get; set; }
    public virtual ICollection<Assignment> Assignments { get; set; }
}

// DTO (external)
public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    // No navigation properties, no audit fields
}
```

---

## 📈 Scalability Considerations

### Horizontal Scaling

**Stateless API Design**:
- No session state stored in memory
- All state in database/cache
- Can run multiple API instances behind load balancer

**Caching Strategy**:
- Redis for distributed caching
- Cache invalidation on updates
- Cache-aside pattern

### Vertical Scaling

**Database Optimization**:
- Proper indexing on frequently queried columns
- Query optimization with EF Core
- Read replicas for heavy read workloads

**Performance**:
- Async/await throughout
- Pagination for large datasets
- Background jobs for heavy processing

### Future Enhancements

1. **Event Sourcing**: Track all state changes
2. **Message Queue**: Async processing (RabbitMQ/Kafka)
3. **Microservices**: Split by bounded contexts
4. **API Gateway**: Centralized entry point
5. **GraphQL**: Flexible query language

---

## 🔐 Security Considerations

### Authentication & Authorization

```csharp
// JWT Authentication (planned)
[Authorize(Roles = "Manager")]
[HttpPut("{id}/approve")]
public async Task<IActionResult> ApproveLiquidation(Guid id)
{
    // Only managers can approve
}
```

### Data Protection

- SQL Injection prevention (parameterized queries via EF Core)
- CORS configuration
- HTTPS enforcement
- Input validation (FluentValidation)
- Output encoding (automatic with JSON serialization)

### Audit Trail

```csharp
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    // Track who created/modified (future)
    // public string? CreatedBy { get; set; }
    // public string? UpdatedBy { get; set; }
}
```

---

## 📚 References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)

---

**Document Version**: 1.0.0  
**Last Review**: December 12, 2025  
**Next Review**: Quarterly or on major architectural changes
