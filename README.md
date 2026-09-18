# Product Management

A full-stack exercise implementation with an ASP.NET Core .NET 8 API, Angular frontend, SQL Server LocalDB, Clean Architecture boundaries, JWT authentication, and automated tests.

## Structure

- `src/ProductManagement.Domain`: Product and User domain models.
- `src/ProductManagement.Application`: use cases, DTOs, and repository/security contracts.
- `src/ProductManagement.Infrastructure`: EF Core SQL Server persistence and security services.
- `src/ProductManagement.Api`: controllers, authentication, CORS, Swagger, and host configuration.
- `tests/`: application, infrastructure, and API tests.
- `frontend/product-management-ui`: Angular client.
- `docs/user-story.md`: epic, acceptance criteria, and sub-tickets.

## Prerequisites

- .NET 8 SDK.
- SQL Server Express LocalDB on Windows, or change the EF Core provider/connection string for another relational database.
- Node.js compatible with the Angular CLI version in `frontend/product-management-ui/package.json`.

## Run the API

```powershell
dotnet restore ProductManagement.sln
dotnet build ProductManagement.sln
dotnet run --project src/ProductManagement.Api
```

Swagger is available at the URL printed by the API. The development connection string targets `ProductManagementDb` on `(localdb)\mssqllocaldb`. The API creates the database on first start and seeds these demo products and credentials:

- Email: `demo@example.com`
- Password: `Demo123!`

## Run the Angular client

```powershell
Set-Location frontend/product-management-ui
npm install
npm start
```

The client expects the API at `https://localhost:7061/api`. Update `src/app/api.service.ts` if the API selects a different HTTPS port.

## Test

```powershell
dotnet test ProductManagement.sln
Set-Location frontend/product-management-ui
npm test -- --watch=false
npm run build
```

## API summary

- `GET /api/products` and `GET /api/products/{id}` are public read endpoints.
- `POST /api/products`, `PUT /api/products/{id}`, and `DELETE /api/products/{id}` require a JWT.
- `POST /api/auth/register` creates a user.
- `POST /api/auth/login` returns a JWT.
