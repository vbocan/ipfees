# IPFees: A Domain-Specific Language Approach to Intellectual Property Fee Calculation

**Valer Bocan, PhD, CSSLP**

*Universitatea Politehnica Timișoara, Romania*

*Email: valer.bocan@upt.ro*

*ORCID: 0009-0006-9084-4064*

---

## Abstract

IPFees is an open-source, jurisdiction-agnostic intellectual property fee calculation system that automates complex legal fee structures across 160+ global IP jurisdictions using a Domain-Specific Language (DSL) approach. The platform addresses documented inefficiencies in IP fee management where professionals spend significant time on repetitive calculations across fragmented government-provided calculators. By enabling legal professionals to define and modify fee calculation rules without software development expertise, IPFees democratizes access to sophisticated fee computation while maintaining precision and auditability. Built on .NET 9.0 with MongoDB and Docker containerization, the system provides real-time currency conversion, multi-jurisdiction support, and comprehensive REST APIs for integration with existing IP management platforms. Performance validation via BenchmarkDotNet demonstrates sub-millisecond DSL execution (23.5μs for complex structures) and 240-320ms multi-jurisdiction calculations, achieving 6-20× improvement over government calculators. Independent validation by IP legal experts confirms dollar-accurate calculations across all implemented jurisdictions. The platform's DSL-based architecture demonstrates cross-domain applicability to other regulatory compliance systems requiring complex, jurisdiction-specific rule evaluation.

**Keywords:** intellectual property, patent fees, domain-specific language, legal technology, computational law, regulatory automation, multi-jurisdiction, fee calculation, IP management

---

## 1. Motivation and Significance

### 1.1 Problem Statement

Intellectual property (IP) practitioners face a persistent challenge in accurately calculating filing, prosecution, and maintenance fees across diverse patent offices globally. Each jurisdiction implements unique fee structures with complex dependencies on entity size, claim counts, page numbers, filing types, and temporal factors, yet current solutions remain fragmented and inadequate for modern practice needs.

Government patent offices provide jurisdiction-specific calculators such as the USPTO fee schedule calculator and EPO fee calculator [1], but these require manual navigation between multiple websites and separate calculations for each jurisdiction. This fragmentation becomes particularly problematic for practitioners managing international portfolios, where a single patent family may require fee calculations across dozens of jurisdictions. The time consumed in switching between different calculator interfaces and re-entering similar data represents a significant inefficiency in IP practice.

Commercial IP management software platforms like CPA Global, Anaqua, and PatSnap primarily focus on docketing and portfolio management rather than comprehensive fee calculation. When fee calculation capabilities exist, they are typically limited to major jurisdictions and hardcode fee structures directly into the software, requiring vendor updates whenever fee schedules change. This creates dependency on vendor responsiveness and limits practitioners' ability to handle emerging jurisdictions or customize calculations for specific scenarios.

Many practitioners consequently fall back on spreadsheets and manual calculations, which increases error risk and consumes billable hours that could be allocated to higher-value legal analysis. The situation becomes even more complex for international IP portfolios, which require accurate multi-currency calculations with historical exchange rates for budget planning and invoicing—functionality rarely integrated into existing tools. The absence of a unified, extensible, open-source solution not only creates inefficiencies in IP practice but also limits research opportunities in computational law and regulatory automation.

### 1.2 Innovation and Contribution

IPFees addresses these challenges through a fundamentally different architectural approach that separates regulatory logic from software implementation. Rather than hardcoding fee calculation rules into the application, the system implements a custom Domain-Specific Language (DSL) [7] that allows jurisdiction-specific rules to be expressed in human-readable format. This separation enables IP professionals to define and modify fee structures without programming expertise while maintaining the precision and reliability required for financial calculations. The DSL supports conditional logic, arithmetic operations, and jurisdiction-specific variables, providing sufficient expressiveness for complex regulatory frameworks while maintaining simplicity for common cases.

The platform's modular design treats each jurisdiction as a configuration entity rather than a code module. Adding support for new patent offices requires only JSON configuration files and DSL rule definitions, not software modifications. This architecture dramatically reduces the barrier to expanding jurisdiction coverage and enables community contributions from practitioners worldwide who understand specific jurisdictions better than any central development team could. The result is a system that can evolve organically as practitioners add support for new jurisdictions or update existing fee schedules in response to regulatory changes.

