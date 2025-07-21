# Airbnb Project

## Description

**Airbnb Project** is a modern multi-layered .NET application implementing an apartment booking service with clean architecture, test coverage, automated CI/CD, and support for large data migrations.

---

## Architecture

The project is built following **Clean Architecture** principles and is divided into layers:

- **Domain** — domain entities, value objects, business rules.
- **Application** — business logic, services, interfaces, DTOs, validation.
- **Infrastructure** — repository implementations, providers, services, database access (EF Core, Dapper), migrations.
- **Web (Airbnb)** — ASP.NET Core Web API, controllers, middleware, DI, Swagger.
- **DataMigrationApp** — a separate console application for migrating large volumes of data.

---

## Main Technologies and Tools

- **.NET 8** — modern development platform.
- **Entity Framework Core** — ORM for database access.
- **Dapper** — fast SQL queries for complex analytics and aggregations.
- **Mapster** — DTO ↔ Entity mapping.
- **FluentValidation** — DTO validation.
- **Identity** — user and role management.
- **JWT** — authentication and authorization.
- **Swagger** — auto-generated API documentation.
- **Docker** — application containerization.
- **GitHub Actions** — CI/CD: build, test, Docker image publishing.
- **xUnit, Moq, Shouldly** — unit testing.

---

## Getting Started

### 1. Clone and Setup
```bash
git clone https://github.com/your-username/airbnb-project.git
cd airbnb-project
```

### 2. Configuration
- Set up connection strings in `appsettings.json` and/or environment variables.
- Use Docker or .NET CLI to run migrations and the application.

### 3. Build and Run with Docker
```bash
docker build -t airbnb-project .
docker run -p 80:80 airbnb-project
```

### 4. Local Run
```bash
dotnet build
cd Airbnb
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### 5. Swagger UI
- After starting the API, documentation is available at: `http://localhost:80/swagger` or `http://localhost:5296/swagger`

---

## CI/CD
- GitHub Actions is used for automatic build, testing, coverage report generation, and Docker image publishing to Docker Hub.
- All secrets (Docker Hub login/token) are stored in GitHub Secrets.

---

## Data Migration Application (DataMigrationApp)

**DataMigrationApp** is a separate console application for migrating large volumes of data (e.g., importing users and apartments from JSON).

- Uses batching, transactions, and logging for reliability.
- Run:
  ```bash
  cd DataMigrtationApp
  dotnet run
  ```
- File path and connection string are configured in code or via environment variables.

---

## Dapper Queries Feature

For complex analytical queries and aggregations, **Dapper** is used:
- Fast SQL queries for grouping, statistics, price quantiles, etc.
- All SQL scripts are stored in the `Airbnb.Infrastructure/ResourcesSql` folder.
- Dapper repositories implement interfaces and are used via services, making it easy to extend and optimize analytical features without losing performance.

---

## Testing
- Key business scenarios are covered by unit tests (xUnit, Moq, Shouldly).
- Both positive and negative scenarios are tested.
- Test and coverage reports are available as CI/CD artifacts.

---

## Security
- All secrets and tokens are stored in environment variables or GitHub Secrets.
- JWT authentication, roles, and a global error handling middleware are implemented.

---

## Contacts

- Author: [Your Name]
- Email: [your email]
- Docker Hub: https://hub.docker.com/r/illiasychovv/airbnb-project
- GitHub: https://github.com/IlliaSychovv/Airbnb-project

---

**This project meets industry standards and you can use it for your own goal** 
