# IPFees Technical Architecture

```mermaid
graph TB
    %% Define subgraphs for layers
    subgraph "Presentation Layer"
        Web[IPFees.Web<br/>Web Interface]
        API[IPFees.API<br/>REST API]
        Mongo[(MongoDB<br/>GridFS/Audit)]
    end

    subgraph "Business Logic Layer"
        Core[IPFees.Core<br/>Business Logic<br/>& DSL Engine]
        Exchange[Exchange Rate<br/>Services<br/>Multi-Provider]
        Reporting[Reporting &<br/>Analytics<br/>Engine]
    end

    subgraph "Data & Configuration Layer"
        Jurisdiction[Jurisdiction<br/>Configuration<br/>Engine]
        Currency[Currency Cache<br/>& Fallback<br/>System]
        Compliance[Compliance<br/>Monitoring<br/>& Audit Trail]
    end

    %% Horizontal connections (bidirectional)
    Web <--> API
    API <--> Mongo
    Core <--> Exchange
    Exchange <--> Reporting
    Jurisdiction <--> Currency
    Currency <--> Compliance

    %% Vertical connections (downward flow)
    Web --> Core
    API --> Exchange
    Mongo --> Reporting
    Core --> Jurisdiction
    Exchange --> Currency
    Reporting --> Compliance

    %% Styling
    classDef presentation fill:#2E86AB,stroke:#fff,stroke-width:2px,color:#fff
    classDef business fill:#F18F01,stroke:#fff,stroke-width:2px,color:#fff
    classDef data fill:#C73E1D,stroke:#fff,stroke-width:2px,color:#fff
    classDef database fill:#A23B72,stroke:#fff,stroke-width:2px,color:#fff

    class Web,API presentation
    class Core,Exchange,Reporting business
    class Jurisdiction,Currency,Compliance data
    class Mongo database
```

## Architecture Overview

The IPFees system is built using a modular, three-tier architecture that provides clear separation of concerns and excellent scalability:

### Presentation Layer
- **IPFees.Web**: Razor Pages-based web application providing the user interface
- **IPFees.API**: RESTful API services for external integrations and mobile clients
- **MongoDB**: Document database with GridFS for file storage and comprehensive audit logging

### Business Logic Layer
- **IPFees.Core**: Contains the Domain-Specific Language (DSL) engine and core business logic
- **Exchange Rate Services**: Multi-provider currency conversion with intelligent fallback mechanisms
- **Reporting & Analytics Engine**: Comprehensive reporting and data analysis capabilities

### Data & Configuration Layer
- **Jurisdiction Configuration Engine**: Manages jurisdiction-specific fee rules and regulations
- **Currency Cache & Fallback System**: Handles currency data caching and backup mechanisms
- **Compliance Monitoring & Audit Trail**: Ensures regulatory compliance and maintains audit logs

## Key Architectural Benefits

1. **Modular Design**: Each component can be developed, tested, and deployed independently
2. **Scalability**: Horizontal scaling possible at each layer
3. **Resilience**: Multiple fallback mechanisms ensure high availability
4. **Maintainability**: Clear separation of concerns makes the system easy to maintain
5. **Extensibility**: New jurisdictions and features can be added without code changes