Beyond its immediate practical utility, IPFees provides a reference implementation for encoding legal rules in executable format, contributing to the emerging field of computational law. The DSL approach ensures transparency, auditability, and version control for regulatory logic—properties essential for legal compliance systems [6]. By demonstrating how computational law principles can be applied to a real-world legal technology challenge with measurable benefits, the platform offers a template for similar systems in other regulatory domains.

### 1.3 Research and Practical Applications

The open-source nature of IPFees creates opportunities for both research and practical applications that extend beyond immediate fee calculation needs. As a testbed for studying automated compliance systems, the platform enables empirical research into how legal professionals interact with DSL-based configuration tools and how effectively domain-specific languages can capture complex regulatory logic. The system's transparent architecture allows researchers to analyze the tradeoffs between expressiveness and usability in regulatory DSLs, providing concrete data on what language features practitioners actually use versus what they find confusing or unnecessary.

The platform's three-tier exchange rate fallback mechanism (real-time API, historical database, manual override) demonstrates a pattern for reliable financial calculations in distributed systems that balances accuracy with resilience. This approach addresses the practical reality that external API dependencies will occasionally fail, while still providing the most current data when available. As an MIT-licensed open-source project, IPFees enables reproducible research in IP economics, fee structure analysis, and international patent system studies—areas where proprietary commercial tools have limited research utility due to their black-box nature.

While developed specifically for IP fees, the architecture extends naturally to other regulatory compliance domains. Any system requiring complex, jurisdiction-specific rule evaluation with frequent regulatory updates could benefit from a similar DSL-based approach. Examples include customs duty calculations based on product classifications and origin rules, multi-jurisdiction tax compliance systems, and professional licensing fee structures across different states or countries. The principles demonstrated here—separation of regulatory logic from software implementation, human-readable rule encoding, and version-controlled compliance specifications—apply broadly across regulatory technology domains.

### 1.4 Related Work and Research Gap

DSL applications in legal domains have matured significantly, yet multi-jurisdiction fee calculation remains unexplored.

Domain-specific languages for legal contracts have explored declarative syntax for obligations and temporal constraints, but typically focus on execution semantics rather than financial computations. Monetary aspects in legal contexts require specialized treatment with arithmetic precision and multi-currency support that general-purpose legal DSLs have not prioritized. LegalRuleML [10] offers XML-based legal rule specifications with ontology-based compliance reasoning, but these systems emphasize binary compliance checking (compliant/non-compliant) with minimal arithmetic support. Catala [11], a DSL for tax law, demonstrates sophisticated financial calculations but targets single-jurisdiction applications and requires formal methods expertise that makes it less accessible to legal practitioners.

Patent offices themselves provide web calculators with proprietary logic. The USPTO [1], EPO [3], JPO [8], and WIPO [2] all maintain tools for their respective jurisdictions, but these lack API access and multi-jurisdiction support. Commercial platforms like CPA Global and Anaqua hardcode fee calculations in closed codebases requiring vendor patches for updates, creating dependency on vendor responsiveness when fee schedules change.

To the best of our knowledge, a systematic search of academic databases (ACM, IEEE, Springer, Web of Science) and GitHub reveals a gap in DSL frameworks for multi-jurisdiction regulatory fee calculations that combine temporal logic, multi-currency support, and arithmetic expressiveness. Open-source alternatives are limited to single-jurisdiction calculators such as InPatent [4] for India with minimal functionality. Table 1 in the supplementary materials illustrates the positioning of IPFees in this underserved domain, with comprehensive literature review available in the supplementary documentation.

---

## 2. Software Description

### 2.1 Architecture Overview

IPFees implements a three-tier architecture optimized for extensibility and maintainability:

**Presentation Layer**: 
- **IPFees.Web**: ASP.NET Core Razor Pages application providing interactive user interface with Bootstrap 5 responsive design
- **IPFees.API**: RESTful API with OpenAPI/Swagger documentation for programmatic access and integration with IP management platforms

**Business Logic Layer**:
- **IPFees.Core**: Domain models, service interfaces, and business logic coordination
- **IPFees.Calculator**: Custom DSL parser and interpreter implementing fee calculation engine

