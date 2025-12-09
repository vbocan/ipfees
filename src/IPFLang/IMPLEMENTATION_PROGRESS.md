# IPFLang Novelty Implementation Progress

This document tracks the implementation of research novelty features for IPFLang, based on the analysis in `article/Novelty Gaps and Opportunities.txt`.

---

## Overview

The goal is to transform IPFLang from a standard DSL into a research contribution with genuine academic novelty. Three primary innovations were selected for implementation:

| # | Innovation | Status | Academic Impact | Practical Value |
|---|------------|--------|-----------------|-----------------|
| 1 | Currency-Aware Type System | ✅ **COMPLETE** | High | High |
| 2 | Completeness Verification | ✅ **COMPLETE** | Very High | High |
| 3 | Provenance Semantics | ✅ **COMPLETE** | High | Very High |

---

## 1. Currency-Aware Type System ✅ COMPLETE

### Problem Solved
IPFLang previously allowed nonsensical operations like adding EUR amounts to USD amounts without conversion. Errors were caught at runtime or not at all.

### Innovation Implemented
A static type system with dimensional analysis for currencies, preventing cross-currency arithmetic errors at compile time.

### Implementation Date
December 2024

### Files Created

| File | Purpose |
|------|---------|
| `Types/Currency.cs` | ISO 4217 currency validation (161 currencies) |
| `Types/IPFType.cs` | Type hierarchy (`IPFTypeAmount`, `IPFTypeVariable`, etc.) |
| `Types/TypeError.cs` | Type error representation |
| `Types/TypeEnvironment.cs` | Typing context (Γ) for static analysis |
| `Types/CurrencyTypeChecker.cs` | **Core novelty**: Static type checker |
| `CurrencyConversion/ICurrencyConverter.cs` | Conversion interface |
| `CurrencyConversion/IExchangeRateFetcher.cs` | Rate fetcher interface |
| `CurrencyConversion/ExchangeRateResponse.cs` | Response model |
| `CurrencyConversion/CurrencyConverter.cs` | EUR-relative conversion |
| `CurrencyConversion/ExchangeRateFetcher.cs` | ECB rate fetcher |
| `Evaluator/NodeCurrencyLiteral.cs` | AST node for currency literals |

### Files Modified

| File | Changes |
|------|---------|
| `Parser/Records.cs` | Added `DslInputAmount`, extended `DslFee` with type parameters |
| `Parser/DslParser.cs` | Parse AMOUNT, currency literals, polymorphic fees, CONVERT |
| `Evaluator/Records.cs` | Added `IPFValueAmount` |
| `Evaluator/Token.cs` | Added `CurrencyLiteral` token |
| `Evaluator/Tokenizer.cs` | Parse `100<EUR>` syntax |
| `Evaluator/NodeVariable.cs` | Exposed `Name` property |
| `Evaluator/NodeFunctionCall.cs` | CONVERT function handling |
| `Evaluator/IContext.cs` | Added `ConvertCurrency` method |
| `Evaluator/EvaluatorContext.cs` | Currency converter integration |
| `Evaluator/ReflectionContext.cs` | Stub implementation |
| `Evaluator/DslEvaluator.cs` | Currency converter parameter |
| `Calculator/IDslCalculator.cs` | Added `GetTypeErrors`, `SetCurrencyConverter` |
| `Calculator/DslCalculator.cs` | Type checker integration |

### New Syntax Implemented

```dsl
# Currency-annotated literals
YIELD 100<EUR> IF EntityType EQ NormalEntity

# Amount input type
DEFINE AMOUNT FilingFee AS 'Filing fee'
CURRENCY EUR
DEFAULT 100
ENDDEFINE

# Explicit conversion
YIELD CONVERT(SearchFee, USD, EUR) + FilingFee

# Polymorphic fees (currency-generic)
COMPUTE FEE ClaimFee<C> RETURN C
  YIELD 50<C> * ExcessClaims
ENDCOMPUTE
```

### Type Rules Implemented

