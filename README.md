# N5 Employees API

A modular, testable, and scalable .NET 8 solution built following Clean Architecture principles. Designed for managing employee permissions with support for event-based communication and search capabilities using Kafka and Elasticsearch.

---

## Project Architecture

This solution is structured into four core layers:

- **Domain**: Contains core business entities and contracts.
- **Application**: Contains use cases, CQRS (MediatR), and interfaces.
- **Infrastructure**: Implements external concerns such as persistence (EF Core), Kafka producers/consumers, and Elasticsearch indexing.
- **API**: ASP.NET Core Web API exposing endpoints and handling HTTP communication.

> This separation allows for clear boundaries, testability, and independent evolution of modules.

---

## Features

- Create new permissions (`POST /api/permissions`)
- Update existing permissions (`PUT /api/permissions/{id}`)
- Search for permissions with filters (`GET /api/permissions`)
- Publish event logs to **Apache Kafka**
- Read search results from **Elasticsearch**
- Exception handling middleware
- Logging via **Serilog**

---

## Tech Stack

- [.NET 8](https://dotnet.microsoft.com/)
- [MediatR](https://github.com/jbogard/MediatR) (CQRS pattern)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) with SQL Server
- [Apache Kafka](https://kafka.apache.org/) (messaging)
- [Elasticsearch](https://www.elastic.co/elasticsearch/) (search engine)
- [Serilog](https://serilog.net/) (structured logging)
- [xUnit](https://xunit.net/) + [FluentAssertions](https://fluentassertions.com/) (unit/integration testing)
- [Docker](https://www.docker.com/) (containerization)

---

## Testing Strategy

### Unit Tests
- Implemented for all command and query handlers.
- Repository dependencies mocked using interfaces.

### Integration Tests
- Cover key operations: `CreatePermission`, `UpdatePermission`, `GetPermissions`.
- Use `CustomWebApplicationFactory` to bootstrap real HTTP calls.
- External services like Kafka and Elasticsearch are replaced with fake implementations:
  - `FakeKafkaProducer`
  - `FakeElasticSearchService`

> Tests validate application behavior from controller to handler, including HTTP status, response DTOs, and simulated side-effects.

---

## Docker Support

The solution includes a working `Dockerfile` and is fully containerized using Docker Compose for all its components.

### How It Works

To build and start all containers (API, SQL Server, Kafka, Zookeeper, and Elasticsearch), navigate to the root directory of the solution and run:

```bash
docker-compose up --build -d
```

If any container fails or shuts down (commonly due to initialization timing or port issues), reset the environment:

```bash
docker-compose down -v
```

Then retry the build and up process:

```bash
docker-compose up --build -d
```

> After a successful startup, the database will be created via EF Core migrations and all services should remain running.

You can monitor the logs and health of the services using:

```bash
docker ps

docker logs <container-name>
```

Once all services are up, access the API via browser or Swagger UI at:

```
http://localhost:<mapped-port>/swagger
```

### External Dependencies

- SQL Server
- Kafka & Zookeeper
- Elasticsearch

These can be run via Docker Compose or individually with standard images.

---

## How to Run

### Prerequisites

Ensure the following containers or services are available and accessible:

- SQL Server (`mcr.microsoft.com/mssql/server`)
- Kafka (`bitnami/kafka`)
- Elasticsearch (`docker.elastic.co/elasticsearch/elasticsearch:8.x`)

These services are typically started using Docker Compose before launching the API.

### Running the API

The application is executed via **Visual Studio IDE** using the `F5` shortcut or the "Run" button. Visual Studio builds and runs the API inside a Docker container.

Thanks to the configuration in `appsettings.json`, which uses `Server=host.docker.internal`, the API running inside Docker is able to communicate with other containers (SQL Server, Kafka, Elasticsearch) launched separately via Docker Compose.

After the application starts, EF Core applies any pending migrations automatically, and Swagger UI opens by default in the browser.

---

## Project Structure

```
N5.Employees.Api/              → ASP.NET Core Web API
N5.Employees.Application/      → CQRS, commands, queries, interfaces
N5.Employees.Domain/           → Core entities and domain models
N5.Employees.Infrastructure/   → Persistence, Kafka, Elasticsearch
N5.Employees.UnitTests/        → Unit testing (handlers, validation)
N5.Employees.IntegrationTests/ → HTTP-level tests, test doubles
```

---

## Future Improvements

> While the solution is functional and production-oriented, certain enhancements are planned or noted below:

- **Kubernetes deployment files not included**  
  > Intended for future deployment; manifests can be easily created using `Deployment` and `Service` YAML files targeting the Docker image.

- **Kafka and Elasticsearch integration tests not executed against live services**  
  > Simulated via fakes to avoid external dependencies during test execution.

- **Limited failure case testing (e.g., service outages)**  
  > Retry policies and fallback mechanisms can be added for resiliency.

---

## Author

**Joel Pacheco Vilcapoma**  \
Senior .NET Developer | Software Architect (in progress)

- GitHub: [joelpacheco](https://github.com/joelpachecov89)
- LinkedIn: [linkedin.com/in/jpachecov89](https://linkedin.com/in/jpachecov89)

---

_This solution was delivered under time constraints as part of a technical challenge. Efforts were focused on producing a clean, extensible, and testable codebase aligned with modern architectural standards._