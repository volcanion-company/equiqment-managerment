# Architecture Documentation

## 📐 Tổng quan Kiến trúc

Equipment Management System được xây dựng dựa trên **Clean Architecture** (hay còn gọi là Onion Architecture, Hexagonal Architecture), kết hợp với **Domain-Driven Design (DDD)** và **CQRS Pattern**.

---

## 🏛️ Clean Architecture

### Nguyên tắc cốt lõi

1. **Independence of Frameworks** - Không phụ thuộc vào frameworks cụ thể
2. **Testability** - Business logic có thể test độc lập
3. **Independence of UI** - UI có thể thay đổi dễ dàng
4. **Independence of Database** - Database có thể thay đổi
5. **Independence of External Services** - Business logic không phụ thuộc vào external services

### Dependency Rule

```
┌─────────────────────────────────────────────┐
│          WebAPI (Presentation)              │
│  ┌───────────────────────────────────────┐  │
│  │     Infrastructure Layer              │  │
│  │  ┌─────────────────────────────────┐  │  │
│  │  │   Application Layer             │  │  │
│  │  │  ┌───────────────────────────┐  │  │  │
│  │  │  │   Domain Layer            │  │  │  │
│  │  │  │  (Core Business Logic)    │  │  │  │
│  │  │  └───────────────────────────┘  │  │  │
│  │  └─────────────────────────────────┘  │  │
│  └───────────────────────────────────────┘  │
└─────────────────────────────────────────────┘

Dependencies flow inward only (→)
```

---

## 📦 Layer Details

### 1. Domain Layer (Core) 🎯

**Mục đích:** Chứa core business logic và business rules

**Không phụ thuộc vào:** Bất kỳ layer nào khác

**Bao gồm:**

```
Domain/
├── Common/
│   └── BaseEntity.cs                    # Base class cho entities
├── Entities/
│   ├── Equipment.cs                     # Aggregate root
│   ├── WarehouseItem.cs
│   ├── WarehouseTransaction.cs
│   ├── Assignment.cs
│   ├── AuditRecord.cs
│   ├── MaintenanceRequest.cs
│   └── LiquidationRequest.cs
├── Enums/
│   ├── EquipmentStatus.cs
│   ├── AssignmentStatus.cs
│   ├── AuditResult.cs
│   ├── MaintenanceStatus.cs
│   └── WarehouseTransactionType.cs
└── Repositories/
    ├── IRepository.cs                   # Generic repository
    ├── IEquipmentRepository.cs
    ├── IWarehouseItemRepository.cs
    ├── IWarehouseTransactionRepository.cs
    ├── IAssignmentRepository.cs
    ├── IAuditRecordRepository.cs
    ├── IMaintenanceRequestRepository.cs
    ├── ILiquidationRequestRepository.cs
    └── IUnitOfWork.cs
```

**Đặc điểm:**
- ✅ Rich domain models với business logic
- ✅ Entities có behavior, không phải anemic models
- ✅ Repository interfaces (implementation ở Infrastructure)
- ✅ Domain events (nếu cần)
- ❌ Không có dependencies ngoài .NET core libraries

---

### 2. Application Layer 💼

**Mục đích:** Orchestrate domain objects để thực hiện use cases

**Phụ thuộc vào:** Domain Layer only

**Bao gồm:**

```
Application/
├── Common/
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs        # MediatR pipeline
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   └── ValidationException.cs
│   ├── Interfaces/
│   │   ├── ICacheService.cs
│   │   └── IQRCodeService.cs
│   └── Models/
│       └── PagedResult.cs
├── Features/
│   ├── Equipments/
│   │   ├── Commands/
│   │   │   ├── CreateEquipment/
│   │   │   │   ├── CreateEquipmentCommand.cs
│   │   │   │   ├── CreateEquipmentCommandHandler.cs
│   │   │   │   └── CreateEquipmentCommandValidator.cs
│   │   │   ├── UpdateEquipment/
│   │   │   └── DeleteEquipment/
│   │   ├── Queries/
│   │   │   ├── GetEquipments/
│   │   │   │   ├── GetEquipmentsQuery.cs
│   │   │   │   └── GetEquipmentsQueryHandler.cs
│   │   │   └── GetEquipmentById/
│   │   └── DTOs/
│   │       └── EquipmentDto.cs
│   ├── Warehouses/
│   ├── Assignments/
│   ├── Audits/
│   ├── Maintenances/
│   └── Liquidations/
└── DependencyInjection.cs
```

**Patterns được sử dụng:**

#### CQRS (Command Query Responsibility Segregation)

```csharp
// Command - Write operation
public class CreateEquipmentCommand : IRequest<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    // ... other properties
}

// Query - Read operation
public class GetEquipmentsQuery : IRequest<PagedResult<EquipmentDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Type { get; set; }
}
```

#### Mediator Pattern

```csharp
// Controller không gọi trực tiếp business logic
public class EquipmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateEquipmentCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}
```