| Expression | Valid | Reason |
|------------|-------|--------|
| `100<EUR> + 50<EUR>` | ✓ | Same currency |
| `100<EUR> * 2` | ✓ | Scalar multiplication |
| `100<EUR> + 50<USD>` | ✗ | **TYPE ERROR**: Mixed currencies |
| `CONVERT(x, USD, EUR) + 50<EUR>` | ✓ | Explicit conversion |

### Test Coverage
- 39 new tests in `CurrencyTypeTests.cs`
- All 116 tests passing (77 original + 39 new)

### Academic Contribution
First application of dimensional type systems to multi-jurisdiction regulatory calculations. The type checker provides:
- Formal soundness: well-typed programs don't have currency errors
- Parametric polymorphism for currency-generic fee definitions
- Static + runtime hybrid: compile-time safety with runtime conversion rates

---

## 2. Completeness Verification ✅ COMPLETE

### Problem Solved
Fee schedules must cover all valid inputs (no gaps) and should exhibit expected monotonicity (more claims → higher fees, never lower). Previously there were no static guarantees for these properties.

### Innovation Implemented
Static analysis algorithms that verify fee completeness and monotonicity at compile time, with DSL syntax for declaring verification requirements.

### Implementation Date
December 2024

### Files Created

| File | Purpose |
|------|---------|
| `Analysis/InputDomain.cs` | Domain representations (BooleanDomain, ListDomain, NumericDomain, AmountDomain, DateDomain, MultiListDomain) |
| `Analysis/DomainValue.cs` | Value types for domain values (BooleanValue, SymbolValue, NumericValue, etc.) |
| `Analysis/DomainAnalyzer.cs` | Extracts domains from inputs, generates combinations |
| `Analysis/LogicalExpression.cs` | AST for logical conditions (And, Or, Not, Comparison) |
| `Analysis/ConditionExtractor.cs` | Parses DSL conditions into logical expressions |
| `Analysis/CompletenessChecker.cs` | Verifies all inputs are covered |
| `Analysis/MonotonicityChecker.cs` | Verifies fee monotonicity |

### Files Modified

| File | Changes |
|------|---------|
| `Parser/Records.cs` | Added `DslVerify`, `DslVerifyComplete`, `DslVerifyMonotonic` |
| `Parser/IDslParser.cs` | Added `GetVerifications()` |
| `Parser/DslParser.cs` | Parse VERIFY COMPLETE and VERIFY MONOTONIC directives |
| `Calculator/IDslCalculator.cs` | Added `VerifyCompleteness()`, `VerifyMonotonicity()`, `RunVerifications()`, `VerificationResults` |
| `Calculator/DslCalculator.cs` | Integration of completeness and monotonicity checkers |
| `Types/CurrencyTypeChecker.cs` | Fixed operator recognition (LTE, GTE, NEQ, IN, NIN) |

### New Syntax Implemented

```dsl
# Verify that a fee covers all input combinations
VERIFY COMPLETE FEE BasicFee

# Verify that a fee is monotonic (never decreases as input increases)
VERIFY MONOTONIC FEE ClaimFee WITH RESPECT TO ClaimCount

# Specify direction (default is NonDecreasing)
VERIFY MONOTONIC FEE ClaimFee WITH RESPECT TO ClaimCount DIRECTION NonDecreasing
VERIFY MONOTONIC FEE DiscountFee WITH RESPECT TO Quantity DIRECTION NonIncreasing
VERIFY MONOTONIC FEE StrictFee WITH RESPECT TO Count DIRECTION StrictlyIncreasing
```

### Verification Algorithm

**Completeness Checking:**
1. Extract input domains from DSL definitions
2. Parse fee conditions into logical expressions
3. For small domains: exhaustive enumeration of all input combinations
4. For large domains: representative sampling at boundaries and intermediate points
5. Report gaps where no condition matches

**Monotonicity Checking:**
1. Generate representative values for the target numeric input
2. Evaluate fee at each point while holding other inputs constant
3. Verify fee values respect the expected direction (non-decreasing, etc.)
4. Report violations with specific input values showing the problem

### Test Coverage
- 25 new tests in `CompletenessTests.cs`
- All 141 tests passing (116 previous + 25 new)

### Academic Contribution
- Novel completeness verification for fee domain coverage
- Monotonicity verification ensuring "more input → more fee" properties
- Efficient algorithms for large input domains using representative sampling
- Clean separation of domain analysis, condition extraction, and verification

