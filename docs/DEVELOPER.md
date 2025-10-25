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

### IPFLang Overview

IPFees uses **IPFLang** (Intellectual Property Fees Language), a declarative DSL with keyword-based syntax designed for legal professionals. The language uses structured blocks with explicit keywords rather than expression-based syntax.

### Input Definitions

IPFLang supports five input types:

#### 1. Single-Selection List (LIST)
```
DEFINE LIST EntityType AS 'Select the desired entity type'
GROUP G1
CHOICE NormalEntity AS 'Normal'
CHOICE SmallEntity AS 'Small'
CHOICE MicroEntity AS 'Micro'
DEFAULT NormalEntity
ENDDEFINE
```

#### 2. Multiple-Selection List (MULTILIST)
```
DEFINE MULTILIST Countries AS 'Select validation countries'
GROUP G2
CHOICE VAL_DE AS 'Germany'
CHOICE VAL_FR AS 'France'
CHOICE VAL_GB AS 'United Kingdom'
DEFAULT VAL_DE,VAL_FR
ENDDEFINE
```

#### 3. Number Input (NUMBER)
```
DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
GROUP G3
BETWEEN 10 AND 1000
DEFAULT 15
ENDDEFINE
```

#### 4. Boolean Input (BOOLEAN)
```
DEFINE BOOLEAN ContainsDependentClaims AS 'Does this contain dependent claims?'
GROUP G4
DEFAULT TRUE
ENDDEFINE
```

#### 5. Date Input (DATE)
```
DEFINE DATE ApplicationDate AS 'Application filing date'
GROUP G5
BETWEEN 1990-01-01 AND 2025-12-31
DEFAULT 2024-01-01
ENDDEFINE
```

### Fee Computation

Fee calculations use `COMPUTE FEE` blocks with `YIELD` statements:

```
COMPUTE FEE BasicNationalFee
YIELD 320 IF EntityType EQ NormalEntity
YIELD 128 IF EntityType EQ SmallEntity
YIELD 64 IF EntityType EQ MicroEntity
ENDCOMPUTE
```

### Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `EQ` | Equality | `EntityType EQ SmallEntity` |
| `NEQ` | Not equal | `EntityType NEQ NormalEntity` |
| `GT` | Greater than | `ClaimCount GT 20` |
| `LT` | Less than | `SheetCount LT 100` |
| `GTE` | Greater than or equal | `ClaimCount GTE 15` |
| `LTE` | Less than or equal | `SheetCount LTE 50` |
| `AND` | Logical AND | `ClaimCount GT 10 AND EntityType EQ SmallEntity` |
| `OR` | Logical OR | `EntityType EQ SmallEntity OR EntityType EQ MicroEntity` |
| `NOT` | Logical NOT | `NOT ContainsDependentClaims` |
| `IN` | Set membership | `VAL_DE IN Countries` |
| `NIN` | Not in set | `VAL_FR NIN Countries` |

### Arithmetic Operations

Standard arithmetic operators are supported:
- Addition: `+`
- Subtraction: `-`
- Multiplication: `*`
- Division: `/`

### Built-in Functions

| Function | Description | Example |
|----------|-------------|---------|
| `ROUND(value)` | Round to nearest integer | `ROUND(SheetCount * 1.5)` |
| `ROUND(value, decimals)` | Round to decimal places | `ROUND(Total, 2)` |

### Date Functions

Date inputs support temporal calculations:

| Function | Description | Example |
|----------|-------------|---------|
| `!MONTHSTONOW` | Months from date to now | `ApplicationDate!MONTHSTONOW` |
| `!MONTHSTONOW_FROMLASTDAY` | Months from last day of month to now | `ApplicationDate!MONTHSTONOW_FROMLASTDAY` |
| `!YEARSTONOW` | Years from date to now | `ApplicationDate!YEARSTONOW` |

### List Operations

Multiple-selection lists support count operations:

```
DEFINE MULTILIST Countries AS 'Select countries'
CHOICE VAL_DE AS 'Germany'
CHOICE VAL_FR AS 'France'
DEFAULT VAL_DE
ENDDEFINE

COMPUTE FEE ValidationFee
LET CountryCount AS Countries!COUNT
YIELD CountryCount * 100
ENDCOMPUTE
```