#### Validation Pipeline

```csharp
// FluentValidation tự động chạy trước khi handler execute
public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}
```

---

### 3. Infrastructure Layer 🔧

**Mục đích:** Implement các interfaces từ Application layer, xử lý concerns kỹ thuật

**Phụ thuộc vào:** Application Layer, Domain Layer

**Bao gồm:**

```
Infrastructure/
├── Persistence/
│   ├── Configurations/
│   │   ├── EquipmentConfiguration.cs   # EF Core fluent API
│   │   ├── WarehouseItemConfiguration.cs
│   │   └── ... (other configurations)
│   ├── Migrations/
│   │   └── [timestamp]_InitialCreate.cs
│   └── ApplicationDbContext.cs
├── Repositories/
│   ├── Repository.cs                    # Generic implementation
│   ├── EquipmentRepository.cs
│   ├── WarehouseItemRepository.cs
│   └── UnitOfWork.cs
├── Services/
│   ├── RedisCacheService.cs            # ICacheService implementation
│   └── QRCodeService.cs                # IQRCodeService implementation
└── DependencyInjection.cs
```

**Entity Framework Core Configuration:**

```csharp
public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipments");
        builder.HasKey(e => e.Id);
        
        // Indexes
        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.IsDeleted);
        
        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
        
        // Relationships
        builder.HasMany(e => e.Assignments)
               .WithOne(a => a.Equipment)
               .HasForeignKey(a => a.EquipmentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

**Repository Pattern:**

```csharp
public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
{
    public async Task<Equipment?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Code == code);
    }
    
    public async Task<(IEnumerable<Equipment>, int)> GetPagedAsync(
        int pageNumber, int pageSize, string? type, string? status)
    {
        var query = _dbSet.AsQueryable();
        
        if (!string.IsNullOrEmpty(type))
            query = query.Where(e => e.Type == type);
            
        var total = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (items, total);
    }
}
```

**Unit of Work Pattern:**

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IEquipmentRepository Equipments { get; }
    public IWarehouseItemRepository WarehouseItems { get; }
    // ... other repositories
    
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync() { }
    public async Task CommitTransactionAsync() { }
    public async Task RollbackTransactionAsync() { }
}
```

---

### 4. WebAPI Layer (Presentation) 🌐

**Mục đích:** HTTP endpoints, middleware, configuration

**Phụ thuộc vào:** Application Layer, Infrastructure Layer

**Bao gồm:**

```
WebAPI/
├── Controllers/
│   ├── EquipmentsController.cs
│   ├── WarehousesController.cs
│   ├── AssignmentsController.cs
│   ├── AuditsController.cs
│   ├── MaintenancesController.cs
│   └── LiquidationsController.cs
├── Middleware/
│   ├── GlobalExceptionHandlingMiddleware.cs
│   └── RequestResponseLoggingMiddleware.cs
├── appsettings.json
├── appsettings.Production.json
└── Program.cs
```

**Controller Example:**

```csharp
[ApiController]
[Route("api/[controller]")]
[Tags("Equipments")]
public class EquipmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EquipmentDto>), 200)]
    public async Task<ActionResult<PagedResult<EquipmentDto>>> GetEquipments(
        [FromQuery] GetEquipmentsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

**Global Exception Handling:**

```csharp
public class GlobalExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await WriteErrorResponse(context, ex.Message);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await WriteErrorResponse(context, ex.Errors);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, "Internal server error");
        }
    }
}
```

---

## 🔄 Request Flow

### Command Flow (Write Operation)

```
1. HTTP POST Request
   ↓
2. Controller receives CreateEquipmentCommand
   ↓
3. Controller.Send(command) → MediatR
   ↓
4. ValidationBehavior validates command
   ↓
5. CreateEquipmentCommandHandler executes
   ↓
6. Handler calls Domain Repository (via UnitOfWork)
   ↓
7. Infrastructure Repository saves to Database
   ↓
8. Handler invalidates cache
   ↓
9. Returns result to Controller
   ↓
10. Controller returns HTTP 201 Created
```

### Query Flow (Read Operation)

```
1. HTTP GET Request
   ↓
2. Controller receives GetEquipmentsQuery
   ↓
3. Controller.Send(query) → MediatR
   ↓
4. GetEquipmentsQueryHandler executes
   ↓
5. Handler checks Redis cache
   ↓
6. If cache miss:
   ├─→ Query Database via Repository
   ├─→ Map to DTOs using Mapster
   └─→ Store in Redis cache (TTL: 30 min)
   ↓
7. Returns PagedResult<EquipmentDto>
   ↓
8. Controller returns HTTP 200 OK
```

---

## 🎨 Design Patterns

### 1. Repository Pattern

**Mục đích:** Abstract data access layer

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
```

### 2. Unit of Work Pattern

**Mục đích:** Manage transactions và coordinate repositories

```csharp
public interface IUnitOfWork : IDisposable
{
    IEquipmentRepository Equipments { get; }
    IWarehouseItemRepository WarehouseItems { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
}
```