---

## 3. Provenance Semantics ✅ COMPLETE

### Problem Solved
Fee calculations lacked formal audit trails. Users could see final amounts but not understand *why* specific fees were charged or *which rules* contributed. Legal disputes and regulatory audits require precise provenance tracking.

### Innovation Implemented
Every computed fee carries formal provenance tracking:
- Which CASE conditions fired
- Which YIELD conditions matched
- Individual contribution amounts from each rule
- LET variable evaluations
- Counterfactual explanations showing alternative outcomes

### Implementation Date
December 2024

### Files Created

| File | Purpose |
|------|---------|
| `Provenance/ProvenanceRecord.cs` | Core data model with `ProvenanceRecord`, `FeeProvenance`, `ComputationProvenance`, `Counterfactual` classes |
| `Provenance/ProvenanceCollector.cs` | Collects provenance during fee evaluation |
| `Provenance/CounterfactualEngine.cs` | Generates what-if scenarios by trying alternative input values |
| `Provenance/ProvenanceExporter.cs` | Export to JSON, text, markdown, and legal citation formats |

### Files Modified

| File | Changes |
|------|---------|
| `Calculator/IDslCalculator.cs` | Added `ComputeWithProvenance()`, `ComputeWithCounterfactuals()` |
| `Calculator/DslCalculator.cs` | Integration of provenance collector and counterfactual engine |
| `Types/CurrencyTypeChecker.cs` | Fixed LET variable binding (prefixed vs unprefixed names) |

### API Usage

```csharp
// Get provenance with computation
var provenance = calculator.ComputeWithProvenance(inputs);

// Access breakdown
foreach (var fee in provenance.FeeProvenances)
{
    Console.WriteLine($"Fee: {fee.FeeName}, Total: {fee.TotalAmount}");
    foreach (var record in fee.ContributingRecords)
    {
        Console.WriteLine($"  + {record.Contribution} from [{record.Expression}]");
        if (record.CaseCondition != null)
            Console.WriteLine($"    CASE: {record.CaseCondition}");
    }
}

// Get counterfactuals too
var withCf = calculator.ComputeWithCounterfactuals(inputs);
foreach (var cf in withCf.Counterfactuals)
{
    Console.WriteLine($"If {cf.InputName} were {cf.AlternativeValue}: {cf.AlternativeTotal} ({cf.Difference:+0.00;-0.00})");
}
```

### Provenance Data Model

```csharp
ProvenanceRecord {
    FeeName: string,           // Which fee this contribution belongs to
    CaseCondition: string?,    // The CASE condition (if any)
    CaseConditionResult: bool, // Whether case condition was true
    YieldCondition: string?,   // The YIELD IF condition (if any)
    YieldConditionResult: bool,// Whether yield condition was true
    Expression: string,        // The evaluated expression
    Contribution: decimal,     // Amount contributed
    DidContribute: bool,       // True if actually added to total
    ReferencedInputs: dict,    // Input values used in evaluation
    LetVariables: dict         // LET variable values
}

FeeProvenance {
    FeeName: string,
    IsOptional: bool,
    Records: List<ProvenanceRecord>,
    LetVariables: Dictionary<string, decimal>,
    TotalAmount: decimal,      // Computed property
    ContributingRecords,       // Records where DidContribute = true
    SkippedRecords            // Records where condition was false
}

ComputationProvenance {
    InputValues: Dictionary<string, object>,
    FeeProvenances: List<FeeProvenance>,
    TotalMandatory: decimal,
    TotalOptional: decimal,
    GrandTotal: decimal,
    Counterfactuals: List<Counterfactual>
}

Counterfactual {
    InputName: string,
    OriginalValue: object,
    AlternativeValue: object,
    OriginalTotal: decimal,
    AlternativeTotal: decimal,
    Difference: decimal
}
```

### Export Formats

| Method | Format | Use Case |
|--------|--------|----------|
| `ToJson()` | JSON | API integration, data exchange |
| `ToText()` | Human-readable | Console output, logging |
| `ToMarkdown()` | Markdown tables | Documentation, reports |
| `ToLegalCitation()` | Legal format | Dispute resolution, formal audits |