### Variable Assignment

Use `LET` statements for intermediate calculations:

```
COMPUTE FEE ClaimFee
LET CF1 AS 265
LET CF2 AS 660
YIELD CF1*(ClaimCount-15) IF ClaimCount GT 15 AND ClaimCount LT 51
YIELD CF2*(ClaimCount-50) + CF1*35 IF ClaimCount GT 50
ENDCOMPUTE
```

### Conditional Logic with CASE

Use `CASE` blocks for complex conditional logic:

```
COMPUTE FEE SearchFee
CASE SituationType EQ PreparedIPEA AS
YIELD 0 IF EntityType EQ NormalEntity
YIELD 0 IF EntityType EQ SmallEntity
YIELD 0 IF EntityType EQ MicroEntity
ENDCASE
CASE SituationType EQ PaidAsISA AS
YIELD 140 IF EntityType EQ NormalEntity
YIELD 56 IF EntityType EQ SmallEntity
YIELD 28 IF EntityType EQ MicroEntity
ENDCASE
YIELD 700 IF EntityType EQ NormalEntity
YIELD 280 IF EntityType EQ SmallEntity
YIELD 140 IF EntityType EQ MicroEntity
ENDCOMPUTE
```

### Optional Fees

Mark fees as optional when they may not apply:

```
COMPUTE FEE ExaminationFee OPTIONAL
LET ExFee AS 3000000
YIELD ExFee
ENDCOMPUTE
```

### Return Statements

Specify currency and informational returns:

```
RETURN Currency AS 'EUR'
RETURN ClaimLimits AS '10 claims included; additional fee as of 11'
RETURN ExReqDate AS 'Examination request due within 7 years after filing'
```

### Complete Example

```
# EPO PCT Regional Phase Entry
RETURN Currency AS 'EUR'

DEFINE LIST ISA AS 'International Search Authority'
CHOICE ISA_EPO AS 'EPO'
CHOICE ISA_AT AS 'Austria'
CHOICE ISA_FI AS 'Finland'
DEFAULT ISA_EPO
ENDDEFINE

DEFINE LIST IPRP AS 'International Preliminary Report on Patentability'
CHOICE IPRP_EPO AS 'Prepared by EPO'
CHOICE IPRP_NONE AS 'None'
DEFAULT IPRP_NONE
ENDDEFINE

DEFINE NUMBER SheetCount AS 'Number of sheets'
BETWEEN 1 AND 1000
DEFAULT 35
ENDDEFINE

DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 1 AND 100
DEFAULT 15
ENDDEFINE

COMPUTE FEE OFF_BasicNationalFee
YIELD 135
ENDCOMPUTE

COMPUTE FEE OFF_DesignationFee
YIELD 660
ENDCOMPUTE

COMPUTE FEE OFF_SheetFee
YIELD 17*(SheetCount-35) IF SheetCount GT 35
ENDCOMPUTE

COMPUTE FEE OFF_ClaimFee
LET CF1 AS 265
LET CF2 AS 660
YIELD CF1*(ClaimCount-15) IF ClaimCount GT 15 AND ClaimCount LT 51
YIELD CF2*(ClaimCount-50) + CF1*35 IF ClaimCount GT 50
ENDCOMPUTE

COMPUTE FEE OFF_SearchFee
LET SF1 AS 1460
LET SF2 AS 1185
YIELD 0 IF ISA EQ ISA_EPO
YIELD SF1-SF2 IF ISA EQ ISA_AT OR ISA EQ ISA_FI
YIELD SF1 IF ISA NEQ ISA_EPO
ENDCOMPUTE

COMPUTE FEE OFF_ExaminationFee
LET ExFee1 AS 2055
LET ExFee2 AS 1840
LET Discount AS 0.75
CASE IPRP EQ IPRP_EPO AS
YIELD ExFee1*Discount IF ISA EQ ISA_EPO
YIELD ExFee2*Discount IF ISA NEQ ISA_EPO
ENDCASE
YIELD ExFee1 IF ISA EQ ISA_EPO
YIELD ExFee2 IF ISA NEQ ISA_EPO
ENDCOMPUTE
```

