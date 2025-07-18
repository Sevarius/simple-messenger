# SimpleMessenger

SimpleMessenger is a multi-layered messaging application solution built with .NET. It demonstrates the principles of modern messenger architecture using Domain-Driven Design (DDD), CQRS, Entity Framework Core, SignalR, and ASP.NET Core.

## Solution Overview

This solution provides a basic messenger with chat creation, message sending/receiving, user status tracking, and both web and console clients. The architecture is modular, separating domain logic, application services, data access, and user interfaces.

## Projects Structure

- **Domain**: Contains core business logic, domain entities, and aggregates. Defines the rules of the messaging domain.
- **Application**: Implements application layer logic, including CQRS handlers, services, and repository interfaces. Decoupled from infrastructure.
- **Data**: Implements data access, database context (MessengerDbContext), entity configurations, and DI extensions.
- **Data.Migrations**: Manages Entity Framework Core migrations and database schema updates.
- **Services**: Auxiliary services used across the solution (e.g., caching, dependency injection helpers).
- **Models**: DTOs and models for data transfer between layers and serialization.
- **Client**: Console client for interacting with the messenger server (sending/receiving messages, managing chats).
- **Host**: Main server project, entry point for the application, configures and runs the backend services.
- **SignalRApi**: SignalR hubs for real-time messaging between clients.
- **WebApi**: RESTful API for HTTP-based interaction with the messenger (controllers, models, endpoints).

## Getting Started

1. Restore dependencies:
   ```bash
   dotnet restore
   ```
2. Apply database migrations:
   ```bash
   dotnet ef database update --startup-project src/Host --project src/Data.Migrations
   ```
3. Run the server:
   ```bash
   dotnet run --project src/Host
   ```
4. (Optional) Run the console client:
   ```bash
   dotnet run --project src/Client
   ```

## Technologies Used
- .NET 8+
- Entity Framework Core
- SignalR
- ASP.NET Core (REST API)
- CQRS, DDD

## Notes
- For development and testing purposes.
- For questions or suggestions, please open an issue or pull request.

---

### Useful EF Core Commands

```bash
dotnet ef migrations add <Name> --startup-project src/Host --project src/Data.Migrations
```

```bash
dotnet ef database update --startup-project src/Host --project src/Data.Migrations
```