### 3. CQRS Pattern

**Mục đích:** Separate read and write operations

- **Commands:** Modify state, return void or ID
- **Queries:** Read-only, return DTOs

### 4. Mediator Pattern

**Mục đích:** Reduce coupling between components

```csharp
// Request
public class GetEquipmentByIdQuery : IRequest<EquipmentDto?>
{
    public Guid Id { get; set; }
}

// Handler
public class GetEquipmentByIdQueryHandler 
    : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
{
    public async Task<EquipmentDto?> Handle(GetEquipmentByIdQuery request)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.Id);
        return equipment?.Adapt<EquipmentDto>();
    }
}
```

### 5. Strategy Pattern

**Mục đích:** Different caching strategies

```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration);
    Task RemoveAsync(string key);
}

// Redis implementation
public class RedisCacheService : ICacheService { }

// In-Memory implementation (for testing)
public class InMemoryCacheService : ICacheService { }
```

---

## 🔐 Cross-Cutting Concerns

### 1. Logging (Serilog)

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

**Structured Logging:**
```csharp
_logger.LogInformation(
    "Equipment {EquipmentCode} created by {UserId}", 
    equipment.Code, 
    userId);
```

### 2. Exception Handling

- **Global Exception Middleware:** Catch all unhandled exceptions
- **Custom Exceptions:** NotFoundException, ValidationException
- **Consistent Error Response:**

```json
{
  "statusCode": 404,
  "message": "Equipment (guid) was not found",
  "details": null
}
```

### 3. Validation

**FluentValidation Pipeline:**

```csharp
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, ...)
    {
        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();
            
        if (failures.Any())
            throw new ValidationException(failures);
            
        return await next();
    }
}
```

### 4. Caching Strategy

**Multi-level caching:**

1. **L1 Cache:** Memory cache (nếu cần)
2. **L2 Cache:** Redis (distributed)

**Cache Invalidation:**
- Write operations invalidate related cache
- TTL: 30 minutes default
- Prefix-based invalidation: `equipments_*`

---

## 🗄️ Database Design

### Soft Delete Pattern

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }  // Soft delete
}

// EF Core Query Filter
modelBuilder.Entity<Equipment>()
    .HasQueryFilter(e => !e.IsDeleted);
```

### Audit Trail

Mỗi entity track:
- `CreatedAt` - Thời điểm tạo
- `UpdatedAt` - Thời điểm cập nhật cuối
- `IsDeleted` - Soft delete flag

---

## 📊 Performance Considerations

### 1. Async/Await Everywhere

```csharp
public async Task<Equipment?> GetByIdAsync(Guid id)
{
    return await _dbSet.FindAsync(id);
}
```

### 2. Pagination

```csharp
var items = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 3. Caching

```csharp
var cacheKey = $"equipments_{pageNumber}_{pageSize}_{type}";
var cached = await _cache.GetAsync<PagedResult<EquipmentDto>>(cacheKey);
if (cached != null) return cached;
```

### 4. Database Indexes

```csharp
builder.HasIndex(e => e.Code).IsUnique();
builder.HasIndex(e => e.Type);
builder.HasIndex(e => e.Status);
builder.HasIndex(e => e.IsDeleted);
```

### 5. Select Only Needed Columns

```csharp
var dto = await _context.Equipments
    .Where(e => e.Id == id)
    .Select(e => new EquipmentDto 
    { 
        Id = e.Id,
        Name = e.Name,
        // ... only needed fields
    })
    .FirstOrDefaultAsync();
```

---

## 🧪 Testing Strategy

### Unit Tests

```csharp
[Fact]
public async Task Handle_ShouldCreateEquipment_WhenValidCommand()
{
    // Arrange
    var command = new CreateEquipmentCommand { ... };
    var handler = new CreateEquipmentCommandHandler(...);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.Should().NotBeEmpty();
}
```

### Integration Tests (Future)

- Test full request pipeline
- Use TestServer
- In-memory database

---

## 🚀 Deployment Architecture

```
┌─────────────────────────────────────────┐
│         Load Balancer / API Gateway     │
└────────────────┬────────────────────────┘
                 │
         ┌───────┴────────┐
         │                │
    ┌────▼─────┐    ┌────▼─────┐
    │  API 1   │    │  API 2   │
    │ (Docker) │    │ (Docker) │
    └────┬─────┘    └────┬─────┘
         │                │
         └───────┬────────┘
                 │
        ┌────────┴─────────┐
        │                  │
   ┌────▼─────┐      ┌────▼─────┐
   │PostgreSQL│      │  Redis   │
   │ (Primary)│      │  Cache   │
   └──────────┘      └──────────┘
```

---

## 📚 References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design by Eric Evans](https://www.domainlanguage.com/ddd/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Microsoft .NET Architecture Guides](https://dotnet.microsoft.com/learn/dotnet/architecture-guides)
