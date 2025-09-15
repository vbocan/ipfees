# IPFees - Intelligent Intellectual Property Fee Calculator

[![.NET](https://img.shields.io/badge/.NET-9.0-purple?logo=dotnet)](https://dotnet.microsoft.com)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue?logo=docker)](https://docker.com)
[![MongoDB](https://img.shields.io/badge/Database-MongoDB-green?logo=mongodb)](https://mongodb.com)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/ipfees/ipfees)
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

## Technical Architecture

IPFees leverages a modular architecture designed specifically for the demands of global IP practice, addressing complex requirements of many distinct regulatory frameworks.

For a detailed technical architecture diagram, see [architecture.md](architecture.md).

### Technology Stack

- **Backend**: .NET 9.0, ASP.NET Core
- **Frontend**: Razor Pages, Bootstrap 5
- **Database**: MongoDB with GridFS
- **Containerization**: Docker & Docker Compose
- **Background Services**: .NET BackgroundService
- **Expression Parsing**: Custom DSL interpreter
- **Testing**: xUnit

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