### Counterfactual Generation

The counterfactual engine automatically generates what-if scenarios:

| Input Type | Alternative Values Generated |
|------------|------------------------------|
| Boolean | Opposite value |
| List | All other choices |
| Number | Boundary values (min, max, middle) |
| Amount | Doubled and halved values |

### Test Coverage
- 20 new tests in `ProvenanceTests.cs`
- All 161 tests passing (141 previous + 20 new)

### Academic Contribution
- Formal provenance semantics for regulatory fee calculations
- Counterfactual generation algorithm for fee explanations
- Multiple export formats for different stakeholders (technical, legal, documentation)
- Provenance composition across LET variables and nested conditions

---

## Future Innovations (In Progress)

Based on Critical Review analysis, three additional innovations are planned to strengthen academic contribution and address formal verification gaps.

---

## 4. Regulatory Change Semantics 🚧 IN PROGRESS

### Problem to Solve
Fee schedules change frequently (annually or more often). Current implementation has no versioning support, making it impossible to:
- Query historical fee amounts ("what was the fee on date X?")
- Analyze impact of fee schedule changes
- Verify that updates preserve completeness/monotonicity
- Generate audit trails showing when and why fees changed

### Innovation to Implement
First-class versioning semantics with automatic diff generation and impact analysis.

### Academic Contribution
- **Novelty**: First DSL with formal regulatory change semantics and automated impact analysis
- **Addresses Critical Review gaps**: Versioning, formal verification, temporal correctness
- **Publication potential**: Standalone paper on "Version Control for Regulatory DSLs"
- **Real-world validation**: Can analyze actual historical changes from 118 jurisdictions

### Planned Implementation (Phase 1: Core Versioning)

#### Files to Create

| File | Purpose |
|------|---------|
| `Versioning/Version.cs` | Version representation (semantic versioning + effective dates) |
| `Versioning/VersionedScript.cs` | Container for multiple versions of a fee schedule |
| `Versioning/VersionResolver.cs` | Resolve which version applies for a given date |
| `Versioning/ChangeRecord.cs` | Representation of changes between versions |
| `Versioning/DiffEngine.cs` | Compare two versions, generate structured diff |
| `Versioning/ImpactAnalyzer.cs` | Analyze impact of changes on calculations |
| `Versioning/TemporalQuery.cs` | Query historical fee amounts |

#### Files to Modify

| File | Changes |
|------|---------|
| `Parser/Records.cs` | Add `DslVersion`, extend `DslScript` with version metadata |
| `Parser/DslParser.cs` | Parse VERSION directive |
| `Calculator/IDslCalculator.cs` | Add `ComputeAtDate()`, `AnalyzeChange()`, `GetVersionHistory()` |
| `Calculator/DslCalculator.cs` | Version resolver integration |

#### New Syntax to Implement

```dsl
# Version declaration
VERSION '2024.1' EFFECTIVE 2024-01-15
DESCRIPTION 'USPTO fee increase per Federal Register Vol. 89, No. 123'

# Reference historical versions
COMPUTE FEE FilingFee RETURN USD
  CASE EffectiveDate BEFORE 2024-01-15
    YIELD 100<USD>  # Old rate (version 2023.1)
  DEFAULT
    YIELD 150<USD>  # New rate (version 2024.1)
  ENDCASE
ENDCOMPUTE

# Change analysis directive
ANALYZE CHANGE FROM VERSION '2023.1' TO VERSION '2024.1'
VERIFY PRESERVES COMPLETENESS
VERIFY PRESERVES MONOTONICITY
```

#### API Design

```csharp
// Query historical fee
var fee = calculator.ComputeAtDate(inputs, date: new DateTime(2023, 6, 1));

// Analyze change impact
var impact = calculator.AnalyzeChange("2023.1", "2024.1");
// Returns: "32 of 355 fees changed, affecting calculations with ClaimCount > 20"

// Get diff
var diff = calculator.GetDiff("2023.1", "2024.1");
foreach (var change in diff.Changes)
{
    Console.WriteLine($"{change.FeeName}: {change.OldValue} -> {change.NewValue}");
    Console.WriteLine($"  Breaking: {change.IsBreaking}");
    Console.WriteLine($"  Affects: {change.AffectedInputCombinations.Count} scenarios");
}
```

