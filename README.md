# API Starter Kit

A modern .NET API starter kit with layered architecture, following clean code principles and best practices.

## Project Structure

The solution follows a layered architecture pattern with the following projects:

- **API**: Presentation layer - Contains controllers and API endpoints
- **BLL**: Business Logic Layer - Contains business rules and services
- **DAL**: Data Access Layer - Contains repositories and database context
- **CORE**: Core layer - Contains interfaces, constants, and shared utilities
- **DTO**: Data Transfer Objects - Contains request/response models
- **ENTITIES**: Domain entities - Contains database models
- **SOURCE**: Generate source code for all layers to implement CRUD operations

## Prerequisites

- .NET 9.0 or later
- Visual Studio 2022 or later
- PostgreSQL (for database)

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Update the connection string in `appsettings.json`
5. Run the database migrations
6. Build and run the project

## Features

- Clean Architecture
- Repository Pattern
- Dependency Injection
- Entity Framework Core
- Swagger UI for API documentation
- Docker support
- CI/CD pipeline configuration

## Development

### Building the Project

```bash
dotnet build
```

### Running the Project

```bash
dotnet run --project API
```

### Running Tests

```bash
dotnet test
```

## Docker Support

The project includes Docker support. To build and run the container:

```bash
docker build -t api-starter .
docker run -p 5000:80 api-starter
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request