## Adding New Jurisdictions

### Step-by-Step Process

1. **Create IPFLang Fee Structure**

IPFLang files are stored in MongoDB GridFS, not as separate files. However, you can create them through the web interface or directly in the database.

Example IPFLang structure for Canada:

```
# File name: PCT-CA-OFF
# File content: Official fees for PCT national phase: Canada
# Valid until: n/a
RETURN Currency AS 'CAD'

DEFINE LIST EntitySize AS 'Entity size'
CHOICE Entity_Company AS 'Company'
CHOICE Entity_Person AS 'Individual'
DEFAULT Entity_Company
ENDDEFINE

DEFINE NUMBER SheetCount AS 'Number of sheets'
BETWEEN 1 AND 1000
DEFAULT 30
ENDDEFINE

DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 1 AND 100
DEFAULT 10
ENDDEFINE

COMPUTE FEE OFF_BasicNationalFee
LET Fee1 AS 400
LET Fee2 AS 200
YIELD Fee1 IF EntitySize EQ Entity_Company
YIELD Fee2 IF EntitySize EQ Entity_Person
ENDCOMPUTE

COMPUTE FEE OFF_SheetFee
LET Fee1 AS 10
YIELD Fee1*(SheetCount-30) IF SheetCount GT 30
ENDCOMPUTE

COMPUTE FEE OFF_ClaimFee
LET CF1 AS 50
YIELD CF1*(ClaimCount-20) IF ClaimCount GT 20
ENDCOMPUTE
```

2. **Add Jurisdiction to MongoDB**

Insert jurisdiction document in the `jurisdictions` collection:

```json
{
  "Name": "CA",
  "Description": "Canada (CIPO)",
  "AttorneyFeeLevel": "Level1",
  "LastUpdatedOn": "2025-01-15T00:00:00Z"
}
```

3. **Add Fee Structure to MongoDB**

Insert fee structure in the `fees` collection and upload IPFLang code to GridFS:

```json
{
  "Name": "PCT-CA-OFF",
  "Description": "Entry of national phase in Canada",
  "SourceCode": "<IPFLang code from above>",
  "ReferencedModules": [],
  "LastUpdatedOn": "2025-01-15T00:00:00Z",
  "Category": "OfficialFees",
  "JurisdictionName": "CA"
}
```

4. **Test the Jurisdiction**

Create test cases in `IPFees.Calculator.Tests`:

```csharp
[Fact]
public async Task Calculate_CanadaNationalPhase_ReturnsCorrectFee()
{
    // Arrange
    var jurisdictionFeeManager = GetJurisdictionFeeManager();
    var inputs = new List<IPFValue>
    {
        new IPFValueList("EntitySize", "Entity_Company"),
        new IPFValueNumber("SheetCount", 35),
        new IPFValueNumber("ClaimCount", 25)
    };

    // Act
    var result = await jurisdictionFeeManager.Calculate(
        new[] { "CA" }, 
        inputs, 
        "CAD", 
        0);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result.Errors);
    var caResult = result.Results.First(r => r.Jurisdiction == "CA");
    
    // OFF_BasicNationalFee: 400
    // OFF_SheetFee: 10 * (35-30) = 50
    // OFF_ClaimFee: 50 * (25-20) = 250
    // Total: 700
    Assert.Equal(700, caResult.TotalFee);
}
```
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

### Get Calculation Parameters

**GET** `/api/v1/Fee/Parameters/{Jurisdictions}`

Get required input parameters for specified jurisdictions.

**Example Request:**
```
GET /api/v1/Fee/Parameters/EP,US,CA
Headers: X-API-Key: <api-key>
```

**Example Response:**
```json
{
  "parameters": [
    {
      "Type": "String",
      "Name": "EntitySize",
      "ExpectedValues": ["Entity_Company", "Entity_Person"]
    },
    {
      "Type": "Number",
      "Name": "SheetCount",
      "ExpectedValues": null
    },
    {
      "Type": "Number",
      "Name": "ClaimCount",
      "ExpectedValues": null
    }
  ]
}
```

**Note**: The actual API returns parameters as JSON-encoded strings within an array. The above shows the logical structure for clarity.