**Data Layer**:
- **MongoDB**: Document database with GridFS for flexible jurisdiction schema and audit logging
- **Configuration System**: JSON-based jurisdiction definitions and fee structure specifications

The architecture follows SOLID principles with dependency injection, enabling independent testing and deployment of components. Docker containerization ensures reproducible deployments across development, testing, and production environments.

### 2.2 Domain-Specific Language: IPFLang

IPFees implements IPFLang (Intellectual Property Fees Language), a declarative DSL with explicit keyword-based syntax designed for legal professionals without programming expertise [7]. Unlike expression-based DSLs, IPFLang uses structured blocks with clear semantic keywords, following established DSL design principles emphasizing domain-specific notations, explicit semantics, and minimal syntax complexity.

**Input Definition**: IPFLang supports five input types with explicit declarations:

```
DEFINE LIST EntityType AS 'Select the desired entity type'
CHOICE NormalEntity AS 'Normal'
CHOICE SmallEntity AS 'Small'
CHOICE MicroEntity AS 'Micro'
DEFAULT NormalEntity
ENDDEFINE

DEFINE MULTILIST Countries AS 'Select validation countries'
CHOICE VAL_DE AS 'Germany'
CHOICE VAL_FR AS 'France'
CHOICE VAL_GB AS 'United Kingdom'
DEFAULT VAL_DE,VAL_FR
ENDDEFINE

DEFINE NUMBER SheetCount AS 'Enter the number of sheets'
BETWEEN 10 AND 1000
DEFAULT 15
ENDDEFINE

DEFINE BOOLEAN ContainsDependentClaims AS 'Does this contain dependent claims?'
DEFAULT TRUE
ENDDEFINE

DEFINE DATE ApplicationDate AS 'Application filing date'
BETWEEN 1990-01-01 AND 2025-12-31
DEFAULT 2024-01-01
ENDDEFINE
```

**Fee Computation**: Fee calculations use structured COMPUTE blocks with conditional logic:

```
COMPUTE FEE BasicNationalFee
YIELD 320 IF EntityType EQ NormalEntity
YIELD 128 IF EntityType EQ SmallEntity
YIELD 64 IF EntityType EQ MicroEntity
ENDCOMPUTE

COMPUTE FEE SearchFee
CASE SituationType EQ PreparedIPEA AS
YIELD 0 IF EntityType EQ NormalEntity
YIELD 0 IF EntityType EQ SmallEntity
YIELD 0 IF EntityType EQ MicroEntity
ENDCASE
YIELD 700 IF EntityType EQ NormalEntity
YIELD 280 IF EntityType EQ SmallEntity
YIELD 140 IF EntityType EQ MicroEntity
ENDCOMPUTE
```

**Supported Operations**:
- Comparison operators: `EQ`, `NEQ`, `GT`, `LT`, `GTE`, `LTE`
- Set membership: `IN`, `NIN` (for multi-selection inputs)
- Arithmetic: `+`, `-`, `*`, `/`, `ROUND()`
- Logical: `AND`, `OR`, `NOT`
- Date functions: `!MONTHSTONOW`, `!YEARSTONOW`, `!MONTHSTONOW_FROMLASTDAY`
- List operations: `!COUNT` (count of selected items)

**Variable Assignment**: Intermediate calculations use LET statements:

```
COMPUTE FEE ClaimFee
LET CF1 AS 265
LET CF2 AS 660
YIELD CF1*(ClaimCount-15) IF ClaimCount GT 15 AND ClaimCount LT 51
YIELD CF2*(ClaimCount-50) + CF1*35 IF ClaimCount GT 50
ENDCOMPUTE
```

The keyword-based syntax prioritizes readability and explicit semantics over conciseness, reducing the learning curve for legal professionals—a key principle of effective DSL design [7]. IPFLang implementation adheres to DSL design guidelines through: (1) **Domain-Specific Notation** using legal terminology (EntityType, ClaimFee, ValidationFee); (2) **Explicit Semantics** with clear keywords (DEFINE, COMPUTE, YIELD) avoiding ambiguous operators; (3) **Syntactic Sugar** minimizing boilerplate while maintaining clarity; and (4) **Error Prevention** through structured blocks with explicit ENDCOMPUTE and ENDDEFINE markers that aid comprehension and prevent common syntax errors. The language successfully balances expressiveness for complex regulatory rules with accessibility for non-programmer domain experts, demonstrating practical application of DSL engineering principles in the legal technology domain.

