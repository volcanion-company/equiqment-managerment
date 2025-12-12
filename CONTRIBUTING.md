# 🤝 Contributing to Equipment Management System

Thank you for your interest in contributing to the Equipment Management System! We welcome contributions from the community and are grateful for your support.

---

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Workflow](#development-workflow)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Commit Guidelines](#commit-guidelines)
- [Testing Requirements](#testing-requirements)
- [Documentation](#documentation)
- [Community](#community)

---

## 📜 Code of Conduct

### Our Pledge

We are committed to providing a welcoming and inclusive environment for all contributors. We expect everyone to:

- ✅ Be respectful and considerate
- ✅ Use welcoming and inclusive language
- ✅ Accept constructive criticism gracefully
- ✅ Focus on what's best for the community
- ✅ Show empathy towards other community members

### Unacceptable Behavior

- ❌ Harassment, trolling, or discriminatory language
- ❌ Personal or political attacks
- ❌ Publishing others' private information without permission
- ❌ Any conduct that could reasonably be considered inappropriate

---

## 🛠️ How Can I Contribute?

### Reporting Bugs

**Before submitting a bug report**:
1. Check [existing issues](https://github.com/volcanion-company/volcanion-device-management/issues) to avoid duplicates
2. Ensure you're using the latest version
3. Collect relevant information (logs, screenshots, steps to reproduce)

**Bug Report Template**:

```markdown
## Bug Description
Clear and concise description of the bug.

## Steps to Reproduce
1. Go to '...'
2. Click on '...'
3. See error

## Expected Behavior
What you expected to happen.

## Actual Behavior
What actually happened.

## Environment
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 9.0.0]
- Database: [e.g., PostgreSQL 17.1]
- Browser (if applicable): [e.g., Chrome 120]

## Screenshots
If applicable, add screenshots.

## Logs
```
Paste relevant log output here
```

## Additional Context
Any other information about the problem.
```

### Suggesting Features

**Before submitting a feature request**:
1. Check [existing feature requests](https://github.com/volcanion-company/volcanion-device-management/issues?q=is%3Aissue+label%3Aenhancement)
2. Consider if the feature aligns with project goals
3. Think about how it would benefit other users

**Feature Request Template**:

```markdown
## Feature Description
Clear and concise description of the feature.

## Problem Statement
What problem does this feature solve?

## Proposed Solution
How should this feature work?

## Alternatives Considered
What other solutions have you considered?

## Benefits
Who would benefit from this feature and how?

## Implementation Notes
Any technical considerations or suggestions.
```

### Contributing Code

We welcome code contributions! Here are areas where you can help:

- **New Features**: Implement items from our roadmap
- **Bug Fixes**: Fix reported issues
- **Performance**: Optimize existing code
- **Tests**: Improve test coverage
- **Documentation**: Enhance or fix documentation
- **Refactoring**: Improve code quality

---

## 🔄 Development Workflow

### 1. Fork and Clone

```bash
# Fork the repository on GitHub
# Then clone your fork
git clone https://github.com/YOUR-USERNAME/volcanion-device-management.git
cd volcanion-device-management

# Add upstream remote
git remote add upstream https://github.com/volcanion-company/volcanion-device-management.git
```

### 2. Create a Branch

Follow the branch naming convention:

```bash
# Feature branch
git checkout -b feature/my-feature-name

# Bug fix branch
git checkout -b fix/issue-123-bug-description

# Hotfix branch
git checkout -b hotfix/critical-issue

# Documentation branch
git checkout -b docs/update-readme
```

**Branch Naming Patterns**:
- `feature/` - New features
- `fix/` - Bug fixes
- `hotfix/` - Critical fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Test improvements

### 3. Set Up Development Environment

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Start application
dotnet run --project src/presentations/EquipmentManagement.WebAPI
```

See [GETTING_STARTED.md](docs/GETTING_STARTED.md) for detailed setup instructions.

### 4. Make Your Changes

Follow our [coding standards](docs/GUIDELINES.md):

- ✅ Write clean, maintainable code
- ✅ Follow C# naming conventions
- ✅ Use CQRS pattern for features
- ✅ Add FluentValidation validators
- ✅ Include XML documentation comments
- ✅ Write unit tests for new code

### 5. Test Your Changes

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/EquipmentManagement.UnitTests

# Check for build warnings
dotnet build /warnaserror
```

**Testing Checklist**:
- [ ] All existing tests pass
- [ ] New tests added for new functionality
- [ ] Edge cases covered
- [ ] Validators tested (100% coverage)
- [ ] Handlers tested (80%+ coverage)

### 6. Update Documentation

If your changes require documentation updates:

- [ ] Update XML comments in code
- [ ] Update [API_REFERENCE.md](docs/API_REFERENCE.md) for API changes
- [ ] Update [ARCHITECTURE.md](docs/ARCHITECTURE.md) for architectural changes
- [ ] Update [GUIDELINES.md](docs/GUIDELINES.md) for new patterns
- [ ] Update [README.md](README.md) if necessary

### 7. Commit Your Changes

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```bash
# Stage changes
git add .

# Commit with conventional message
git commit -m "feat(equipments): add QR code scanning feature"
```

See [Commit Guidelines](#commit-guidelines) below.

### 8. Keep Your Branch Updated

```bash
# Fetch upstream changes
git fetch upstream

# Rebase on develop
git rebase upstream/develop

# Resolve conflicts if any
# Then continue rebase
git rebase --continue

# Force push to your fork (if already pushed)
git push --force-with-lease origin feature/my-feature-name
```

### 9. Push to Your Fork

```bash
git push origin feature/my-feature-name
```

### 10. Create Pull Request

1. Go to your fork on GitHub
2. Click **"New Pull Request"**
3. Select `develop` as base branch
4. Fill out the PR template
5. Submit the PR

---

## 📝 Pull Request Process

### PR Title

Use conventional commit format:

```
feat(equipments): add QR code scanning feature
fix(assignments): resolve null reference on return
docs(api): update endpoint documentation
test(warehouses): add validator tests
refactor(maintenances): extract status logic
```

### PR Description Template

```markdown
## Description
Brief description of changes.

## Related Issue
Closes #123

## Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

## How Has This Been Tested?
Describe the tests you ran.

## Checklist
- [ ] My code follows the project's coding standards
- [ ] I have performed a self-review of my code
- [ ] I have commented my code where necessary
- [ ] I have updated the documentation
- [ ] My changes generate no new warnings
- [ ] I have added tests that prove my fix/feature works
- [ ] New and existing unit tests pass locally
- [ ] Any dependent changes have been merged

## Screenshots (if applicable)
Add screenshots to help explain your changes.

## Additional Notes
Any additional information.
```

### Review Process

1. **Automated Checks**: CI/CD pipeline runs tests and builds
2. **Code Review**: Maintainers review your code
3. **Feedback**: Address review comments
4. **Approval**: At least 1 maintainer approval required
5. **Merge**: Maintainer merges your PR

**Review Timeline**:
- Initial review: Within 2-3 business days
- Follow-up reviews: Within 1-2 business days

### After Your PR is Merged

```bash
# Switch to develop
git checkout develop

# Pull latest changes
git pull upstream develop

# Delete feature branch
git branch -d feature/my-feature-name

# Delete remote branch
git push origin --delete feature/my-feature-name
```

---

## 📏 Coding Standards

### General Principles

Follow [SOLID principles](https://en.wikipedia.org/wiki/SOLID):
- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

See [GUIDELINES.md](docs/GUIDELINES.md) for complete coding standards.

### Code Style

**Use EditorConfig**:
```ini
# .editorconfig is included in the project
# Your IDE will automatically apply these settings
```

**Key Rules**:
- 4 spaces for indentation (no tabs)
- Opening braces on new line
- PascalCase for public members
- camelCase for parameters and local variables
- _camelCase for private fields
- Use `var` when type is obvious
- Maximum line length: 120 characters

**Example**:
```csharp
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
            Status = EquipmentStatus.New
        };

        await _repository.AddAsync(equipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return equipment.Id;
    }
}
```

---

## 💬 Commit Guidelines

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

| Type | Description | Example |
|------|-------------|---------|
| `feat` | New feature | `feat(equipments): add QR code generation` |
| `fix` | Bug fix | `fix(assignments): resolve null reference` |
| `docs` | Documentation | `docs(api): update swagger docs` |
| `style` | Formatting | `style(domain): fix indentation` |
| `refactor` | Code restructuring | `refactor(warehouses): extract validation` |
| `test` | Tests | `test(audits): add handler tests` |
| `chore` | Maintenance | `chore(deps): update packages` |
| `perf` | Performance | `perf(queries): optimize database query` |
| `ci` | CI/CD | `ci(github): add workflow` |

### Scopes

| Scope | Description |
|-------|-------------|
| `equipments` | Equipments module |
| `assignments` | Assignments module |
| `warehouses` | Warehouses module |
| `maintenances` | Maintenances module |
| `liquidations` | Liquidations module |
| `audits` | Audits module |
| `domain` | Domain layer |
| `application` | Application layer |
| `infrastructure` | Infrastructure layer |
| `api` | API/WebAPI layer |

### Examples

**Good Commits**:
```bash
feat(equipments): add equipment status update endpoint
fix(assignments): prevent duplicate assignments
docs(readme): add setup instructions
test(warehouses): add transaction validator tests
refactor(maintenances): extract status change logic
```

**Bad Commits** (avoid):
```bash
update code
fix bug
changes
WIP
asdf
```

### Commit Best Practices

- ✅ Use imperative mood ("add feature" not "added feature")
- ✅ Keep subject line under 72 characters
- ✅ Capitalize subject line
- ✅ Don't end subject with a period
- ✅ Separate subject from body with blank line
- ✅ Use body to explain what and why (not how)
- ✅ Reference issues in footer

**Example with body**:
```
feat(audits): add batch upload endpoint for mobile

Implement batch creation of audit records to support offline
mobile app scenarios. Allows up to 1000 records per request
with partial success handling.

Closes #45
```

---

## ✅ Testing Requirements

### Test Coverage Goals

| Component | Coverage Goal |
|-----------|--------------|
| Validators | 100% |
| Command Handlers | 80%+ |
| Query Handlers | 70%+ |
| Domain Logic | 90%+ |

### Writing Tests

**Naming Convention**: `MethodName_Scenario_ExpectedBehavior`

```csharp
[Fact]
public async Task Handle_ShouldCreateEquipment_WhenCommandIsValid()
{
    // Arrange
    var command = new CreateEquipmentCommand { Name = "Test" };

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.Should().NotBeEmpty();
}
```

### Running Tests Locally

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~CreateEquipmentCommandHandlerTests"

# Watch mode
dotnet watch test
```

---

## 📚 Documentation

### Code Documentation

**XML Comments** for public APIs:

```csharp
/// <summary>
/// Creates a new equipment in the system.
/// </summary>
/// <param name="request">Equipment creation data</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>The ID of the created equipment</returns>
/// <exception cref="ValidationException">When validation fails</exception>
public async Task<Guid> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
{
    // Implementation
}
```

### README Updates

Update [README.md](README.md) for:
- New features
- Breaking changes
- Setup changes
- Dependency updates

### API Documentation

Update [API_REFERENCE.md](docs/API_REFERENCE.md) for:
- New endpoints
- Changed parameters
- New response formats
- Deprecated endpoints

---

## 👥 Community

### Getting Help

- **Documentation**: Check [docs/](docs/) folder
- **Issues**: [GitHub Issues](https://github.com/volcanion-company/volcanion-device-management/issues)
- **Discussions**: [GitHub Discussions](https://github.com/volcanion-company/volcanion-device-management/discussions)
- **Email**: dev@volcanion-company.com

### Stay Updated

- Watch the repository for updates
- Star the project to show support
- Follow [@VolcanionCompany](https://github.com/volcanion-company)

### Recognition

Contributors will be:
- Listed in [README.md](README.md)
- Mentioned in release notes
- Credited in documentation

---

## 🙏 Thank You!

Your contributions make this project better for everyone. We appreciate your time and effort!

---

**Questions?** Open a [discussion](https://github.com/volcanion-company/volcanion-device-management/discussions) or email us at dev@volcanion-company.com

---

**Version**: 1.0.0  
**Last Updated**: December 12, 2025  
**Maintainer**: Development Team
