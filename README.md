# AI-Marketplace Backend

# Related Repos: [Front Repo](https://github.com/assembakr/AI-Marketplace-FrontEnd)

AI-Marketplace is a comprehensive backend solution for an AI Services Marketplace, built with **ASP.NET Core 9** following **Clean Architecture** and **CQRS** principles. It facilitates a robust platform for vendors to sell AI models/services and for buyers to purchase and request custom AI work.

---

## 🏗️ Architecture & Patterns

The project follows the **Clean Architecture** pattern to ensure a highly maintainable, testable, and decoupled codebase.

### 🏛️ Layers
-   **API Layer (`AI-Marketplace.Api`)**: The presentation layer. It handles HTTP requests, authentication, and global exception handling.
-   **Application Layer (`AI-Marketplace.Application`)**: Contains business logic, DTOs, interfaces, and CQRS implementations (Commands and Queries).
-   **Infrastructure Layer (`AI-Marketplace.Infrastructure`)**: Handles external concerns such as database persistence (EF Core), repository implementations, identity management, and external service integrations (Stripe, Email).
-   **Domain Layer (`AI-Marketplace.Domain`)**: The core layer containing domain entities, enums, and constants.

### 🧩 Patterns Used
-   **CQRS (Command Query Responsibility Segregation)**: Implemented using **MediatR** to separate read and write operations, making the system more scalable and organized.
-   **Repository Pattern**: Abstracts data access logic behind interfaces to keep the application layer agnostic of the persistence mechanism.
-   **Dependency Injection**: Extensively used across all layers for better modularity and testing.

---

## 🛠️ Technology Stack & Tools

-   **Framework**: ASP.NET Core 9.0
-   **Database**: SQL Server with **Entity Framework Core**
-   **Mapping**: **AutoMapper** for efficient DTO-to-Entity mapping.
-   **Mediation**: **MediatR** for decoupled request/response handling.
-   **Security**: **ASP.NET Core Identity** & **JWT (JSON Web Tokens)** for secure authentication and authorization.
-   **Payments**: **Stripe SDK** integration for checkout flows and webhook processing.
-   **Mailing**: **SMTP** Email Service for system notifications.
-   **API Documentation**: **OpenAPI (Swagger)** for interactive API testing.

---

## 📂 Folder Structure

```text
BackEnd/
├── AI-Marketplace.Api/              # Entry point, Controllers, Middleware
│   ├── Controllers/                 # RESTful Endpoints (15+ Controllers)
│   ├── Middleware/                  # Custom Exception Handling
│   ├── Extentions/                  # API Configuration Extensions
│   └── Program.cs                   # Dependency Injection & Pipeline Config
├── AI-Marketplace.Application/      # Business Logic (CQRS)
│   ├── [FeatureName]/               # Folders per entity (Products, Orders, etc.)
│   │   ├── Commands/                # Create, Update, Delete Logic
│   │   ├── Queries/                 # Read & Filtering Logic
│   │   └── DTOs/                    # Data Transfer Objects
│   ├── Common/                      # Shared Interfaces, Settings, & Mappings
│   └── DependencyInjection.cs       # MediatR & AutoMapper Registration
├── AI-Marketplace.Infrastructure/   # Data Access & External Services
│   ├── Data/                        # DbContext & EF Migrations
│   ├── Repositories/                # Concrete Repository Implementations
│   ├── ExternalServices/            # Stripe, JWT, & Email implementations
│   ├── Seed/                        # Database Seeding (Roles, Initial Data)
│   └── DependencyInjection.cs       # Infrastructure Service Registration
└── AI-Marketplace.Domain/           # Core Entities & Enums
    ├── Entities/                    # Domain Models
    └── Enums/                       # Shared Enumerations
```

---

## 🚀 Key Features

-   **Multi-Role User Management**: Support for Admin, Vendor, and Buyer roles via Identity.
-   **Product Management**: Full CRUD for AI products with filtering and categorization.
-   **Advanced Ordering System**: Support for carts, orders, and master order tracking.
-   **Custom Requests & Offers**: Allows buyers to request specific AI work and vendors to make offers.
-   **Payment Integration**: Secure payment processing with Stripe and automated webhook handlers.
-   **Wishlist & Favorites**: Personalized user experience for saving products.
-   **Real-time Communication**: Chat session management for buyer-vendor interaction.
-   **Global Exception Handling**: A unified middleware that captures all errors and returns standardized JSON responses.

---

## 🛡️ Exception Handling

The system employs a custom `ExceptionHandlingMiddleware` to ensure that even in the case of failure, the API returns a consistent structure:
```json
{
  "status": 500,
  "message": "Friendly error message",
  "detail": "Detailed stack trace (Development only)"
}
```

---

## ⚡ Getting Started

### Prerequisites
-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   SQL Server

### Installation
1.  **Clone the Repo**:
    ```bash
    git clone <repository-url>
    ```
2.  **Configuration**: Update the connection string and Stripe settings in `appsettings.json`.
3.  **Database Migration**:
    ```bash
    dotnet ef database update --project AI-Marketplace.Infrastructure --startup-project AI-Marketplace.Api
    ```
4.  **Run**:
    ```bash
    dotnet run --project AI-Marketplace.Api
    ```
5.  **Documentation**: Visit `https://localhost:<port>/swagger` to explore the API.
