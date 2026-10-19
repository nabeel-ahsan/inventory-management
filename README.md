# Inventory Management API

A RESTful backend API for managing product catalogs, categories, user access, and inventory stock. Built with ASP.NET Core and SQL Server, the API includes role-based authorization, input validation, centralized error handling, pagination, and automated tests.

## Features

- **Product management:** Create, retrieve, update, and soft-delete products.
- **Category management:** Organize products into categories and prevent deletion of categories that still contain products.
- **Inventory operations:** Dedicated endpoints for increasing and decreasing stock, with database-level safeguards against negative inventory and concurrent stock updates.
- **Authentication and authorization:** User registration and login using JWT bearer tokens, with role-based access control for administrative operations.
- **Search and filtering:** Search products and filter by category, with configurable pagination and sorting.
- **Validation and error handling:** Validate incoming requests and return consistent error responses using ASP.NET Core's exception-handling infrastructure and Problem Details.
- **Automated testing:** Unit tests for business rules and integration tests for API behavior, authentication, and authorization.
- **API documentation:** Interactive endpoint exploration through Swagger/OpenAPI.

## Technology Stack

- **Language:** C#
- **Framework:** .NET 10, ASP.NET Core Web API
- **Database:** SQL Server
- **Data access:** Entity Framework Core
- **Authentication:** JWT Bearer Authentication
- **Testing:** xUnit, Moq, WebApplicationFactory
- **API documentation:** Swagger / OpenAPI

## Architecture

The application uses a layered structure to separate HTTP handling, business logic, and data persistence.

```text
HTTP Request
     |
     v
Controllers
     |
     v
Services
     |
     v
Repositories
     |
     v
Entity Framework Core
     |
     v
SQL Server
```

- **Controllers:** Handle HTTP requests and translate application results into HTTP responses.
- **Services:** Implement business rules, coordinate operations, and map between entities and DTOs.
- **Repositories:** Encapsulate database queries and persistence operations.
- **Entities:** Represent the core data models: Product, Category, and User.
- **DTOs:** Define request and response contracts independently of database entities.
- **Data:** Contains the EF Core `AppDbContext` and database migrations.

This structure keeps responsibilities separated without introducing unnecessary architectural complexity.

## Key Engineering Decisions

- **Soft deletion:** Products are marked inactive rather than physically deleted, preserving their database records.
- **Stock integrity:** Stock decreases use a conditional database update so the stock check and decrement happen atomically, preventing concurrent requests from overselling available inventory through this operation.
- **Category deletion rules:** Categories cannot be deleted while products reference them.
- **DTO boundaries:** Request and response models prevent the API from exposing database entities directly.
- **Role-based access:** Catalog modifications and stock operations are restricted to administrative users.
- **Centralized exception handling:** Unexpected errors are handled centrally instead of duplicating error-handling logic across endpoints.
- **Configuration and secrets:** Connection strings and JWT signing secrets are configured through .NET User Secrets for local development.

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server instance accessible from the application
- EF Core CLI tools, if not already installed

### 1. Configure secrets

From the API project directory, configure the database connection string and JWT signing secret:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "JwtSettings:Secret" "YOUR_LONG_RANDOM_SECRET"
```

Use a database connection string appropriate for your SQL Server setup. Do not commit real credentials or signing secrets to source control.

### 2. Apply database migrations

From the repository root:

```bash
dotnet ef database update --project api
```

Ensure the configured database exists or that your SQL Server setup permits the required database creation.

### 3. Run the API

```bash
dotnet run --project api
```

In the Development environment, open the Swagger UI at the HTTPS URL shown in the application output, followed by `/swagger`.

### 4. Run the tests

From the repository root:

```bash
dotnet test
```

## API Endpoints

### Authentication

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/auth/register` | Register a user |
| POST | `/api/auth/login` | Authenticate and obtain a JWT |

### Products

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/product/getAllProducts` | Retrieve products with pagination, search, sorting, and category filtering |
| GET | `/api/product/getProductById/{id}` | Retrieve a product by ID |
| POST | `/api/product/addProduct` | Create a product — Admin |
| PUT | `/api/product/updateProduct/{id}` | Update product details — Admin |
| DELETE | `/api/product/deleteProduct/{id}` | Soft-delete a product — Admin |
| PATCH | `/api/product/stock/increase/{id}` | Increase stock — Admin |
| PATCH | `/api/product/stock/decrease/{id}` | Decrease stock — Admin |

### Categories

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/category/getAllCategories` | Retrieve categories |
| GET | `/api/category/getCategoryById/{id}` | Retrieve a category by ID |
| POST | `/api/category/addCategory` | Create a category — Admin |
| PUT | `/api/category/updateCategory` | Update a category — Admin |
| DELETE | `/api/category/deleteCategory/{id}` | Delete a category when no products reference it — Admin |

## Testing

The test suite uses xUnit and Moq for isolated service-level tests, along with `WebApplicationFactory` for integration tests.

Coverage focuses on meaningful application behavior, including:

- Product retrieval and missing-resource handling
- Category deletion business rules
- Authentication and authorization responses
- Administrative access to protected operations
- API response behavior for representative success and failure cases

Run the complete suite using `dotnet test`.

## Scope

This project focuses on core inventory-management workflows and backend engineering fundamentals. It demonstrates layered application structure, relational data access, authentication, authorization, API validation, concurrency-aware stock updates, and automated testing.

It does not attempt to implement a full enterprise inventory platform with distributed services, complex warehouse workflows, or large-scale infrastructure.
