# 📘 Development Guidelines

## Equipment Management System - Coding Standards & Best Practices

> **Version:** 1.0.0  
> **Last Updated:** December 12, 2025  
> **Target:** .NET 9.0 Development Team

---

## 📋 Table of Contents

- [Code Style](#code-style)
- [Naming Conventions](#naming-conventions)
- [Project Organization](#project-organization)
- [CQRS Implementation](#cqrs-implementation)
- [Validation Rules](#validation-rules)
- [Error Handling](#error-handling)
- [Testing Guidelines](#testing-guidelines)
- [Database Guidelines](#database-guidelines)
- [API Design](#api-design)
- [Git Workflow](#git-workflow)

---

## 🎨 Code Style

### General C# Conventions

Follow official [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions):

**File Organization**:
```csharp
// 1. Using statements (sorted alphabetically)
using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Domain.Entities;
using MediatR;

// 2. Namespace
namespace EquipmentManagement.Application.Features.Equipments.Commands;

// 3. Class definition
public class CreateEquipmentCommand : IRequest<Guid>
{
    // Properties, methods, etc.
}
```

**Formatting**:
- ✅ Use 4 spaces for indentation (no tabs)
- ✅ Opening braces `{` on new line
- ✅ One statement per line
- ✅ Always use braces for `if`, `for`, `while`
- ✅ Maximum line length: 120 characters

**Modern C# Features**:
```csharp
// ✅ DO: Use file-scoped namespaces (.NET 6+)
namespace EquipmentManagement.Application.Features.Equipments;

// ✅ DO: Use primary constructors (.NET 8+)
public class EquipmentsController(IMediator mediator, ILogger<EquipmentsController> logger) 
    : ControllerBase
{
    // mediator and logger are automatically available
}

// ✅ DO: Use collection expressions (.NET 8+)
var items = [];
var numbers = [1, 2, 3, 4, 5];

// ✅ DO: Use target-typed new
Equipment equipment = new() { Name = "Laptop" };

// ✅ DO: Use string interpolation
var message = $"Equipment {equipment.Name} created successfully";

// ✅ DO: Use pattern matching
if (equipment is { Status: EquipmentStatus.New, Price: > 1000 })
{
    // High-value new equipment
}
```

---

## 🏷️ Naming Conventions

### General Rules

| Type | Convention | Example |
|------|------------|---------|
| Namespace | PascalCase | `EquipmentManagement.Application.Features` |
| Class | PascalCase | `CreateEquipmentCommand` |
| Interface | IPascalCase | `IEquipmentRepository` |
| Method | PascalCase | `GetEquipmentByIdAsync` |
| Property | PascalCase | `EquipmentName` |
| Field (private) | _camelCase | `_equipmentRepository` |
| Parameter | camelCase | `equipmentId` |
| Local variable | camelCase | `equipment` |
| Constant | PascalCase | `MaxPageSize` |

### Specific Naming Patterns

**Commands** (Write Operations):
```csharp
// Pattern: {Verb}{Entity}Command
CreateEquipmentCommand
UpdateEquipmentCommand
DeleteEquipmentCommand
AssignTechnicianCommand
ReturnAssignmentCommand
```

**Queries** (Read Operations):
```csharp
// Pattern: Get{Entity(s)}{Criteria}Query
GetEquipmentsQuery
GetEquipmentByIdQuery
GetEquipmentsByStatusQuery
GetPendingMaintenancesQuery
```

**Handlers**:
```csharp
// Pattern: {CommandOrQuery}Handler
CreateEquipmentCommandHandler
GetEquipmentByIdQueryHandler
```

**Validators**:
```csharp
// Pattern: {CommandOrQuery}Validator
CreateEquipmentCommandValidator
GetEquipmentByIdQueryValidator
```

**DTOs**:
```csharp
// Pattern: {Entity}Dto
EquipmentDto
AssignmentDto
MaintenanceRequestDto
```

**Repositories**:
```csharp
// Pattern: I{Entity}Repository (interface)
IEquipmentRepository
IAssignmentRepository

// Pattern: {Entity}Repository (implementation)
EquipmentRepository
AssignmentRepository
```

---

## 📁 Project Organization

### Feature-Based Structure

Organize code by **feature**, not by technical layer:

```
Features/
  └── Equipments/
      ├── Commands/
      │   ├── CreateEquipment/
      │   │   ├── CreateEquipmentCommand.cs
      │   │   ├── CreateEquipmentCommandHandler.cs
      │   │   └── CreateEquipmentCommandValidator.cs
      │   ├── UpdateEquipment/
      │   └── DeleteEquipment/
      ├── Queries/
      │   ├── GetEquipments/
      │   └── GetEquipmentById/
      └── DTOs/
          └── EquipmentDto.cs
```

**Benefits**:
- ✅ Related code stays together
- ✅ Easy to find all components for a feature
- ✅ Better team collaboration (less merge conflicts)
- ✅ Easy to add/remove features

### File Naming

**One class per file**, file name matches class name:

```
CreateEquipmentCommand.cs        // Contains CreateEquipmentCommand class
CreateEquipmentCommandHandler.cs // Contains CreateEquipmentCommandHandler class
```

---

## ⚡ CQRS Implementation

### Commands (Write Operations)

**Purpose**: Modify system state

**Pattern**:
```csharp
// 1. Command
public class CreateEquipmentCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// 2. Handler
public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, Guid>
{
    private readonly IEquipmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEquipmentCommandHandler(
        IEquipmentRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Price = request.Price,
            Status = EquipmentStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(equipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return equipment.Id;
    }
}

// 3. Validator
public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Equipment name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}
```

**Rules**:
- ✅ Commands return `Unit` (void) or created ID (`Guid`)
- ✅ Never return full entities
- ✅ Always validate with FluentValidation
- ✅ Use `CancellationToken` for async operations
- ✅ Wrap in transaction via `IUnitOfWork`

### Queries (Read Operations)

**Purpose**: Retrieve data without side effects

**Pattern**:
```csharp
// 1. Query
public class GetEquipmentByIdQuery : IRequest<EquipmentDto>
{
    public Guid EquipmentId { get; set; }
}

// 2. Handler
public class GetEquipmentByIdQueryHandler : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto>
{
    private readonly IEquipmentRepository _repository;

    public GetEquipmentByIdQueryHandler(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<EquipmentDto> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _repository.GetByIdAsync(request.EquipmentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Equipment with ID {request.EquipmentId} not found");

        return equipment.Adapt<EquipmentDto>();
    }
}

// 3. Validator (optional for queries)
public class GetEquipmentByIdQueryValidator : AbstractValidator<GetEquipmentByIdQuery>
{
    public GetEquipmentByIdQueryValidator()
    {
        RuleFor(x => x.EquipmentId)
            .NotEmpty().WithMessage("Equipment ID is required");
    }
}
```

**Rules**:
- ✅ Queries return DTOs, never entities
- ✅ No side effects (read-only)
- ✅ Use Mapster for entity → DTO mapping
- ✅ Optimize for read performance
- ✅ Consider caching for frequently accessed data

---

## ✅ Validation Rules

### FluentValidation Best Practices

**Always validate**:
```csharp
public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        // Required fields
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Equipment name is required");

        // String length
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        // Numeric ranges
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .LessThan(1000000).WithMessage("Price cannot exceed 1,000,000");

        // Enum validation
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid equipment status");

        // Date validation
        RuleFor(x => x.PurchaseDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Purchase date cannot be in the future");

        // Complex validation
        RuleFor(x => x.WarrantyEndDate)
            .GreaterThan(x => x.PurchaseDate)
            .When(x => x.WarrantyEndDate.HasValue)
            .WithMessage("Warranty end date must be after purchase date");

        // Collection validation
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required")
            .Must(items => items.Count <= 1000).WithMessage("Cannot process more than 1000 items");

        // Nested validation
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");
        });
    }
}
```

**Validation Pipeline** (automatic):
```csharp
// Configured in DependencyInjection.cs
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});
```

---

## 🚨 Error Handling

### Exception Types

**Domain Exceptions**:
```csharp
// Use built-in exceptions
throw new KeyNotFoundException($"Equipment with ID {id} not found");
throw new InvalidOperationException("Cannot assign equipment that is already assigned");
throw new ArgumentException("Equipment ID is required", nameof(equipmentId));
```

**Validation Exceptions**:
```csharp
// Automatic via ValidationBehavior
// Returns 400 Bad Request with validation errors
```

### Controller Error Handling

```csharp
[ApiController]
[Route("api/[controller]")]
public class EquipmentsController : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipmentDto>> GetById(Guid id)
    {
        try
        {
            var query = new GetEquipmentByIdQuery { EquipmentId = id };
            var equipment = await _mediator.Send(query);
            return Ok(equipment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
```

**Global Exception Handling** (Middleware):
```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "An error occurred",
                Message = error.Error.Message
            });
        }
    });
});
```

---

## 🧪 Testing Guidelines

### Unit Test Structure

**Naming Convention**: `MethodName_Scenario_ExpectedBehavior`

```csharp
public class CreateEquipmentCommandValidatorTests
{
    private readonly CreateEquipmentCommandValidator _validator;

    public CreateEquipmentCommandValidatorTests()
    {
        _validator = new CreateEquipmentCommandValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateEquipmentCommand { Name = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Equipment name is required");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenNameIsValid()
    {
        // Arrange
        var command = new CreateEquipmentCommand { Name = "Valid Name" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
```

### Handler Tests with Mocking

```csharp
public class CreateEquipmentCommandHandlerTests
{
    private readonly Mock<IEquipmentRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateEquipmentCommandHandler _handler;

    public CreateEquipmentCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEquipmentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateEquipmentCommandHandler(
            _repositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateEquipment_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateEquipmentCommand
        {
            Name = "Test Equipment",
            Price = 1000
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(x => x.AddAsync(
            It.IsAny<Equipment>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

### Test Coverage Goals

- ✅ **Validators**: 100% coverage
- ✅ **Command Handlers**: 80%+ coverage
- ✅ **Query Handlers**: 70%+ coverage
- ✅ **Domain Logic**: 90%+ coverage

---

## 🗄️ Database Guidelines

### Entity Configuration

**Always use Fluent API**, not data annotations:

```csharp
public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Required fields
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Precision for decimals
        builder.Property(e => e.Price)
            .HasPrecision(18, 2);

        // Indexes
        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.HasIndex(e => e.Status);

        // Relationships
        builder.HasMany(e => e.Assignments)
            .WithOne(a => a.Equipment)
            .HasForeignKey(a => a.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Global query filter (soft delete)
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Table name
        builder.ToTable("Equipments");
    }
}
```

### Migrations

**Create migrations**:
```bash
# From Infrastructure project directory
dotnet ef migrations add AddEquipmentTable --startup-project ../presentations/EquipmentManagement.WebAPI

# Apply migrations
dotnet ef database update --startup-project ../presentations/EquipmentManagement.WebAPI
```

**Migration Naming**: `{Verb}{What}{When}`
- `AddEquipmentTable`
- `UpdateAssignmentStatus`
- `RemoveDeprecatedFields`

---

## 🌐 API Design

### RESTful Conventions

| HTTP Method | Endpoint | Purpose |
|-------------|----------|---------|
| GET | `/api/equipments` | List all (paginated) |
| GET | `/api/equipments/{id}` | Get single item |
| POST | `/api/equipments` | Create new |
| PUT | `/api/equipments/{id}` | Update entire resource |
| PATCH | `/api/equipments/{id}` | Partial update |
| DELETE | `/api/equipments/{id}` | Delete (soft) |

### Controller Best Practices

```csharp
[ApiController]
[Route("api/[controller]")]
[Tags("Equipments")]
public class EquipmentsController(IMediator mediator, ILogger<EquipmentsController> logger) 
    : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of equipments with optional filters.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 100)</param>
    /// <param name="status">Optional equipment status filter</param>
    /// <returns>Paginated list of equipments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetEquipmentsQueryResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetEquipmentsQueryResult>> GetEquipments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] EquipmentStatus? status = null)
    {
        var query = new GetEquipmentsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status
        };

        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new equipment.
    /// </summary>
    /// <param name="command">Equipment creation data</param>
    /// <returns>ID of the created equipment</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateEquipment([FromBody] CreateEquipmentCommand command)
    {
        logger.LogInformation("Creating equipment: {Name}", command.Name);
        
        var id = await mediator.Send(command);
        
        return CreatedAtAction(nameof(GetEquipmentById), new { id }, id);
    }
}
```

**Response Status Codes**:
- `200 OK` - Successful GET/PUT
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Validation error
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## 🔄 Git Workflow

### Branch Naming

```
feature/{module}-{description}
fix/{issue-number}-{description}
hotfix/{critical-issue}
release/{version}
```

**Examples**:
- `feature/equipments-qr-code-generation`
- `feature/assignments-return-workflow`
- `fix/123-null-reference-assignment`
- `hotfix/database-connection-timeout`

### Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Formatting
- `refactor`: Code restructure
- `test`: Adding tests
- `chore`: Maintenance

**Examples**:
```bash
feat(equipments): add QR code generation
fix(assignments): resolve null reference on return
docs(api): update swagger documentation
test(warehouses): add validator unit tests
refactor(maintenances): extract status change logic
```

### Pull Request Guidelines

**PR Title**: Same format as commit messages

**PR Description Template**:
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] New feature
- [ ] Bug fix
- [ ] Breaking change
- [ ] Documentation update

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Unit tests added/updated
- [ ] All tests passing
- [ ] Documentation updated
```

---

## 📚 Additional Resources

- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [Clean Code Principles](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Document Version**: 1.0.0  
**Last Review**: December 12, 2025  
**Maintainer**: Development Team Lead