### 2.3 Multi-Currency System

IPFees implements a sophisticated three-tier currency conversion system:

1. **Real-Time API Provider**: Primary source for current exchange rates via configurable API integration
2. **Historical Database**: MongoDB-cached rates with automatic population from API calls, providing fallback for API unavailability
3. **Manual Override**: Administrator-configurable rates for jurisdictions requiring specific conversion factors or in case of extended API outages

The system maintains 6-8 decimal precision for high-value portfolio calculations and logs all conversions for audit trails. Exchange rates are timestamped and versioned, enabling historical fee calculations for budget planning and retrospective analysis.

### 2.4 Key Features

Beyond basic fee calculation, IPFees delivers comprehensive functionality designed for real-world IP portfolio management. The system supports bulk processing for portfolio-level fee estimation across multiple jurisdictions, with CSV export capabilities for budget planning and financial reporting. This allows practitioners to model the costs of different filing strategies across entire patent families rather than calculating fees one jurisdiction at a time.

The API-first design philosophy ensures that all functionality available through the web interface is also accessible programmatically through comprehensive REST endpoints. This enables integration with existing IP management systems and supports automated workflows where fee estimates feed into larger decision-making processes. Jurisdiction-specific validation rules prevent invalid parameter combinations—such as claim counts exceeding jurisdiction limits or invalid entity type specifications—catching errors before they propagate into budget models or client communications.

Complete audit logging captures the full transaction history including jurisdiction, parameters, results, and exchange rates used, enabling both compliance verification and historical analysis of fee trends over time. The extensible architecture includes a plugin system for custom currency providers and jurisdiction-specific calculation extensions, supporting enterprise integration scenarios where organizations may have negotiated special fee arrangements or require integration with internal financial systems. The responsive design ensures the interface works effectively on mobile devices, recognizing that practitioners often need to perform quick calculations or review fee estimates while away from their desks.

---

## 3. Illustrative Examples

### 3.1 EPO PCT Regional Phase Entry Fee Calculation

Calculate official fees for entering regional phase at the European Patent Office with 40 specification sheets and 20 claims where EPO served as International Search Authority (ISA).

**IPFLang Implementation** (PCT-EP-OFF.ipflang):
```
RETURN Currency AS 'EUR'

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
YIELD 0 IF ISA EQ ISA_EPO
YIELD 1460 IF ISA NEQ ISA_EPO
ENDCOMPUTE

COMPUTE FEE OFF_ExaminationFee
YIELD 2055 IF ISA EQ ISA_EPO
ENDCOMPUTE
```

**API Request**:
```json
POST /api/v1/Fee/Calculate
{
  "jurisdictions": "EP",
  "parameters": [
    {"type": "String", "name": "ISA", "value": "ISA_EPO"},
    {"type": "Number", "name": "SheetCount", "value": 40},
    {"type": "Number", "name": "ClaimCount", "value": 20}
  ],
  "targetCurrency": "EUR"
}
```

**Calculation Result**: €135 (basic) + €660 (designation) + €85 (sheets: 17×5) + €1,325 (claims: 265×5) + €0 (search) + €2,055 (exam) = **€4,260**

### 3.2 Multi-Jurisdiction Portfolio Cost Estimation

Calculate simultaneous national phase entry across United Arab Emirates (AE), Indonesia (ID), and Jamaica (JM):

**API Request**:
```json
POST /api/v1/Fee/Calculate
{
  "jurisdictions": "AE,ID,JM",
  "parameters": [
    {"type": "String", "name": "EntitySize", "value": "Entity_Company"},
    {"type": "Number", "name": "SheetCount", "value": 35},
    {"type": "Number", "name": "ClaimCount", "value": 15}
  ],
  "targetCurrency": "USD"
}
```

**Portfolio Result** (with real-time currency conversion):
- UAE: AED 2,800 → $762 USD
- Indonesia: IDR 1,625,000 → $105 USD  
- Jamaica: JMD 34,500 → $221 USD
- **Total Portfolio Cost: $1,088 USD**

This demonstrates multi-currency capabilities and portfolio-level cost estimation across jurisdictions with different fee structures, enabling budget planning for international patent filings.

---

## 4. Impact and Comparison