#### Implementation Phases

**Phase 1: Core Versioning (Weeks 1-2)**
- [ ] Version data model and parser
- [ ] VersionedScript container
- [ ] VersionResolver for date-based resolution
- [ ] Tests: 15 tests covering version parsing, resolution

**Phase 2: Diff Engine (Weeks 3-4)**
- [ ] DiffEngine comparing two versions
- [ ] ChangeRecord with breaking/non-breaking classification
- [ ] ImpactAnalyzer showing affected input combinations
- [ ] Tests: 20 tests covering diff generation, impact analysis

**Phase 3: Temporal Queries (Weeks 5-6)**
- [ ] TemporalQuery for historical calculations
- [ ] Integration with existing provenance system
- [ ] Verification that changes preserve completeness/monotonicity
- [ ] Tests: 15 tests covering temporal queries, verification

**Phase 4: Real-World Validation (Week 7)**
- [ ] Load actual 2023 vs 2024 USPTO fee schedules
- [ ] Load actual 2023 vs 2024 EPO fee schedules
- [ ] Generate change reports
- [ ] Measure: number of breaking changes, affected scenarios
- [ ] Document findings for paper

#### Success Metrics
- Successfully parse and manage 5+ versions per jurisdiction
- Accurately identify breaking vs. non-breaking changes
- Generate change reports validated against official notices
- Query historical fees with 100% accuracy vs. official archives

### Academic Paper Outline
- **Title**: "Version Control and Change Impact Analysis for Regulatory Domain-Specific Languages"
- **Contribution 1**: Formal semantics for regulatory versioning
- **Contribution 2**: Automated diff algorithm with breaking change detection
- **Contribution 3**: Impact analysis framework
- **Evaluation**: Analysis of real fee changes across 118 jurisdictions over 2023-2024

---

## 5. Regulatory Temporal Logic (RTL) 📋 PLANNED

### Problem to Solve
IP fee calculations often depend on deadlines and time-sensitive events:
- Late filing fees based on days past deadline
- Deadline calculations vary by jurisdiction (business days vs. calendar days)
- Holiday calendars differ by jurisdiction
- No static verification that deadlines can be met

### Innovation to Implement
Domain-specific temporal logic with jurisdiction-aware calendar semantics and type-level temporal constraints.

### Academic Contribution
- **Novelty**: First temporal logic designed for regulatory compliance
- **Formal foundations**: Adds mathematical framework requested by reviewers
- **Practical value**: Automated deadline calculation and verification

### Planned Syntax

```dsl
# Deadline input type
DEFINE DEADLINE FilingDeadline AS 'Patent filing deadline'
JURISDICTION USPTO
BUSINESS_DAYS_ONLY
EXCLUDE_HOLIDAYS
DEFAULT TODAY + 6 MONTHS
ENDDEFINE

# Late fee calculation
COMPUTE FEE LateFee RETURN USD
  LET DaysLate = BUSINESS_DAYS_BETWEEN(FilingDeadline, ActualFilingDate)
  YIELD 0<USD> IF DaysLate <= 0
  YIELD 50<USD> * DaysLate IF DaysLate BETWEEN 1 AND 30
  YIELD REJECT "Filing deadline exceeded" IF DaysLate > 30
ENDCOMPUTE

# Temporal verification
VERIFY DEADLINE FilingDeadline IS REACHABLE FROM TODAY
```

### Implementation Phases
**Phase 1**: Temporal type system (DEADLINE type, temporal operators)
**Phase 2**: Jurisdiction calendar data (business days, holidays)
**Phase 3**: Temporal verification (deadline reachability)
**Phase 4**: Integration with existing type checker

### Success Metrics
- Support 10+ jurisdictions with different calendar rules
- Accurately calculate business days vs. calendar days
- Verify temporal constraints statically

**Status**: Awaiting completion of Regulatory Change Semantics
**Estimated Start**: Week 8

---

## 6. Jurisdiction Composition Calculus 📋 PLANNED

