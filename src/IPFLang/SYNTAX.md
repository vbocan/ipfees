# IPFLang Syntax Reference

IPFLang is a domain-specific language for defining intellectual property fee calculations with advanced features for currency handling, completeness verification, provenance tracking, and version management. This document provides a complete syntax reference for IP practitioners and developers.

**Key Features:**
- Currency-aware type system with compile-time validation
- Static completeness and monotonicity verification
- Provenance tracking with counterfactual analysis
- Version management with effective dates and regulatory references

---

## Table of Contents

1. [Basic Concepts](#basic-concepts)
2. [Comments](#comments)
3. [Version Declaration](#version-declaration) *(versioning)*
4. [Input Definitions](#input-definitions)
   - [LIST](#list-input)
   - [MULTILIST](#multilist-input)
   - [NUMBER](#number-input)
   - [BOOLEAN](#boolean-input)
   - [DATE](#date-input)
   - [AMOUNT](#amount-input) *(currency-aware)*
5. [Groups](#groups)
6. [Fee Computations](#fee-computations)
   - [Basic Fees](#basic-fees)
   - [Optional Fees](#optional-fees)
   - [Cases and Conditions](#cases-and-conditions)
   - [Local Variables (LET)](#local-variables)
   - [Polymorphic Fees](#polymorphic-fees) *(currency-aware)*
7. [Expressions](#expressions)
   - [Arithmetic](#arithmetic-operators)
   - [Comparisons](#comparison-operators)
   - [Logical](#logical-operators)
   - [Built-in Functions](#built-in-functions)
   - [Currency Literals](#currency-literals) *(currency-aware)*
   - [Currency Conversion](#currency-conversion) *(currency-aware)*
8. [Returns](#returns)
9. [Verification Directives](#verification-directives) *(completeness checking)*
   - [VERIFY COMPLETE](#verify-complete)
   - [VERIFY MONOTONIC](#verify-monotonic)
10. [Complete Example](#complete-example)

---

## Basic Concepts

IPFLang scripts define:
- **Versions**: Optional metadata for tracking fee schedule changes over time
- **Inputs**: Variables that users provide (entity type, claim count, dates, fees)
- **Fees**: Computed amounts based on input values
- **Returns**: Named outputs from the calculation
- **Verification**: Static analysis directives for completeness and monotonicity

The calculator processes inputs through fee definitions and produces totals, with support for:
- **Currency-aware type checking**: Prevents accidental cross-currency arithmetic
- **Completeness verification**: Ensures all input combinations are covered
- **Provenance tracking**: Records which rules contributed to each fee
- **Version management**: Tracks fee schedule changes with effective dates

---

## Comments

Use `#` for single-line comments. Comments can appear anywhere except inside quoted strings.

```
# This is a comment
DEFINE NUMBER ClaimCount AS 'Number of claims'  # Inline comment
BETWEEN 0 AND 100
DEFAULT 1
ENDDEFINE
```

---

## Version Declaration

Declare the version of a fee schedule with an effective date and optional metadata. This enables version tracking and temporal queries.

**Syntax:**
```
VERSION '<VersionId>' EFFECTIVE yyyy-MM-dd [DESCRIPTION '<description>'] [REFERENCE '<reference>']
```

**Parameters:**
- `VersionId`: Unique identifier for this version (e.g., '2024.1', '1.0.0')
- `EFFECTIVE yyyy-MM-dd`: Date when this version becomes effective (ISO 8601 format)
- `DESCRIPTION` (optional): Human-readable description of changes
- `REFERENCE` (optional): Regulatory reference (e.g., Federal Register citation)

**Examples:**

Basic version declaration:
```
VERSION '2024.1' EFFECTIVE 2024-01-15
```

With description:
```
VERSION '2024.1' EFFECTIVE 2024-01-15 DESCRIPTION 'Annual fee increase'
```

With regulatory reference:
```
VERSION '2024.1' EFFECTIVE 2024-01-15 REFERENCE 'Federal Register Vol. 89, No. 123'
```

Complete example:
```
VERSION '2024.1' EFFECTIVE 2024-01-15 DESCRIPTION 'USPTO fee increase' REFERENCE 'Federal Register Vol. 89, No. 123'

DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 1 AND 100
DEFAULT 1
ENDDEFINE

COMPUTE FEE FilingFee RETURN USD
  YIELD 100<USD>
ENDCOMPUTE
```

**Notes:**
- Version declaration is optional but recommended for tracking fee schedule changes
- Only one VERSION directive is allowed per script
- Version directive should appear at the beginning of the script
- Effective date uses yyyy-MM-dd format (e.g., 2024-01-15)

---

## Input Definitions

### LIST Input

A single-selection dropdown list.

**Syntax:**
```
DEFINE LIST <Name> AS '<Display Text>'
CHOICE <Symbol1> AS '<Label1>'
CHOICE <Symbol2> AS '<Label2>'
...
DEFAULT <DefaultSymbol>
GROUP <GroupName>
ENDDEFINE
```

**Example:**
```
DEFINE LIST EntityType AS 'Entity type'
CHOICE NormalEntity AS 'Normal entity'
CHOICE SmallEntity AS 'Small entity'
CHOICE MicroEntity AS 'Micro entity'
DEFAULT NormalEntity
GROUP General
ENDDEFINE
```

**Usage in expressions:**
```
YIELD 100 IF EntityType EQ NormalEntity
YIELD 50 IF EntityType EQ SmallEntity
```

---

### MULTILIST Input

A multiple-selection list where users can choose several options.

**Syntax:**
```
DEFINE MULTILIST <Name> AS '<Display Text>'
CHOICE <Symbol1> AS '<Label1>'
CHOICE <Symbol2> AS '<Label2>'
...
DEFAULT <Symbol1>,<Symbol2>
GROUP <GroupName>
ENDDEFINE
```

**Example:**
```
DEFINE MULTILIST DesignationCountries AS 'Designation countries'
CHOICE DE AS 'Germany'
CHOICE FR AS 'France'
CHOICE GB AS 'United Kingdom'
CHOICE IT AS 'Italy'
DEFAULT DE,FR
GROUP Designations
ENDDEFINE
```

**Special properties:**
- `<Name>!COUNT` - Returns the number of selected items

**Usage:**
```
YIELD 100 * DesignationCountries!COUNT
```

---

### NUMBER Input

An integer input with optional bounds.

**Syntax:**
```
DEFINE NUMBER <Name> AS '<Display Text>'
BETWEEN <Min> AND <Max>
DEFAULT <DefaultValue>
GROUP <GroupName>
ENDDEFINE
```

**Example:**
```
DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 1 AND 1000
DEFAULT 10
GROUP Claims
ENDDEFINE

DEFINE NUMBER PageCount AS 'Number of pages'
BETWEEN 1 AND 500
DEFAULT 30
GROUP Application
ENDDEFINE
```

**Usage:**
```
# Excess claims fee (claims over 10)
LET ExcessClaims AS ClaimCount - 10
YIELD 50 * ExcessClaims IF ExcessClaims GT 0
```

---

### BOOLEAN Input

A true/false checkbox input.

**Syntax:**
```
DEFINE BOOLEAN <Name> AS '<Display Text>'
DEFAULT TRUE|FALSE
GROUP <GroupName>
ENDDEFINE
```

**Example:**
```
DEFINE BOOLEAN RequestsExamination AS 'Requests examination'
DEFAULT TRUE
GROUP Examination
ENDDEFINE

DEFINE BOOLEAN ClaimsSequenceListing AS 'Contains sequence listing'
DEFAULT FALSE
GROUP Claims
ENDDEFINE
```

**Usage:**
```
YIELD 500 IF RequestsExamination EQ TRUE
```

---

### DATE Input

A date input with optional bounds. Dates use `dd.MM.yyyy` format.

**Syntax:**
```
DEFINE DATE <Name> AS '<Display Text>'
BETWEEN <MinDate> AND <MaxDate>
DEFAULT <DefaultDate>
GROUP <GroupName>
ENDDEFINE
```

Special date values:
- `TODAY` - Current date

**Example:**
```
DEFINE DATE FilingDate AS 'Filing date'
BETWEEN 01.01.2000 AND TODAY
DEFAULT TODAY
GROUP Application
ENDDEFINE

DEFINE DATE PriorityDate AS 'Priority date'
BETWEEN 01.01.1990 AND TODAY
DEFAULT 01.01.2024
GROUP Priority
ENDDEFINE
```

**Special properties:**
- `<Name>!YEARSTONOW` - Years from date to today
- `<Name>!MONTHSTONOW` - Months from date to today
- `<Name>!DAYSTONOW` - Days from date to today
- `<Name>!MONTHSTONOW_FROMLASTDAY` - Months from end of date's month to today

**Usage:**
```
# Annuity calculation based on years since filing
LET YearsSinceFiling AS FilingDate!YEARSTONOW
YIELD 100 * FLOOR(YearsSinceFiling)
```

---

### AMOUNT Input

*(Currency-Aware Feature)*

A monetary amount with an associated ISO 4217 currency code.

**Syntax:**
```
DEFINE AMOUNT <Name> AS '<Display Text>'
CURRENCY <ISO4217Code>
DEFAULT <Value>
GROUP <GroupName>
ENDDEFINE
```

**Example:**
```
DEFINE AMOUNT PriorArtSearchFee AS 'Prior art search fee'
CURRENCY EUR
DEFAULT 1500
GROUP Search
ENDDEFINE

DEFINE AMOUNT TranslationCost AS 'Translation cost'
CURRENCY USD
DEFAULT 2000
GROUP Translation
ENDDEFINE
```

**Usage:**
```
# Same-currency arithmetic is type-safe
YIELD PriorArtSearchFee + 500<EUR>

# Mixed currencies cause compile-time type error!
# YIELD PriorArtSearchFee + TranslationCost  # ERROR: EUR + USD
```

---

## Groups

Groups organize inputs in the user interface.

**Syntax:**
```
DEFINE GROUP <Name> AS '<Display Text>' WITH WEIGHT <Weight>
```

Weight determines display order (lower weights appear first).

**Example:**
```
DEFINE GROUP General AS 'General Information' WITH WEIGHT 1
DEFINE GROUP Claims AS 'Claims Information' WITH WEIGHT 2
DEFINE GROUP Fees AS 'Fee Options' WITH WEIGHT 3
```

---

## Fee Computations

### Basic Fees

**Syntax:**
```
COMPUTE FEE <Name>
<Yields and Cases>
ENDCOMPUTE
```

**Example:**
```
COMPUTE FEE FilingFee
YIELD 500
ENDCOMPUTE
```

---

### Optional Fees

Optional fees are computed but reported separately from mandatory fees.

**Syntax:**
```
COMPUTE FEE <Name> OPTIONAL
<Yields and Cases>
ENDCOMPUTE
```

**Example:**
```
COMPUTE FEE ExpeditedProcessingFee OPTIONAL
YIELD 1000
ENDCOMPUTE
```

---

### Cases and Conditions

Use `CASE` blocks for complex conditional logic.

**Syntax:**
```
COMPUTE FEE <Name>
CASE <Condition> AS
  YIELD <Expression> IF <Condition>
  YIELD <Expression>
ENDCASE
CASE <Condition> AS
  YIELD <Expression>
ENDCASE
ENDCOMPUTE
```

**Example:**
```
COMPUTE FEE EntityBasedFee
CASE EntityType EQ NormalEntity AS
  YIELD 1000 IF ClaimCount LTE 20
  YIELD 1000 + (50 * (ClaimCount - 20)) IF ClaimCount GT 20
ENDCASE
CASE EntityType EQ SmallEntity AS
  YIELD 500 IF ClaimCount LTE 20
  YIELD 500 + (25 * (ClaimCount - 20)) IF ClaimCount GT 20
ENDCASE
CASE EntityType EQ MicroEntity AS
  YIELD 250 IF ClaimCount LTE 20
  YIELD 250 + (12 * (ClaimCount - 20)) IF ClaimCount GT 20
ENDCASE
ENDCOMPUTE
```

**Simple conditional yields (without CASE):**
```
COMPUTE FEE SimpleFee
YIELD 100 IF EntityType EQ MicroEntity
YIELD 200 IF EntityType EQ SmallEntity
YIELD 400 IF EntityType EQ NormalEntity
ENDCOMPUTE
```

---

### Local Variables

Use `LET` to define computed variables within a fee.

**Syntax:**
```
LET <VarName> AS <Expression>
```

**Example:**
```
COMPUTE FEE ExcessClaimsFee
LET ExcessClaims AS ClaimCount - 10
LET ExcessPages AS PageCount - 30
YIELD 50 * ExcessClaims IF ExcessClaims GT 0
YIELD 20 * ExcessPages IF ExcessPages GT 0
ENDCOMPUTE
```

---

### Polymorphic Fees

*(Currency-Aware Feature)*

Define fees that work with any currency, specified at instantiation.

**Syntax:**
```
COMPUTE FEE <Name><<TypeVar>> RETURN <TypeVar>
<Yields using TypeVar>
ENDCOMPUTE
```

**Example:**
```
# A fee that returns the same currency type it receives
COMPUTE FEE ScaledFee<C> RETURN C
YIELD 100<C> * Multiplier
ENDCOMPUTE
```

---

## Expressions

### Arithmetic Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `+` | Addition | `ClaimCount + 5` |
| `-` | Subtraction | `ClaimCount - 10` |
| `*` | Multiplication | `50 * ExcessClaims` |
| `/` | Division | `TotalFee / 2` |
| `( )` | Grouping | `(ClaimCount - 10) * 50` |

**Example:**
```
LET ExcessClaims AS ClaimCount - 10
LET BaseFee AS 500
YIELD BaseFee + (ExcessClaims * 50)
```

---

### Comparison Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `EQ` | Equal | `EntityType EQ NormalEntity` |
| `NEQ` | Not equal | `EntityType NEQ MicroEntity` |
| `LT` | Less than | `ClaimCount LT 10` |
| `LTE` | Less than or equal | `ClaimCount LTE 20` |
| `GT` | Greater than | `ClaimCount GT 10` |
| `GTE` | Greater than or equal | `ClaimCount GTE 5` |
| `IN` | Is in list | `Country IN DesignatedCountries` |
| `NIN` | Not in list | `Country NIN ExcludedCountries` |

---

### Logical Operators

| Operator | Description | Example |
|----------|-------------|---------|
| `AND` | Logical AND | `ClaimCount GT 10 AND EntityType EQ NormalEntity` |
| `OR` | Logical OR | `EntityType EQ SmallEntity OR EntityType EQ MicroEntity` |

**Example:**
```
YIELD 1000 IF ClaimCount GT 20 AND EntityType EQ NormalEntity
YIELD 500 IF ClaimCount GT 20 AND (EntityType EQ SmallEntity OR EntityType EQ MicroEntity)
```

---

### Built-in Functions

| Function | Description | Example |
|----------|-------------|---------|
| `ROUND(x)` | Round to nearest integer | `ROUND(Fee / 10) * 10` |
| `FLOOR(x)` | Round down | `FLOOR(YearsSinceFiling)` |
| `CEIL(x)` | Round up | `CEIL(MonthsRemaining / 12)` |

**Example:**
```
LET Years AS FLOOR(FilingDate!YEARSTONOW)
YIELD 100 * Years
```

---

### Currency Literals

*(Currency-Aware Feature)*

Annotate numeric values with ISO 4217 currency codes.

**Syntax:**
```
<Number><<CurrencyCode>>
```

**Examples:**
```
100<EUR>      # 100 Euros
50.50<USD>    # 50.50 US Dollars
1000<GBP>     # 1000 British Pounds
250<JPY>      # 250 Japanese Yen
```

**Type-safe arithmetic:**
```
# Valid: same currency
YIELD 100<EUR> + 50<EUR>           # Result: 150 EUR
YIELD 100<EUR> * 2                 # Result: 200 EUR (scalar multiplication)

# Invalid: mixed currencies (compile-time error)
YIELD 100<EUR> + 50<USD>           # TYPE ERROR: Cannot mix EUR and USD
```

---

### Currency Conversion

*(Currency-Aware Feature)*

Convert between currencies at runtime using exchange rates.

**Syntax:**
```
CONVERT(<Amount>, <SourceCurrency>, <TargetCurrency>)
```

**Example:**
```
# Convert USD amount to EUR
YIELD CONVERT(100<USD>, USD, EUR) + 50<EUR>

# Convert an amount variable
YIELD CONVERT(TranslationCost, USD, EUR) + PriorArtSearchFee
```

---

## Returns

Define named outputs from the calculation.

**Syntax:**
```
RETURN <Symbol> AS '<Display Text>'
```

**Example:**
```
RETURN TotalMandatory AS 'Total mandatory fees'
RETURN TotalOptional AS 'Total optional fees'
RETURN GrandTotal AS 'Grand total'
```

---

## Complete Example

```
# ===========================================
# European Patent Application Fee Calculator
# ===========================================

# Version declaration
VERSION '2024.1' EFFECTIVE 2024-01-15 DESCRIPTION 'Updated EPO fees for 2024' REFERENCE 'EPO Official Journal 2023/12'

# --- Groups ---
DEFINE GROUP General AS 'General Information' WITH WEIGHT 1
DEFINE GROUP Claims AS 'Claims' WITH WEIGHT 2
DEFINE GROUP Pages AS 'Application Pages' WITH WEIGHT 3
DEFINE GROUP Options AS 'Options' WITH WEIGHT 4

# --- Inputs ---

DEFINE LIST EntityType AS 'Applicant type'
CHOICE NormalEntity AS 'Large entity'
CHOICE SmallEntity AS 'SME (small/medium enterprise)'
CHOICE MicroEntity AS 'Micro entity / Natural person'
DEFAULT NormalEntity
GROUP General
ENDDEFINE

DEFINE NUMBER ClaimCount AS 'Total number of claims'
BETWEEN 1 AND 500
DEFAULT 10
GROUP Claims
ENDDEFINE

DEFINE NUMBER PageCount AS 'Number of application pages'
BETWEEN 1 AND 1000
DEFAULT 35
GROUP Pages
ENDDEFINE

DEFINE BOOLEAN RequestsExamination AS 'Request examination'
DEFAULT TRUE
GROUP Options
ENDDEFINE

DEFINE AMOUNT AdditionalServiceFee AS 'Additional service fee'
CURRENCY EUR
DEFAULT 0
GROUP Options
ENDDEFINE

# --- Fee Computations ---

# Filing fee with entity-based reduction
COMPUTE FEE FilingFee
CASE EntityType EQ NormalEntity AS
  YIELD 1500<EUR>
ENDCASE
CASE EntityType EQ SmallEntity AS
  YIELD 750<EUR>
ENDCASE
CASE EntityType EQ MicroEntity AS
  YIELD 375<EUR>
ENDCASE
ENDCOMPUTE

# Excess claims fee (over 15 claims)
COMPUTE FEE ExcessClaimsFee
LET ExcessClaims AS ClaimCount - 15
YIELD 250<EUR> * ExcessClaims IF ExcessClaims GT 0
ENDCOMPUTE

# Excess pages fee (over 35 pages)
COMPUTE FEE ExcessPagesFee
LET ExcessPages AS PageCount - 35
YIELD 17<EUR> * ExcessPages IF ExcessPages GT 0
ENDCOMPUTE

# Examination fee
COMPUTE FEE ExaminationFee
CASE RequestsExamination EQ TRUE AS
  YIELD 1950<EUR> IF EntityType EQ NormalEntity
  YIELD 975<EUR> IF EntityType EQ SmallEntity
  YIELD 488<EUR> IF EntityType EQ MicroEntity
ENDCASE
ENDCOMPUTE

# Additional services
COMPUTE FEE AdditionalServices
YIELD AdditionalServiceFee
ENDCOMPUTE

# --- Returns ---
RETURN EPOFiling AS 'EPO Filing Fees'
RETURN Examination AS 'Examination Fees'
RETURN Total AS 'Total Fees'
```

---

## ISO 4217 Currency Codes (Common)

| Code | Currency |
|------|----------|
| EUR | Euro |
| USD | United States Dollar |
| GBP | British Pound Sterling |
| JPY | Japanese Yen |
| CHF | Swiss Franc |
| CNY | Chinese Yuan |
| CAD | Canadian Dollar |
| AUD | Australian Dollar |
| KRW | South Korean Won |
| INR | Indian Rupee |
| RON | Romanian Leu |
| PLN | Polish Zloty |
| SEK | Swedish Krona |
| DKK | Danish Krone |
| NOK | Norwegian Krone |

IPFLang supports all 161 ISO 4217 currency codes.

---

## Verification Directives

IPFLang includes static analysis to verify fee correctness at compile time.

### VERIFY COMPLETE

Verify that a fee covers all possible input combinations (no gaps).

**Syntax:**
```
VERIFY COMPLETE FEE <FeeName>
```

**Example:**
```
DEFINE LIST EntityType AS 'Entity type'
CHOICE Normal AS 'Normal'
CHOICE Small AS 'Small'
DEFAULT Normal
ENDDEFINE

COMPUTE FEE BasicFee
YIELD 100 IF EntityType EQ Normal
YIELD 50 IF EntityType EQ Small
ENDCOMPUTE

VERIFY COMPLETE FEE BasicFee  # Verifies all EntityType values are covered
```

If the fee is incomplete (e.g., missing a case for one EntityType), the verification will report the gap.

---

### VERIFY MONOTONIC

Verify that a fee is monotonic with respect to a numeric input (e.g., more claims never reduces the fee).

**Syntax:**
```
VERIFY MONOTONIC FEE <FeeName> WITH RESPECT TO <InputName>
VERIFY MONOTONIC FEE <FeeName> WITH RESPECT TO <InputName> DIRECTION <Direction>
```

**Direction options:**
- `NonDecreasing` (default) - Fee never decreases as input increases
- `NonIncreasing` - Fee never increases as input increases
- `StrictlyIncreasing` - Fee always increases as input increases
- `StrictlyDecreasing` - Fee always decreases as input increases

**Example:**
```
DEFINE NUMBER ClaimCount AS 'Number of claims'
BETWEEN 1 AND 100
DEFAULT 10
ENDDEFINE

COMPUTE FEE ClaimFee
LET ExcessClaims AS ClaimCount - 10
YIELD 50 * ExcessClaims IF ExcessClaims GT 0
YIELD 0 IF ExcessClaims LTE 0
ENDCOMPUTE

VERIFY MONOTONIC FEE ClaimFee WITH RESPECT TO ClaimCount
VERIFY MONOTONIC FEE ClaimFee WITH RESPECT TO ClaimCount DIRECTION NonDecreasing
```

If the fee violates monotonicity (e.g., fee decreases when ClaimCount increases), the verification will report the violation with specific values.

---

## Type System Summary

The currency-aware type system prevents common errors:

| Expression | Valid? | Reason |
|------------|--------|--------|
| `100<EUR> + 50<EUR>` | ✓ | Same currency |
| `100<EUR> * 2` | ✓ | Scalar multiplication |
| `100<EUR> / 2` | ✓ | Scalar division |
| `100<EUR> + 50<USD>` | ✗ | Mixed currencies |
| `100<EUR> + 50` | ✓ | Plain number compatible |
| `CONVERT(x, USD, EUR) + 50<EUR>` | ✓ | Explicit conversion |

Type errors are reported at parse time, before any computation runs.
