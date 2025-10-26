# IPFees - Intellectual Property Fee Calculator

[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)](https://dotnet.microsoft.com)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue?logo=docker)](https://docker.com)
[![MongoDB](https://img.shields.io/badge/Database-MongoDB-green?logo=mongodb)](https://mongodb.com)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/ipfees/ipfees)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![API](https://img.shields.io/badge/API-REST-orange)](https://localhost:8090/swagger)

**🌐 [Live Demo Application](https://ipfees.dataman.ro/)** | **🐳 [Docker Hub](https://hub.docker.com/repository/docker/vbocan/ipfees)** | **📚 [Developer Guide](docs/developer.md)**

![IPFees Application Screenshot](ipfees-screenshot.png)

## Code Metadata

| Metadata Item                                                       | Description                                                                                |
| ------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| **Current code version**                                            | v1.0.0                                                                                     |
| **Permanent link to code/repository**                               | https://github.com/vbocan/ipfees                                                           |
| **Legal Code License**                                              | MIT License                                                                                |
| **Code versioning system used**                                     | Git                                                                                        |
| **Software code languages, tools, and services used**               | C# (.NET 9.0), ASP.NET Core, MongoDB, Docker, Razor Pages, Bootstrap 5, jQuery, xUnit      |
| **Compilation requirements, operating environments & dependencies** | .NET 9.0 SDK, Docker & Docker Compose, MongoDB 8.0+; Compatible with Windows, Linux, macOS |
| **Link to developer documentation/manual**                          | [Developer Guide](docs/developer.md)                                                       |
| **Support email for questions**                                     | GitHub Issues or Discussions                                                               |

## Overview

IPFees is a jurisdiction-agnostic intellectual property fee calculation system that automates complex legal fee structures with multi-currency support. The platform provides automated fee calculations across 160+ global IP jurisdictions using a Domain-Specific Language (DSL) approach, enabling legal professionals to define and modify fee calculation rules without software development expertise.

## Quick Start with Docker

### Option 1: Clone from GitHub (Recommended)

The easiest way to get started:

```bash
# Clone the repository
git clone https://github.com/vbocan/ipfees
cd ipfees/src

# Start all services
docker-compose up -d
```

That's it! The system includes sample data and will work immediately.

### Option 2: Pull from Docker Hub

If you prefer to run without cloning the repository:

```bash
# Pull the images from Docker Hub
docker pull vbocan/ipfees:web-latest
docker pull vbocan/ipfees:api-latest

# Create a docker-compose.yml file (see example below)
# Then start the services
docker-compose up -d
```

**Example docker-compose.yml**:

```yaml
version: "3.8"

services:
  mongodb:
    image: mongo:8.0
    container_name: ipfees-mongodb
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db
    environment:
      MONGO_INITDB_DATABASE: ipfees

  ipfees-web:
    image: vbocan/ipfees:web-latest
    container_name: ipfees-web
    ports:
      - "8080:8080"
    depends_on:
      - mongodb
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__MongoDB=mongodb://mongodb:27017/ipfees

  ipfees-api:
    image: vbocan/ipfees:api-latest
    container_name: ipfees-api
    ports:
      - "8090:8090"
    depends_on:
      - mongodb
    environment:
      - ASPNETCORE_URLS=http://+:8090
      - ConnectionStrings__MongoDB=mongodb://mongodb:27017/ipfees

volumes:
  mongodb_data:
```

### Access the Application

- **Web Application** (local): http://localhost:8080
- **API Documentation** (local): http://localhost:8090/swagger
- **MongoDB** (local): localhost:27017

That's it! The system includes sample data and will work immediately. For production use, configure your exchange rate API key in the settings.

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test IPFees.Core.Tests
dotnet test IPFees.Calculator.Tests
```

## Technical Architecture

IPFees leverages a modular architecture designed specifically for the demands of global IP practice, addressing complex requirements of many distinct regulatory frameworks.

For a detailed technical architecture diagram, see [architecture.md](docs/architecture.md).

### Technology Stack

- **Backend**: .NET 9.0, ASP.NET Core
- **Frontend**: Razor Pages, Bootstrap 5
- **Database**: MongoDB with GridFS
- **Containerization**: Docker & Docker Compose
- **Background Services**: .NET BackgroundService
- **Expression Parsing**: Custom DSL interpreter
- **Testing**: xUnit, Testcontainers

### Key Features

- **DSL-Based Fee Calculation**: Define complex fee structures in human-readable format without hardcoding business logic
- **Multi-Jurisdiction Support**: Configurable architecture supporting USPTO, EPO, WIPO, and 160+ national patent offices
- **Real-Time Currency Management**: Multi-currency precision with real-time conversion, historical rate tracking, and three-tier fallback system
- **API-First Design**: Comprehensive REST APIs for integration with IP management platforms
- **Bulk Processing**: Portfolio-level fee estimation for large IP holdings
- **Extensible Architecture**: Add new jurisdictions through configuration without code changes

## Illustrative Examples

### Example 1: EPO PCT Regional Phase Entry

Calculate official fees for entering regional phase at the European Patent Office using the Web UI or API:

**Via Web Interface**:

1. Visit https://ipfees.dataman.ro/ or http://localhost:8080
2. Select "EP" jurisdiction
3. Enter parameters:
   - ISA: EPO
   - IPRP: None
   - Sheet Count: 40
   - Claim Count: 20
4. Click "Calculate"

**Via API**:

```bash
curl -X POST http://localhost:8090/api/v1/Fee/Calculate \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "jurisdictions": "EP",
    "parameters": [
      {"type": "String", "name": "ISA", "value": "ISA_EPO"},
      {"type": "String", "name": "IPRP", "value": "IPRP_NONE"},
      {"type": "Number", "name": "SheetCount", "value": 40},
      {"type": "Number", "name": "ClaimCount", "value": 20}
    ],
    "targetCurrency": "EUR"
  }'
```

**Result**:

- Basic National Fee: €135
- Designation Fee: €660
- Sheet Fee: €85
- Claim Fee: €1,325
- Search Fee: €0 (EPO as ISA)
- Examination Fee: €2,055
- **Total: €4,260**

### Example 2: Multi-Jurisdiction Calculation

Calculate fees for simultaneous national phase entry across multiple jurisdictions:

```bash
curl -X POST http://localhost:8090/api/v1/Fee/Calculate \
  -H "Content-Type: application/json" \
  -H "X-API-Key: your-api-key" \
  -d '{
    "jurisdictions": "EP,JP,CA",
    "parameters": [
      {"type": "String", "name": "EntitySize", "value": "Entity_Company"},
      {"type": "Number", "name": "SheetCount", "value": 35},
      {"type": "Number", "name": "ClaimCount", "value": 15}
    ],
    "targetCurrency": "USD"
  }'
```

**Result**: Aggregated costs across multiple jurisdictions with automatic currency conversion to USD.

### Example 3: IPFLang Fee Definition

Define a new fee structure using IPFLang DSL:

```
# Canadian National Phase Entry
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

The IPFLang DSL uses keyword-based syntax (DEFINE, COMPUTE, YIELD) that is readable by legal professionals without programming expertise.

## Scientific Impact and Reusability

### Cross-Domain Applicability

While developed for IP fee management, the DSL-based architecture is applicable to other regulatory domains:

- International tax calculations
- Customs and import duty calculations
- Legal compliance fee structures
- Multi-jurisdiction regulatory reporting

### Research Applications

- **Legal Technology Research**: Framework for studying automated compliance systems
- **Domain-Specific Languages**: Reference implementation for regulatory rule engines
- **Multi-Currency Systems**: Pattern for handling financial calculations across jurisdictions
- **API Integration Studies**: Example of REST API design for legal technology

### Extensibility

The modular architecture enables researchers and practitioners to:

1. Add new jurisdictions through JSON configuration
2. Implement custom currency providers
3. Extend the DSL with domain-specific functions
4. Integrate with existing IP management systems via REST APIs

## Performance Metrics

- **Calculation Latency**: <500ms for complex multi-jurisdiction calculations ✅ [Validated](docs/performance_benchmark_report.md)
  - Typical 3-jurisdiction: 240-320ms (36-52% below target)
  - Core DSL engine: 23.5μs for complex fee structures
  - P95 latency: 420ms under normal load
- **Multi-Currency Precision**: 6-8 decimal places for high-value portfolios
- **Currency Support**: 150+ currencies with real-time conversion
- **Concurrent Users**: Scalable architecture supporting 25+ simultaneous users
- **Extensibility**: Add jurisdictions without code changes through configuration

**Performance Validation:** Comprehensive benchmarking completed using BenchmarkDotNet v0.14.0. See [Performance Benchmark Report](docs/performance_benchmark_report.md) for detailed methodology, results, and analysis.

## Citation

If you use IPFees in your research, please cite:

```bibtex
@software{ipfees2025,
  title = {IPFees: Intellectual Property Fee Calculator},
  author = {Valer Bocan, PhD, CSSLP},
  year = {2025},
  version = {1.0.0},
  url = {https://github.com/vbocan/ipfees},
  license = {MIT}
}
```

See [CITATION.cff](CITATION.cff) for structured citation metadata.

## Contributing

We welcome contributions from the community:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

For major changes, please open an issue first to discuss proposed modifications. See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. This OSI-approved license permits academic research, commercial use, modification, and integration into other projects.

## Documentation

- **User Guide**: See web application's built-in documentation at `/about` and `/reference`
- **API Documentation**: Interactive Swagger/OpenAPI documentation at `/swagger`
- **Architecture**: Detailed technical architecture in [docs/architecture.md](docs/architecture.md)
- **Developer Guide**: Instructions for extending the system in [docs/developer.md](docs/developer.md)
- **DSL Grammar**: Fee calculation DSL specification at `/grammar`

## Support & Contact

- **Issues**: Report bugs and request features via [GitHub Issues](https://github.com/vbocan/ipfees/issues)
- **Discussions**: Community support via [GitHub Discussions](https://github.com/vbocan/ipfees/discussions)
- **Documentation**: Comprehensive documentation in the `/docs` folder
- **Email**: valer.bocan@upt.ro

## Acknowledgments

IPFees addresses documented inefficiencies in IP fee management where professionals spend significant time on repetitive calculations across fragmented government-provided calculators and limited commercial solutions. The platform aims to improve efficiency through automation while maintaining open-source principles for research and educational use.

---

**Version**: 1.0.0  
**Status**: Active Development  
**Repository**: https://github.com/vbocan/ipfees
