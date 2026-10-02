# Inventory Management API

This is a backend project I built to learn and practice ASP.NET Core Web API development. The goal was to build a proper REST API from scratch with a database, authentication, validation, and automated tests, rather than just following a basic tutorial.

## What This Project Does

The API manages products and their categories for a simple inventory system:
- **Products:** Create, update details, view, and soft-delete products.
- **Categories:** Group products into categories and prevent deleting categories that still have products.
- **Stock Management:** Dedicated endpoints to increase and decrease stock, ensuring stock cannot go below zero.
- **Authentication & Roles:** Users can register and log in using JWT tokens. There are two roles: Admin (can modify catalog and stock) and User (read-only access).

## Tech Stack & Tools

- C# and .NET 10
- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- JWT Bearer Authentication
- xUnit and Moq for unit and integration testing
- Swagger / OpenAPI for testing endpoints in the browser

## Project Structure

I organized the project into folders to keep responsibilities separated:
- **Controllers:** Handle incoming HTTP requests and return appropriate status codes.
- **Services:** Contain business rules, validation logic, and mapping between DTOs and entities.
- **Repositories:** Handle database queries and persistence using EF Core.
- **Entities:** Database models (Product, Category, User).
- **DTOs:** Request and response models to avoid exposing database entities directly.
- **Data:** AppDbContext and EF Core migrations.

## Getting Started

### Prerequisites

- .NET 10 SDK installed
- SQL Server running locally (or LocalDB / Docker)

### 1. Database Connection & Secrets

Configure your database connection and JWT secret key using .NET User Secrets:

```bash
cd api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=InventoryManagementDb;User Id=SA;Password=YourPassword;TrustServerCertificate=True;"
dotnet user-secrets set "JwtSettings:Secret" "YourSuperSecretKeyThatIsLongEnough123!"
```

### 2. Run Database Migrations

Apply EF Core migrations to create the database tables:

```bash
dotnet ef database update --project api
```

### 3. Run the API

```bash
dotnet run --project api
```

Once running, you can open Swagger in development at `https://localhost:<port>/swagger` to try out the endpoints.

### 4. Run Tests

To run the automated test suite:

```bash
dotnet test
```

## API Endpoints Overview

### Auth
- `POST /api/auth/register` - Register a new account
- `POST /api/auth/login` - Log in and get a JWT token

### Products
- `GET /api/product/getAllProducts` - Get products with pagination, search, sorting, and category filtering
- `GET /api/product/getProductById/{id}` - Get a single product by ID
- `POST /api/product/addProduct` - Add a product (Admin only)
- `PUT /api/product/updateProduct/{id}` - Update product name, description, price, or category (Admin only)
- `DELETE /api/product/deleteProduct/{id}` - Soft-delete a product by marking it inactive (Admin only)
- `PATCH /api/product/stock/increase/{id}` - Add stock to a product (Admin only)
- `PATCH /api/product/stock/decrease/{id}` - Reduce stock, preventing negative inventory (Admin only)

### Categories
- `GET /api/category/getAllCategories` - Get list of categories
- `GET /api/category/getCategoryById/{id}` - Get single category by ID
- `POST /api/category/addCategory` - Create a new category (Admin only)
- `PUT /api/category/updateCategory` - Update category name (Admin only)
- `DELETE /api/category/deleteCategory/{id}` - Delete a category if no products belong to it (Admin only)

## What I Learned Building This

- How to structure an ASP.NET Core API with Controllers, Services, and Repositories.
- Managing database schema and foreign keys with EF Core migrations.
- Working with JWT tokens, claims, and role-based authorization attributes.
- Writing unit tests with Moq to test business rules (like preventing negative stock) in isolation.
- Handling errors centrally using ASP.NET Core's IExceptionHandler and ProblemDetails.