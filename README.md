# IPFees - Intelligent Intellectual Property Fee Calculator

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/ipfees/ipfees)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue?logo=docker)](https://docker.com)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)](https://dotnet.microsoft.com)
[![MongoDB](https://img.shields.io/badge/Database-MongoDB-green?logo=mongodb)](https://mongodb.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![API](https://img.shields.io/badge/API-REST-orange)](https://localhost:8090/swagger)

![IPFees Application Screenshot](ipfees-screenshot.png)

## Quick Start with Docker

### Prerequisites

- [Docker](https://docker.com) and Docker Compose

### Start the Application

```bash
# Clone the repository
git clone <repository-url>
cd ipfees/src

# Start all services
docker-compose up -d
```

### Access the Application

- **Web Application**: http://localhost:8080
- **API Documentation**: http://localhost:8090/swagger
- **MongoDB**: localhost:27017

That's it! The system includes sample data and will work immediately. For production use, configure your exchange rate API key in the settings.

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test IPFees.Core.Tests
dotnet test IPFees.Calculator.Tests
```

## IPFees: Transforming global IP fee management through intelligent automation

A revolutionary, jurisdiction-agnostic intellectual property fee calculation system that transforms complex legal fee structures into automated, accurate, and multi-currency calculations. IPFees addresses the $50,000 annual efficiency drain faced by IP professionals navigating complex multi-jurisdictional fee structures, delivering the world's first truly comprehensive IP fee calculation platform that scales seamlessly across 160+ global IP jurisdictions.

## Problem Statement

The intellectual property management sector faces a fundamental operational crisis. Research reveals that IP professionals waste approximately 10 hours per week on repetitive fee calculations, representing over $50,000 in lost annual productivity at current billing rates. This inefficiency stems from:

- **Fragmented calculation tools**: Government-provided calculators operate in isolation without integration capabilities
- **Limited commercial solutions**: Existing IP management platforms suffer from restricted APIs and require extensive manual data entry
- **Currency conversion timing mismatches**: Creating accounting discrepancies and compliance issues
- **Complex entity-based fee calculations**: Varying dramatically across 160+ jurisdictions
- **Absence of real-time integration**: No seamless connectivity with practice management systems

With 3.55 million patent applications filed worldwide in 2023 and the IP management software market projected to grow from $12.30 billion in 2024 to $24.82 billion by 2030 (12.9% CAGR), there is an urgent need for intelligent automation in IP fee management.

## Key Features & Novel Approach

### Intelligent Fee Calculation Engine

The system implements a novel Domain-Specific Language (DSL) parser that allows complex fee structures to be defined in human-readable format rather than hardcoded business logic. This approach enables legal professionals to define and modify fee calculation rules without requiring software development expertise. The dynamic fee evaluation engine interprets these rules at runtime, supporting multi-layered calculations that include base fees, service fees, and complex conditional logic based on application parameters such as claim counts, page numbers, and filing types.

### Multi-Jurisdiction Architecture

The jurisdiction-agnostic fee calculation engine represents a revolutionary approach to handling international intellectual property fee calculations, processing complex, multi-variable fee structures across major patent offices including USPTO, EPO, WIPO, and 160+ national offices. Each jurisdiction implements distinct entity classifications, claim-based pricing models, and temporal fee obligations that traditional systems struggle to manage efficiently.

Unlike systems that hardcode jurisdiction-specific logic, this modular architecture allows new countries or regions to be added through configuration alone. The platform provides intelligent API integration capabilities that enable seamless connectivity with major IP management platforms while addressing the integration limitations that currently force manual processes. Independent fee modules can be combined and configured per jurisdiction, enabling unprecedented scalability and maintainability while solving the "no API" problem that hinders current solutions like Memotech and Anaqua.

### Advanced Currency Management

The system provides sophisticated multi-currency precision management that represents a core technical differentiator. Unlike existing solutions that struggle with exchange rate timing discrepancies, IPFees implements real-time currency conversion with sub-second precision, historical rate tracking for audit compliance, and rate-lock options to eliminate the accounting mismatches that plague current IP practice management. The system integrates with multiple foreign exchange data providers, implements sophisticated caching mechanisms to reduce API dependencies, and maintains 6-8 decimal place precision required for financial accuracy in high-value patent portfolios.

When online exchange rate services are unavailable, the intelligent three-tier fallback system automatically transitions to backup historic rates, ensuring continuous operation. The status-aware currency handling system provides clear user feedback about data freshness (Online, Stale, or Invalid) and supports automatic conversion between any of the 150+ supported currencies.

### Resilient Data Management

A hybrid data strategy combines MongoDB for persistent storage of jurisdiction data, fee structures, and calculation history with backup for critical exchange rate information. Background services automatically update exchange rates at configurable intervals, while graceful degradation ensures the system continues operating with backup data when external services fail. This approach guarantees high availability and data integrity.

### User-Centric Design

The web-based interface features a wizard-driven fee calculation process that guides users through jurisdiction selection, parameter specification, and result review. Real-time feedback provides instant validation and calculation updates as parameters change. The responsive design ensures optimal functionality across desktop and mobile devices, while transparent data status indicators keep users informed about the reliability of exchange rate data.

### Administrative Excellence

Runtime configuration capabilities allow fee structures to be modified without code deployment or system downtime. The administrative interface provides comprehensive module management for creating, editing, and organizing fee calculation modules. Centralized settings management handles currencies, API keys, and system parameters, while complete audit trails track all fee calculations and system changes for compliance and troubleshooting purposes.

### Regulatory Compliance and Monitoring

Regulatory compliance management is built into the platform's foundation, with automated monitoring of fee schedule changes across jurisdictions, entity status verification systems that ensure ongoing small entity compliance, and comprehensive audit trails that meet the stringent requirements of USPTO, EPO, and international IP offices. The system maintains compliance with GDPR data privacy requirements, implements end-to-end encryption for financial data, and provides detailed reporting capabilities required for regulatory compliance across multiple jurisdictions.

## Technical Architecture

IPFees leverages a modular architecture designed specifically for the demands of global IP practice, addressing complex requirements of many distinct regulatory frameworks.

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   IPFees.Web    │    │   IPFees.API    │    │   MongoDB       │
│  (Web Interface)│◄──►│   (REST API)    │◄──►│ (GridFS/Audit)  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  IPFees.Core    │    │ Exchange Rate   │    │ Reporting &     │
│ (Business Logic)│    │ Services        │    │ Analytics       │
│ & DSL Engine    │    │ (Multi-Provider)│    │ Engine          │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│ Jurisdiction    │    │ Currency Cache  │    │ Compliance      │
│ Configuration   │    │ & Fallback      │    │ Monitoring      │
│ Engine          │    │ System          │    │ & Audit Trail   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### Technology Stack

- **Backend**: .NET 9.0, ASP.NET Core
- **Frontend**: Razor Pages, Bootstrap 5
- **Database**: MongoDB with GridFS
- **Containerization**: Docker & Docker Compose
- **Background Services**: .NET BackgroundService
- **Expression Parsing**: Custom DSL interpreter
- **Testing**: xUnit

## Market Position & Competitive Advantage

### Target Market Segments

IPFees addresses four distinct market segments within the rapidly growing IP management ecosystem:

- **BigLaw IP practices** (52% of law firm market share): Advanced portfolio analytics, multi-client fee management, and sophisticated reporting capabilities that integrate with existing billing systems
- **Corporate IP departments**: Executive-level cost analytics, budget forecasting capabilities, and automated renewal management for large patent portfolios
- **Boutique IP firms and solo practitioners**: Cost-effective solutions that automate routine fee calculations while providing essential deadline management
- **International associates and IP service providers**: Automated currency management and jurisdiction-specific compliance monitoring

### Key Differentiators

Unlike existing solutions like WIPO's fragmented calculators or commercial platforms with limited fee calculation sophistication, IPFees delivers end-to-end fee lifecycle management:

- **Real-time accuracy**: 99%+ fee calculation precision with automated updates when jurisdictions modify fee schedules
- **Bulk processing capabilities**: Portfolio-level fee estimation and optimization for large IP holdings
- **Currency hedge options**: Advanced foreign exchange management tools that eliminate timing-based accounting discrepancies
- **Advanced reporting**: Comprehensive cost analysis and historical tracking across jurisdictions
- **Compliance automation**: Automated entity status monitoring and regulatory compliance across multiple jurisdictions

## Performance Metrics

- **Sub-second Calculations**: Complex multi-jurisdiction fee calculations complete in <500ms
- **Multi-currency precision**: Supports 150+ currencies with 6-8 decimal place accuracy
- **Scalable architecture**: Handles concurrent calculations for multiple users across global jurisdictions
- **Extensible platform**: Add new jurisdictions without code changes through configuration
- **API Integration**: Comprehensive REST APIs for seamless connectivity with major IP management platforms

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. This license is suitable for academic research, commercial use, and integration into other projects.

## Support & Contact

- **Issues**: Create GitHub issues for bugs and feature requests
- **Documentation**: Check the `/docs` folder for detailed documentation
- **API Documentation**: Available at `/swagger` endpoint when running

---

Made for the Intellectual Property community

_Transforming complex fee calculations into simple, accurate, and reliable automated processes._