### Calculate Fees

**POST** `/api/v1/Fee/Calculate`

Calculate fees for specified jurisdictions.

**Request Body:**
```json
{
  "jurisdictions": "EP,CA",
  "parameters": [
    {"type": "String", "name": "EntitySize", "value": "Entity_Company"},
    {"type": "String", "name": "ISA", "value": "ISA_EPO"},
    {"type": "String", "name": "IPRP", "value": "IPRP_NONE"},
    {"type": "Number", "name": "SheetCount", "value": 40},
    {"type": "Number", "name": "ClaimCount", "value": 20}
  ],
  "targetCurrency": "EUR"
}
```

**Response:**
```json
{
  "results": [
    {
      "jurisdiction": "EP",
      "currency": "EUR",
      "fees": [
        {"name": "OFF_BasicNationalFee", "amount": 135},
        {"name": "OFF_DesignationFee", "amount": 660},
        {"name": "OFF_SheetFee", "amount": 85},
        {"name": "OFF_ClaimFee", "amount": 1325},
        {"name": "OFF_SearchFee", "amount": 0},
        {"name": "OFF_ExaminationFee", "amount": 2055}
      ],
      "totalFee": 4260,
      "targetCurrency": "EUR",
      "exchangeRate": 1.0
    }
  ],
  "errors": []
}
```

### Get Jurisdictions

**GET** `/api/v1/Jurisdiction`

Get list of available jurisdictions.

**Example Response:**
```json
[
  {"jurisdiction": "EP", "description": "European Patent Organisation"},
  {"jurisdiction": "US", "description": "United States of America"},
  {"jurisdiction": "CA", "description": "Canada (CIPO)"}
]
```

### Get Currencies

**GET** `/api/v1/Currency`

Get list of supported currencies.

**Example Response:**
```json
[
  {"currency": "USD", "description": "US Dollar"},
  {"currency": "EUR", "description": "Euro"},
  {"currency": "GBP", "description": "British Pound"}
]
```

## Extension Points

### Custom Currency Provider

Implement `ICurrencyConverter` to integrate custom exchange rate sources:

```csharp
public interface ICurrencyConverter
{
    Task<decimal> ConvertAsync(
        decimal amount, 
        string fromCurrency, 
        string toCurrency, 
        decimal markup);
    
    IEnumerable<(string, string)> GetCurrencies();
}
```

### Adding Custom DSL Functions

The DSL parser is extensible, but core functions are built into the evaluator. To add custom functions, you would need to modify the `IPFees.Evaluator` project. However, most use cases can be handled with the existing operators and `LET` statements for intermediate calculations.

### Custom Modules

Modules are reusable IPFLang code blocks that can be referenced by multiple fee structures. Example:

```
# Module: ApplicationDate
DEFINE DATE ApplicationDate AS 'Application filing date'
BETWEEN 1990-01-01 AND 2030-12-31
DEFAULT 2024-01-01
ENDDEFINE
```

Fee structures reference modules:

```
# MODULES: ApplicationDate; SheetCount; ClaimCount

COMPUTE FEE OFF_Renewal
LET RenMonth AS ROUND(ApplicationDate!MONTHSTONOW)
YIELD 250
YIELD 300 IF RenMonth GTE 12
YIELD 350 IF RenMonth GTE 24
ENDCOMPUTE
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
public async Task Calculate_EPORegionalPhase_ReturnsCorrectFees()
{
    // Arrange
    var jurisdictionFeeManager = GetJurisdictionFeeManager();
    var inputs = new List<IPFValue>
    {
        new IPFValueList("ISA", "ISA_EPO"),
        new IPFValueList("IPRP", "IPRP_NONE"),
        new IPFValueNumber("SheetCount", 40),
        new IPFValueNumber("ClaimCount", 20)
    };

    // Act
    var result = await jurisdictionFeeManager.Calculate(
        new[] { "EP" }, 
        inputs, 
        "EUR", 
        0);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result.Errors);
    
    var epResult = result.Results.First(r => r.Jurisdiction == "EP");
    Assert.Equal(4260, epResult.TotalFee); // 135+660+85+1325+0+2055
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