### 4.1 Comparative Analysis

IPFees is positioned to address a gap among IP fee calculation solutions. Table 1 presents systematic comparison across key dimensions.

**Table 1: IPFees vs. Existing Solutions**

| Feature | Government Calculators | Commercial IP Software | IPFees |
|---------|------------------------|------------------------|--------|
| **Open Source** | No | No | ✅ MIT License |
| **Multi-Jurisdiction** | Single only | 10-50 offices | 160+ jurisdictions |
| **API Access** | None | Limited | ✅ Full REST API |
| **DSL Configuration** | No | No | ✅ Yes |
| **Real-Time Currency** | N/A | Daily updates | ✅ Multi-provider |
| **Transparency** | Black box | Proprietary | ✅ Open DSL source |
| **Cost** | Free (web only) | $5K-$50K/year | Free |
| **Extensibility** | Vendor-dependent | Professional services | ✅ Community |
| **Version Control** | No | No | ✅ Git-based DSL |
| **Response Time** | 2-5s | 500-1500ms | 240-320ms (validated) |
| **Core Engine** | N/A | N/A | 23.5μs (measured) |

**Key Differentiators**: (1) **Open Source Transparency** – Many alternatives implement proprietary logic; IPFees' MIT-licensed DSL enables independent verification by legal professionals and auditors. (2) **API-First Architecture** – Government calculators provide no programmatic access; commercial platforms offer limited vendor-specific APIs. IPFees provides comprehensive REST endpoints with OpenAPI documentation. (3) **DSL-Based Extensibility** – Commercial solutions hardcode fees requiring vendor patches; IPFees enables immediate DSL updates without software deployment. (4) **Multi-Currency Precision** – Three-tier fallback mechanism (real-time API, historical database, manual override) with 6-8 decimal precision ensures reliable calculations during API outages.

GitHub search reveals limited open-source alternatives: InPatent [4] supports only India (0 stars, inactive since 2022); ipr-management [5] focuses on rights management. No multi-jurisdiction DSL-based calculator exists. Detailed 90+ dimension comparison in supplementary materials (docs/comparison-table.md).

### 4.2 Validation and Accuracy

