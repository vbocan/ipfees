# Developer Guide

This guide provides detailed information for developers who want to extend, modify, or contribute to IPFees.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [DSL Language Specification](#dsl-language-specification)
3. [Adding New Jurisdictions](#adding-new-jurisdictions)
4. [API Reference](#api-reference)
5. [Extension Points](#extension-points)
6. [Testing Guide](#testing-guide)

## Architecture Overview

### System Components

```
┌─────────────────────────────────────────────────────────┐
│                    IPFees.Web                           │
│              (Presentation Layer)                       │
│  - Razor Pages                                          │
│  - REST API Controllers                                 │
│  - Swagger/OpenAPI                                      │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│                 IPFees.Core                             │
│              (Business Logic Layer)                     │
│  - Domain Models                                        │
│  - Service Interfaces                                   │
│  - Business Logic                                       │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│             IPFees.Calculator                           │
│           (Calculation Engine)                          │
│  - DSL Parser & Interpreter                             │
│  - Fee Calculation Logic                                │
│  - Expression Evaluator                                 │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│                  MongoDB                                │
│                (Data Layer)                             │
│  - Jurisdiction Data                                    │
│  - Fee Structures                                       │
│  - Currency Rates                                       │
└─────────────────────────────────────────────────────────┘
```

### Technology Stack Rationale

- **.NET 9.0**: Modern, high-performance framework with excellent cross-platform support
- **ASP.NET Core**: For web hosting and REST API capabilities
- **MongoDB**: Flexible schema for diverse jurisdiction fee structures
- **Docker**: Ensures reproducibility and easy deployment
- **xUnit**: Industry-standard testing framework

## DSL Language Specification

### Grammar Definition

The IPFees DSL uses a simple expression-based syntax for defining fee calculations:

```
Expression := Assignment | Calculation
Assignment := Identifier '=' Expression
Calculation := Term (('+' | '-') Term)*
Term := Factor (('*' | '/') Factor)*
Factor := Number | Identifier | '(' Expression ')' | Function
Function := Identifier '(' Arguments ')'
Conditional := Expression '?' Expression ':' Expression
```

### Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `+` | Addition | `BaseFee + ClaimFee` |
| `-` | Subtraction | `Total - Discount` |
| `*` | Multiplication | `Claims * FeePerClaim` |
| `/` | Division | `Total / ExchangeRate` |
| `>`, `<`, `>=`, `<=` | Comparison | `Claims > 20` |
| `==`, `!=` | Equality | `EntitySize == "Small"` |
| `?:` | Ternary conditional | `Claims > 20 ? Extra : 0` |

### Built-in Variables

The following variables are available in fee calculations:

- `Claims` - Total number of claims
- `IndependentClaims` - Number of independent claims
- `Pages` - Number of pages in specification
- `EntitySize` - Entity size ("Micro", "Small", "Large")
- `FilingDate` - Date of filing
- `Country` - Jurisdiction country code

### Built-in Functions

| Function | Description | Example |
|----------|-------------|---------|
| `Max(a, b)` | Maximum of two values | `Max(Claims - 20, 0)` |
| `Min(a, b)` | Minimum of two values | `Min(Claims, 100)` |
| `Round(value, decimals)` | Round to decimal places | `Round(Total, 2)` |
| `Ceil(value)` | Round up | `Ceil(Pages / 50)` |
| `Floor(value)` | Round down | `Floor(Total)` |

### Example Fee Structures

#### USPTO Utility Patent (Simplified)

```
// Base filing fees
BaseFee = EntitySize == "Micro" ? 75 : (EntitySize == "Small" ? 150 : 300)
SearchFee = EntitySize == "Micro" ? 168 : (EntitySize == "Small" ? 336 : 672)
ExamFee = EntitySize == "Micro" ? 184 : (EntitySize == "Small" ? 368 : 736)

// Excess claim fees
ExcessClaims = Max(Claims - 20, 0)
ExcessClaimFee = ExcessClaims * (EntitySize == "Micro" ? 26 : (EntitySize == "Small" ? 52 : 104))

ExcessIndependentClaims = Max(IndependentClaims - 3, 0)
ExcessIndependentClaimFee = ExcessIndependentClaims * (EntitySize == "Micro" ? 105 : (EntitySize == "Small" ? 210 : 420))

// Total
TotalFee = BaseFee + SearchFee + ExamFee + ExcessClaimFee + ExcessIndependentClaimFee
```

## Adding New Jurisdictions

### Step-by-Step Process

1. **Create Jurisdiction Configuration**

Create a JSON file in `wwwroot/data/jurisdictions/`:

```json
{
  "jurisdictionCode": "CA",
  "name": "Canada (CIPO)",
  "currency": "CAD",
  "feeTypes": [
    {
      "feeTypeCode": "UTILITY_FILING",
      "name": "Utility Patent Filing",
      "description": "Filing fee for utility patent application",
      "entitySizes": ["Standard", "Small"],
      "rules": "BaseFee = EntitySize == 'Small' ? 200 : 400\nClaimFee = Claims > 20 ? (Claims - 20) * 50 : 0\nTotalFee = BaseFee + ClaimFee"
    }
  ]
}
```

2. **Define Fee Calculation Rules**

Use the DSL to express fee logic:
- Start with base fees
- Add variable components (claims, pages, etc.)
- Include entity-based discounts
- Calculate totals

3. **Add Test Cases**

Create test file in `IPFees.Calculator.Tests/Jurisdictions/`:

```csharp
public class CanadaFeeCalculatorTests
{
    [Fact]
    public void Calculate_StandardEntity_ReturnsCorrectFee()
    {
        // Arrange
        var calculator = new FeeCalculator();
        var request = new FeeCalculationRequest
        {
            Jurisdiction = "CA",
            FeeType = "UTILITY_FILING",
            EntitySize = "Standard",
            Claims = 25
        };

        // Act
        var result = calculator.Calculate(request);

        // Assert
        Assert.Equal(650, result.TotalFee); // 400 + (5 * 50)
    }
}
```

4. **Update Documentation**

Add jurisdiction to supported list and document any special requirements.

## API Reference

### Fee Calculation Endpoint

**POST** `/api/fees/calculate`

Calculate fees for a single jurisdiction and fee type.

**Request Body:**
```json
{
  "jurisdiction": "USPTO",
  "feeType": "UTILITY_FILING",
  "entitySize": "Small",
  "claims": 25,
  "independentClaims": 4,
  "pages": 30,
  "targetCurrency": "USD"
}
```

**Response:**
```json
{
  "jurisdiction": "USPTO",
  "feeType": "UTILITY_FILING",
  "baseFee": 150.00,
  "additionalFees": {
    "searchFee": 336.00,
    "examFee": 368.00,
    "excessClaimFee": 260.00,
    "excessIndependentClaimFee": 210.00
  },
  "totalFee": 1324.00,
  "currency": "USD",
  "exchangeRate": 1.0,
  "calculatedAt": "2025-10-24T09:29:35Z"
}
```

### Bulk Calculation Endpoint

**POST** `/api/fees/bulk-calculate`

Calculate fees for multiple jurisdictions simultaneously.

**Request Body:**
```json
{
  "requests": [
    {
      "jurisdiction": "USPTO",
      "feeType": "UTILITY_FILING",
      "entitySize": "Small",
      "claims": 20
    },
    {
      "jurisdiction": "EPO",
      "feeType": "FILING",
      "claims": 20
    }
  ],
  "targetCurrency": "EUR"
}
```

### Currency Conversion Endpoint

**GET** `/api/currency/rates?base=USD&target=EUR`

Get current exchange rates between currencies.

## Extension Points

### Custom Currency Provider

Implement `ICurrencyProvider` to integrate custom exchange rate sources:

```csharp
public interface ICurrencyProvider
{
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency, DateTime? date = null);
    Task<CurrencyRateStatus> GetRateStatusAsync();
}
```

### Custom Fee Calculator

Extend `BaseFeeCalculator` for jurisdiction-specific complex logic:

```csharp
public class CustomFeeCalculator : BaseFeeCalculator
{
    public override FeeCalculationResult Calculate(FeeCalculationRequest request)
    {
        // Custom calculation logic
        return base.Calculate(request);
    }
}
```

### Custom DSL Functions

Add custom functions to the DSL interpreter:

```csharp
public class CustomDslFunction : IDslFunction
{
    public string Name => "CustomFunction";
    
    public object Evaluate(params object[] args)
    {
        // Implementation
        return result;
    }
}
```

## Testing Guide

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test IPFees.Core.Tests
dotnet test IPFees.Calculator.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "FullyQualifiedName~CanadaFeeCalculatorTests"
```

### Test Structure

Tests follow the Arrange-Act-Assert pattern:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and dependencies
    var calculator = new FeeCalculator();
    var request = CreateTestRequest();

    // Act - Execute the method being tested
    var result = calculator.Calculate(request);

    // Assert - Verify the outcome
    Assert.Equal(expected, result.TotalFee);
}
```

### Test Categories

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **API Tests**: Test REST API endpoints
- **DSL Tests**: Test DSL parser and evaluator

### Test Data

Sample data is provided in `wwwroot/data/` for development and testing. Use `TestDataBuilder` classes for creating test fixtures.

## Best Practices

1. **Code Quality**
   - Follow SOLID principles
   - Keep methods focused and concise
   - Use dependency injection
   - Add XML documentation comments

2. **Performance**
   - Use async/await for I/O operations
   - Cache frequently accessed data
   - Minimize database queries
   - Use efficient data structures

3. **Security**
   - Validate all inputs
   - Use parameterized queries
   - Follow principle of least privilege
   - Keep dependencies updated

4. **Documentation**
   - Update docs when changing functionality
   - Add code comments for complex logic
   - Include examples in API documentation
   - Keep README current

## Getting Help

- **Documentation**: Check this guide and README.md first
- **Issues**: Search existing GitHub issues
- **Discussions**: Use GitHub Discussions for questions
- **Contributing**: See [CONTRIBUTING.md](../CONTRIBUTING.md)

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [MongoDB C# Driver](https://mongodb.github.io/mongo-csharp-driver/)
- [xUnit Documentation](https://xunit.net/)
- [Docker Documentation](https://docs.docker.com/)