### Problem to Solve
Many jurisdictions share fee structures with minor variations (e.g., EPO vs. national phases). Current implementation duplicates fee definitions across 118 jurisdictions.

### Innovation to Implement
Formal composition calculus for jurisdiction inheritance with override semantics.

### Academic Contribution
- **Code reuse**: Reduce 355 fee definitions through inheritance
- **Formal semantics**: Composition and override rules
- **Verification**: Cross-jurisdiction constraint checking

### Planned Syntax

```dsl
# Base jurisdiction
JURISDICTION EPO {
  DEFINE NUMBER ClaimCount AS 'Number of claims'
  BETWEEN 0 AND 100
  DEFAULT 1
  ENDDEFINE
  
  COMPUTE FEE ClaimFee RETURN EUR
    YIELD 15<EUR> * MAX(0, ClaimCount - 10)
  ENDCOMPUTE
}

# Derived jurisdiction with override
JURISDICTION EPO_DE INHERITS EPO {
  OVERRIDE FEE ClaimFee RETURN EUR
    YIELD 50<EUR> * MAX(0, ClaimCount - 10)  # Different rate
  ENDOVERRIDE
}
```

### Implementation Phases
**Phase 1**: Parser support for JURISDICTION blocks and INHERITS
**Phase 2**: Inheritance resolution algorithm
**Phase 3**: Override semantics and validation
**Phase 4**: Verification that overrides preserve type safety

### Success Metrics
- Define base jurisdictions (EPO, PCT, Madrid)
- Reduce code duplication by 40%+
- Verify override correctness

**Status**: Awaiting completion of RTL
**Estimated Start**: Week 14

---

## Implementation Timeline

| Week | Phase | Deliverable |
|------|-------|-------------|
| 1-2  | Change Semantics Phase 1 | Version parsing and resolution |
| 3-4  | Change Semantics Phase 2 | Diff engine and impact analysis |
| 5-6  | Change Semantics Phase 3 | Temporal queries and verification |
| 7    | Change Semantics Phase 4 | Real-world validation with USPTO/EPO |
| 8-10 | RTL Phase 1-2 | Temporal types and calendar data |
| 11-13| RTL Phase 3-4 | Verification and integration |
| 14-16| Composition Phase 1-3 | Inheritance and override semantics |
| 17   | Integration testing | All three innovations working together |

**Current Status**: Starting Week 1 - Regulatory Change Semantics Phase 1
**Next Milestone**: Version parsing complete by end of Week 2

---

## Development Environment

### Prerequisites
- .NET 10.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Build
```bash
cd src/IPFLang
dotnet build
```

### Test
```bash
cd src/IPFLang
dotnet test
```

### Current Test Status
```
Passed: 161 | Failed: 0 | Skipped: 0
- Original tests: 77
- Currency type tests: 39
- Completeness tests: 25
- Provenance tests: 20
```

---

## References

### Academic Background
- **Dimensional Type Systems**: F# units of measure, Ur/Web
- **SMT-based Verification**: Z3, CVC5
- **Provenance**: Database provenance, W3C PROV model
- **Regulatory DSLs**: Catala, LegalRuleML

### Related Files
- `article/Novelty Gaps and Opportunities.txt` - Original analysis
- `SYNTAX.md` - Complete DSL syntax reference
- `README.md` - Project overview

---

## Resuming Work

All three primary innovations are now complete! To extend with future innovations:

1. **Run existing tests** after any changes to ensure backward compatibility:
   ```bash
   dotnet test src/IPFLang/IPFLang.sln
   ```

2. **Follow the pattern** established in the completed implementations:
   - Create interfaces first
   - Implement core logic
   - Extend parser if new syntax needed
   - Integrate with calculator
   - Write comprehensive tests

3. **Consider future innovations** from the "Future Innovations" section above:
   - Regulatory Temporal Logic (RTL)
   - Jurisdiction Composition Calculus
   - Regulatory Change Semantics

### Completed Implementations

| Innovation | Completed | Tests |
|------------|-----------|-------|
| Currency-Aware Type System | December 2024 | 39 tests |
| Completeness Verification | December 2024 | 25 tests |
| Provenance Semantics | December 2024 | 20 tests |