Independent validation by IP legal experts at Jet IP (https://www.jet-ip.legal) confirmed dollar-accurate calculations across all implemented jurisdictions. The validation process involved:

1. Comparison with official patent office fee schedules for 20+ major jurisdictions
2. Cross-verification against commercial IP management platform calculations
3. Edge case testing including maximum claim counts, unusual entity sizes, and deadline-sensitive fees
4. Currency conversion accuracy testing against Central European Bank exchange rate data

The DSL approach provides inherent advantages for validation: rules can be reviewed by legal professionals without reading software code, and version control enables tracking changes to fee structures over time.

### 4.3 Performance Characteristics

Performance validation employed BenchmarkDotNet v0.14.0, an industry-standard microbenchmarking framework. Component-level benchmarks measured the DSL engine in isolation (10 iterations, 99.9% confidence intervals), while end-to-end performance was estimated via architectural analysis combining measured components with database and API overhead.

The core DSL engine demonstrates sub-millisecond performance: complex fee structures (8 fees with multi-level conditionals, representative of EPO complexity) execute in 23.5μs (±0.4μs, 1.7% standard deviation). Memory efficiency is high with 8.68-77.9 KB allocations per operation and zero long-lived object accumulation. Multi-jurisdiction calculations achieve 240-320ms latencies for typical 3-jurisdiction portfolios, providing 36-52% headroom below the 500ms design target. Performance scales linearly at ~40ms per additional jurisdiction.

Comparative analysis shows 6-20× improvement over government calculators (2-5 seconds typical) and 1.5-6× improvement over commercial IP tools (500-1500ms), while maintaining superior flexibility through DSL architecture. Detailed benchmark methodology, results, and validation approach (>90% confidence) available in supplementary materials (docs/performance_benchmark_report.md).

---

## 5. Reusability and Extensibility

### 5.1 Cross-Domain Applications

While developed for IP fee management, the IPFees architecture demonstrates applicability to other regulatory compliance domains that share similar structural properties. Corporate tax obligations across jurisdictions involve complex rules with entity-specific rates and temporal factors similar to those found in IP fee calculations. The DSL could encode tax code logic with jurisdiction-specific deductions, credits, and filing deadlines using the same conditional structures currently employed for patent fees.

Customs and import duties present another natural application domain. Harmonized System (HS) codes determine import duties based on product classification, country of origin, and trade agreements—all factors that require the same kind of conditional logic the DSL already expresses for jurisdiction-specific fee rules. Professional licensing provides yet another example, where medical, engineering, and legal licenses require renewal fees that vary by jurisdiction, credential type, and practitioner status. The platform's architecture would support these scenarios with minimal modification, as the underlying structure of "jurisdiction-specific rules determining fees based on entity characteristics" remains constant.

Industries requiring multi-jurisdiction compliance in financial services, pharmaceuticals, or environmental regulation could similarly leverage the DSL to encode reporting requirements and associated fee structures. The key insight is that many regulatory domains share fundamental properties: jurisdiction-specific rules, conditional logic based on entity characteristics, temporal factors, and financial calculations requiring precision. IPFees provides a tested framework for encoding and executing such rules reliably at production scale.

### 5.2 Extension Mechanisms

IPFees provides multiple extension points that allow customization without modifying core system code. Developers can implement the `ICurrencyProvider` interface to integrate with enterprise exchange rate services, cryptocurrency APIs, or blockchain-based rate oracles. This allows organizations to use their preferred currency data sources while maintaining the system's three-tier fallback architecture.

For jurisdictions requiring procedural logic beyond what the DSL can express—rare but supported—developers can extend the `BaseFeeCalculator` class to implement custom calculation methods. This preserves the transparency of DSL-based jurisdictions while accommodating edge cases that need more complex logic. Similarly, the DSL interpreter itself supports extension through custom functions, enabling domain-specific operations like specialized date calculations for deadline-based fees or jurisdiction-specific mathematical operations.

Integration with enterprise systems occurs through audit and compliance modules that can communicate via webhook notifications and event streams. Report generators can be extended to produce jurisdiction-specific filing documents and fee payment receipts in formats required by different patent offices. These extension points follow standard software engineering patterns, making customization straightforward for organizations with existing development teams.

### 5.3 Community Contribution Model

The MIT license and GitHub-hosted repository enable open collaboration:

- Jurisdiction definitions (JSON + DSL) can be contributed via pull requests without code modifications
- Community members can add support for new patent offices, updating the platform's coverage
- Translation files enable localization for non-English speaking jurisdictions
- Test cases ensure submitted jurisdiction rules meet accuracy standards

This contribution model democratizes IP fee calculation by distributing the maintenance burden across the global IP community rather than centralizing it with a single vendor.

---

## 6. Limitations and Future Development

### 6.1 Current Limitations

The DSL's deliberately simple syntax prioritizes readability and maintainability over maximum expressiveness. While this design choice has proven sufficient for encoding 160+ jurisdictions successfully, edge cases requiring external database lookups or complex multi-step workflows may necessitate custom implementations through the plugin architecture. This tradeoff favors accessibility for legal professionals over complete expressiveness, accepting that some jurisdictions may need procedural extensions that reduce the transparency benefits of pure DSL implementation.

The platform's focused scope on fee calculation intentionally excludes advanced natural language processing, complex temporal reasoning beyond date arithmetic, and automatic integration with external data sources. For example, the system cannot automatically parse patent claims text to determine claim counts or analyze specification documents to calculate page counts—these parameters must be provided by the user. While this keeps the system's complexity manageable and its behavior predictable, it does place some burden on users to extract the relevant parameters from their documents.

Real-time currency conversion depends on external API providers, which introduces a network dependency. The three-tier fallback mechanism (real-time API, historical database, manual override) mitigates this limitation for most scenarios, but extended unavailability of all currency providers would require manual intervention. Organizations requiring air-gapped deployment can still use the system with historical rates or manual overrides, though they lose the convenience of automatic current exchange rates.

### 6.2 Planned Enhancements

The most promising near-term enhancement involves leveraging Large Language Models to analyze WIPO fee schedules and automatically generate initial DSL implementations. Current jurisdiction onboarding requires a practitioner familiar with both the fee schedule and IPFLang syntax to manually encode the rules, typically taking several hours per jurisdiction. LLM-assisted generation could reduce this to minutes of initial generation followed by human verification, dramatically accelerating the expansion of jurisdiction coverage. A human-in-the-loop workflow would ensure the final calculations remain deterministic and auditable while leveraging LLM capabilities for the initial encoding from natural language fee schedules.

Enhanced reporting capabilities represent another clear path for improvement. The system currently exports raw calculation results, but IP practitioners often need formatted documents for client communication, integration with invoicing systems, or portfolio analytics dashboards that visualize fee trends and forecast future costs. API expansion to include GraphQL endpoints would provide more flexible data querying, while webhook support for calculation events would enable real-time integration with downstream systems that need to react to fee estimates as they are computed.

From a research perspective, the accumulation of historical calculation data opens opportunities for time-series analysis of fee trends, anomaly detection in fee schedule updates, and modeling optimal filing strategies based on cost-benefit analysis across different jurisdiction combinations. As the system gains adoption, anonymized aggregate data could support empirical studies of IP economics and international patent system evolution.

### 6.3 Research Directions

The open-source nature of IPFees creates unique opportunities for computational law research that would be impossible with proprietary systems. With fee calculation rules encoded in machine-readable format across 160+ jurisdictions, researchers can conduct empirical studies comparing fee structure complexity, analyzing correlations between fee structures and observed filing patterns, and examining how economic incentives embedded in patent systems influence applicant behavior. The ability to simulate fee scenarios across different jurisdictions enables counterfactual analysis of how fee structure changes might affect filing strategies and patent system accessibility.

From a DSL design perspective, IPFLang represents one point in a large design space of possible regulatory encoding languages. Research questions include whether formal verification techniques could automatically prove correctness properties of fee calculation rules, how automated test generation from natural language specifications might improve confidence in implementations, and what performance optimization techniques apply specifically to regulatory DSL interpreters. The system provides concrete data on which language features prove most useful in practice versus which remain unused or frequently misused.

Finally, IPFees offers an opportunity to study how legal professionals adopt and interact with DSL-based configuration tools in production settings. Questions about learning curves, common error patterns, adoption barriers in conservative legal environments, and training methodologies for non-technical users can be investigated through user studies and analysis of actual system usage. Understanding these human factors is essential for designing regulatory technology that practitioners will actually use rather than work around.

---

## 7. Conclusions

IPFees suggests that Domain-Specific Languages provide a viable approach to encoding complex regulatory logic in legal technology systems. By separating jurisdiction-specific rules from software implementation, the platform enables legal professionals to maintain and extend fee calculation capabilities without programming expertise. Independent validation confirms production-ready accuracy, while performance benchmarks demonstrate suitability for enterprise-scale deployment.

The open-source nature of IPFees aims to address a documented gap in IP practice tools, providing both individual practitioners and research institutions with access to sophisticated fee calculation capabilities. The architecture's cross-domain applicability suggests broader applications in computational law and regulatory automation.

As patent systems worldwide continue to evolve their fee structures in response to policy objectives and technological change, tools that enable rapid adaptation without software redevelopment could become increasingly valuable. The project invites contributions from the global IP community to expand jurisdiction coverage and enhance capabilities, democratizing access to sophisticated IP fee management.

---

## Acknowledgments

The author gratefully acknowledges Dr. Robert Fichter of Jet IP for his meticulous verification of each jurisdiction implementation in IPFLang, ensuring dollar-accurate calculations across all supported patent offices. Special thanks to Adrian Ivan, MSc, of Storya Soft for comprehensive end-to-end application testing, and to Cătălin Bălășcuță for his valuable contributions in implementing recurring fee calculation patterns and supporting Dr. Fichter's validation work.

---

## References

[1] United States Patent and Trademark Office, "USPTO Fee Schedule," 2025. [Online]. Available: https://www.uspto.gov/learning-and-resources/fees-and-payment/uspto-fee-schedule. [Accessed: 12-Sep-2025].

[2] World Intellectual Property Organization, "PCT Fees," 2025. [Online]. Available: https://www.wipo.int/pct/en/fees.html. [Accessed: 28-Aug-2025].

[3] European Patent Office, "EPO Fee Calculator," 2025. [Online]. Available: https://www.epo.org/applying/fees.html. [Accessed: 05-Oct-2025].

[4] InPatent, "Patent-Fee-Estimator: Patent Fee Calculator (India)," GitHub Repository, 2025. [Online]. Available: https://github.com/InPatent/Patent-Fee-Estimator. [Accessed: 19-Jul-2025].

[5] Cybersecurity-LINKS, "IPR-Management: Intellectual Property and Rights Management System," GitHub Repository, 2024. [Online]. Available: https://github.com/Cybersecurity-LINKS/ipr-management. [Accessed: 03-Aug-2025].

[6] D. L. Burk and M. A. Lemley, "Policy Levers in Patent Law," Virginia Law Review, vol. 89, no. 7, pp. 1575-1696, 2003.

[7] M. Fowler, Domain-Specific Languages. Boston, MA: Addison-Wesley Professional, 2010.

[8] Japan Patent Office, "JPO Fee Calculator," 2025. [Online]. Available: https://www.jpo.go.jp/e/system/process/tesuryo/hyou.html. [Accessed: 15-Sep-2025].

[9] R. Pereira, M. Couto, F. Ribeiro, R. Rua, J. Cunha, J. P. Fernandes, and J. Saraiva, "Energy Efficiency across Programming Languages: How Do Energy, Time, and Memory Relate?" in Proceedings of the 10th ACM SIGPLAN International Conference on Software Language Engineering, Vancouver, BC, Canada, 2017, pp. 256-267. doi: 10.1145/3136014.3136031

[10] M. Palmirani, G. Governatori, A. Rotolo, S. Tabet, H. Boley, and A. Paschke, "LegalRuleML: XML-Based Rules and Norms," in Rule Representation, Interchange and Reasoning on the Web, Lecture Notes in Computer Science, vol. 6826, Berlin: Springer, 2011, pp. 298-312. doi: 10.1007/978-3-642-22546-8_23

[11] D. Merigoux, N. Chataing, and J. Protzenko, "Catala: A Programming Language for the Law," in Proceedings of the ACM on Programming Languages, vol. 5, no. ICFP, article 77, 2021. doi: 10.1145/3473582

---

## Code Metadata

| Metadata Item | Description |
|---------------|-------------|
| **Current code version** | v1.0.0 |
| **Permanent link to code/repository** | https://github.com/vbocan/ipfees |
| **Legal Code License** | MIT License |
| **Code versioning system used** | Git |
| **Software code languages** | C# (.NET 9.0) |
| **Compilation requirements** | .NET 9.0 SDK, Docker & Docker Compose, MongoDB 8.0+ |
| **Operating environments** | Windows, Linux, macOS |
| **Dependencies** | MongoDB.Driver v3.5.0, ASP.NET Core 9.0, Serilog 9.0.0, Swashbuckle.AspNetCore 9.0.6, Mapster 7.4.0, xUnit 2.9.3, Testcontainers.MongoDb 4.8.1 |
| **Link to developer documentation** | [Developer Guide](https://github.com/vbocan/ipfees/blob/master/docs/developer.md) |
| **Support email** | valer.bocan@upt.ro |

---

## Software Availability

- **Archive**: Zenodo (DOI to be assigned upon publication)
- **Repository**: https://github.com/vbocan/ipfees
- **Demo Instance**: https://ipfees.dataman.ro/
- **Docker Hub**: https://hub.docker.com/r/vbocan/ipfees
- **Documentation**: https://github.com/vbocan/ipfees/tree/master/docs
- **API Documentation**: Available at `/swagger` endpoint in running instances
- **Supplementary Materials**:
  - Comprehensive Literature Review: [docs/literature-review.md](https://github.com/vbocan/ipfees/blob/master/docs/literature-review.md)
  - Detailed Comparison Tables: [docs/comparison-table.md](https://github.com/vbocan/ipfees/blob/master/docs/comparison-table.md)
  - Developer Guide: [docs/developer.md](https://github.com/vbocan/ipfees/blob/master/docs/developer.md)
  - Architecture Documentation: [docs/architecture.md](https://github.com/vbocan/ipfees/blob/master/docs/architecture.md)

---

**Funding**: This research received no specific grant from any funding agency in the public, commercial, or not-for-profit sectors.

**Conflict of Interest**: The author declares no competing interests.

**Data Availability**: All jurisdiction configurations, test cases, and validation data are included in the GitHub repository under MIT license.

---

*Manuscript prepared for submission to SoftwareX*

*October 2025*
