# IPFLang Novelty Implementation Progress

This document tracks the implementation of research novelty features for IPFLang, based on the analysis in `article/Novelty Gaps and Opportunities.txt`.

---

## Overview

The goal is to transform IPFLang from a standard DSL into a research contribution with genuine academic novelty. Three primary innovations were selected for implementation:

| # | Innovation | Status | Academic Impact | Practical Value |
|---|------------|--------|-----------------|-----------------|
| 1 | Currency-Aware Type System | ✅ **COMPLETE** | High | High |
| 2 | Completeness Verification | 🔲 Not Started | Very High | High |
| 3 | Provenance Semantics | 🔲 Not Started | High | Very High |

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

## 2. Completeness Verification 🔲 NOT STARTED

### Problem to Solve
Fee schedules must cover all valid inputs (no gaps) and should exhibit expected monotonicity (more claims → higher fees, never lower). Currently no static guarantees.

### Proposed Innovation
Static analysis algorithms that verify:

```dsl
# Completeness: Prove all input combinations have defined outputs
VERIFY COMPLETE FeeSchedule
  OVER EntityType, ClaimCount IN [1..200], PageCount IN [1..500]
  # Analyzer proves: no input combination yields undefined result

# Monotonicity: Prove larger inputs never decrease fees
VERIFY MONOTONIC ClaimFee
  WITH RESPECT TO ClaimCount
  # Analyzer proves: ClaimCount↑ ⟹ ClaimFee↑ (or unchanged)
```

### Implementation Plan

#### Phase 1: Input Domain Analysis
- Extract input bounds from DSL definitions
- Build Cartesian product of valid input combinations
- Represent as constraint system

**New files to create:**
```
Types/InputDomain.cs           - Input domain representation
Types/DomainConstraint.cs      - Constraint representation
Analysis/DomainAnalyzer.cs     - Extract domains from inputs
```

#### Phase 2: Condition Coverage Analysis
- Parse CASE/IF conditions into logical formulas
- Check if conditions cover entire input domain
- Identify gaps (input combinations with no matching condition)

**New files to create:**
```
Analysis/ConditionExtractor.cs    - Extract conditions as formulas
Analysis/CoverageChecker.cs       - Check domain coverage
Analysis/GapReport.cs             - Report uncovered inputs
```

#### Phase 3: SMT Integration (Optional but recommended)
- Encode completeness as SMT formula
- Use Z3 or similar solver
- Provide counterexamples for incomplete specs

**New files to create:**
```
Analysis/SmtEncoder.cs            - Encode to SMT-LIB format
Analysis/Z3Integration.cs         - Z3 solver binding
```

#### Phase 4: Monotonicity Verification
- Extract fee expressions
- Compute symbolic derivatives with respect to inputs
- Verify non-negativity of derivatives

**New files to create:**
```
Analysis/MonotonicityChecker.cs   - Check fee monotonicity
Analysis/SymbolicDiff.cs          - Symbolic differentiation
```

#### Phase 5: DSL Syntax Extensions
- Add VERIFY COMPLETE syntax
- Add VERIFY MONOTONIC syntax
- Integration with calculator

**Files to modify:**
```
Parser/Records.cs                 - Add DslVerify records
Parser/DslParser.cs               - Parse VERIFY statements
Calculator/DslCalculator.cs       - Run verification
```

### Academic Contribution (Expected)
- Novel verification conditions for regulatory fee completeness
- Decidable fragment for common fee patterns
- Integration with SMT solvers for counterexample generation

---

## 3. Provenance Semantics 🔲 NOT STARTED

### Problem to Solve
Current audit trails show *what* was calculated but not *why* in a formally structured way. Legal disputes require understanding exactly which rules contributed to each fee component.

### Proposed Innovation
Semantics where every computed value carries formal provenance:

```csharp
FeeResult {
  amount: 1250 EUR,
  provenance: [
    (FilingFee, Rule 2.3, "ClaimCount > 20", contributed: 500 EUR),
    (SearchFee, Rule 4.1, "EntityType = Small", contributed: 750 EUR)
  ],
  counterfactuals: [
    "If EntityType were Large: total would be 2500 EUR (+1250)",
    "If ClaimCount were 20: total would be 750 EUR (-500)"
  ]
}
```

### Implementation Plan

#### Phase 1: Provenance Data Model
- Define provenance record structure
- Track rule references, conditions matched, contributions

**New files to create:**
```
Provenance/ProvenanceRecord.cs      - Single provenance entry
Provenance/ProvenanceTrace.cs       - Full provenance for a result
Provenance/RuleReference.cs         - Reference to DSL rule
```

#### Phase 2: Instrumented Evaluation
- Extend evaluator to track which rules fired
- Record condition evaluation results
- Accumulate contributions per fee

**Files to modify:**
```
Evaluator/DslEvaluator.cs           - Add provenance tracking
Calculator/DslCalculator.cs         - Return provenance with results
```

**New files to create:**
```
Evaluator/ProvenanceContext.cs      - Context with provenance tracking
```

#### Phase 3: Provenance Composition
- Define algebra for composing provenance
- Handle LET variable provenance
- Propagate through arithmetic operations

**New files to create:**
```
Provenance/ProvenanceAlgebra.cs     - Composition operations
Provenance/ProvenanceComposer.cs    - Compose during evaluation
```

#### Phase 4: Counterfactual Generation
- Identify key decision points
- Re-evaluate with modified inputs
- Generate human-readable explanations

**New files to create:**
```
Provenance/CounterfactualEngine.cs  - Generate counterfactuals
Provenance/ExplanationGenerator.cs  - Natural language output
```

#### Phase 5: Output Formats
- JSON export for integration
- Human-readable report format
- Legal citation format

**New files to create:**
```
Provenance/ProvenanceExporter.cs    - Export formats
Provenance/LegalCitationFormat.cs   - Legal-style citations
```

### Academic Contribution (Expected)
- Formal provenance semantics for regulatory calculations
- Counterfactual generation algorithm for fee explanations
- Provenance composition laws (associativity, etc.)

---

## Future Innovations (Lower Priority)

These were identified but not prioritized for initial implementation:

### 4. Regulatory Temporal Logic (RTL)
- Domain-specific temporal logic for regulatory deadlines
- Jurisdiction-aware calendar semantics
- Type-level temporal constraints

### 5. Jurisdiction Composition Calculus
- Formal calculus for jurisdiction inheritance
- Override and composition semantics
- Cross-jurisdiction constraint verification

### 6. Regulatory Change Semantics
- First-class versioning in the language
- Automatic diff generation
- Impact analysis framework

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
Passed: 116 | Failed: 0 | Skipped: 0
- Original tests: 77
- Currency type tests: 39
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

To continue implementation:

1. **For Completeness Verification**: Start with Phase 1 (Input Domain Analysis). Create `Analysis/` folder and implement `DomainAnalyzer.cs`.

2. **For Provenance Semantics**: Start with Phase 1 (Provenance Data Model). Create `Provenance/` folder and implement core records.

3. **Run existing tests** after any changes to ensure backward compatibility:
   ```bash
   dotnet test src/IPFLang/IPFLang.sln
   ```

4. **Follow the pattern** established in Currency Type System:
   - Create interfaces first
   - Implement core logic
   - Extend parser if new syntax needed
   - Integrate with calculator
   - Write comprehensive tests
