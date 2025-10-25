# Contributing to IPFees

Thank you for considering contributing to IPFees! This document provides guidelines for contributing to the project.

## Code of Conduct

We are committed to providing a welcoming and inclusive environment for all contributors. Please be respectful and constructive in all interactions.

## How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with:
- Clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, .NET version, Docker version)
- Screenshots if applicable

### Suggesting Enhancements

For feature requests or enhancements:
- Check existing issues to avoid duplicates
- Clearly describe the proposed feature
- Explain the use case and benefits
- Consider implementation complexity

### Pull Requests

1. **Fork the repository** and create your branch from `main`
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Follow existing code style and conventions
   - Add or update tests for new functionality
   - Update documentation as needed

3. **Test your changes**
   ```bash
   # Run all tests
   dotnet test
   
   # Run specific test projects
   dotnet test IPFees.Core.Tests
   dotnet test IPFees.Calculator.Tests
   ```

4. **Commit your changes**
   - Use clear, descriptive commit messages
   - Reference relevant issue numbers
   ```bash
   git commit -m "Add feature: description (#issue-number)"
   ```

5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Open a Pull Request**
   - Provide a clear description of changes
   - Reference related issues
   - Include screenshots for UI changes
   - Ensure all tests pass

## Development Setup

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [MongoDB](https://www.mongodb.com/try/download/community) (or use Docker)
- Git

### Local Development

```bash
# Clone your fork
git clone https://github.com/YOUR-USERNAME/ipfees.git
cd ipfees

# Start services with Docker
cd src
docker-compose up -d

# Or run locally
cd IPFees.Web
dotnet run

# Run tests
cd ../..
dotnet test
```

## Coding Standards

### C# Style Guidelines

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and concise
- Use async/await for I/O operations

### Testing

- Write unit tests for new functionality
- Maintain or improve test coverage
- Use descriptive test names that explain intent
- Follow Arrange-Act-Assert pattern

### Documentation

- Update README.md for user-facing changes
- Update architecture.md for architectural changes
- Add XML comments for public APIs
- Update API documentation (Swagger) as needed

## Adding New Jurisdictions

To add support for a new IP jurisdiction:

1. Create fee structure definition in `wwwroot/data/`
2. Define fee calculation rules using DSL
3. Add currency information if needed
4. Create test cases in `IPFees.Calculator.Tests`
5. Update documentation

See [docs/developer.md](docs/developer.md) for detailed instructions.

## Project Structure

```
ipfees/
├── src/
│   ├── IPFees.Web/           # Web application (Razor Pages)
│   ├── IPFees.Core/          # Core domain models and services
│   ├── IPFees.Calculator/    # Fee calculation engine and DSL
│   ├── IPFees.Core.Tests/    # Core unit tests
│   └── IPFees.Calculator.Tests/ # Calculator tests
├── docs/                      # Documentation
├── LICENSE                    # MIT License
└── README.md                  # Project overview
```

## Questions?

- **General questions**: Use [GitHub Discussions](https://github.com/[username]/ipfees/discussions)
- **Bug reports**: Create a [GitHub Issue](https://github.com/[username]/ipfees/issues)
- **Security concerns**: Email [security contact] privately

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be recognized in:
- Repository contributors list
- Release notes for significant contributions
- Academic publications (if applicable)

Thank you for helping improve IPFees!
