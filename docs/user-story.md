# Product Management User Story

## Epic: Manage products through a secure full-stack application

As an inventory manager, I want to authenticate and manage products through a responsive Angular interface backed by a Clean Architecture ASP.NET Core API, so that product stock remains accurate and easy to maintain.

## Scope and decisions

- Backend: ASP.NET Core .NET 8 Web API.
- Frontend: Angular in the same repository.
- Storage: SQL Server LocalDB through Entity Framework Core.
- Product fields: `Id`, `Name`, `Price`, and `Quantity`.
- Authentication: custom Users table, hashed passwords, and JWT.

## Sub-tickets

### PM-01: Create the Clean Architecture solution
**Acceptance criteria**
- Domain, Application, Infrastructure, API, frontend, and tests have clear boundaries.
- The solution builds with `dotnet build`.
- Angular can be installed and built independently.

**Definition of Done**
- Project references follow the dependency direction.
- README documents the repository structure and local commands.

### PM-02: Persist products and users
**Acceptance criteria**
- The database contains Products and Users tables.
- Products have a unique ID, name, price, and quantity.
- User email is unique and passwords are stored only as hashes.
- Seeded products and a demo credential are available for development.

**Definition of Done**
- EF Core configuration and migrations are checked in.
- No production secrets are committed.

### PM-03: Implement product read operations
**Acceptance criteria**
- `GET /api/products` returns products.
- `GET /api/products/{id}` returns a product or `404 Not Found`.
- Anonymous users can read products.

**Definition of Done**
- Application and API tests cover success and missing-record cases.

### PM-04: Implement product create, update, and delete operations
**Acceptance criteria**
- `POST`, `PUT`, and `DELETE` use the correct HTTP verbs and status codes.
- Mutations require a valid JWT.
- Name is required, price is greater than zero, and quantity is non-negative.

**Definition of Done**
- Business validation is tested independently of EF Core.
- API tests cover validation, authorization, and CRUD outcomes.

### PM-05: Implement registration, login, and authorization
**Acceptance criteria**
- Users can register with a unique email and password.
- Users can log in and receive a JWT.
- `GET /api/auth/me` requires authentication.
- Invalid credentials are rejected without revealing sensitive details.

**Definition of Done**
- Password hashes are verified with a constant-time comparison.
- JWT key configuration is externalized for non-development environments.

### PM-06: Build the Angular product workspace
**Acceptance criteria**
- Users can sign in and sign out.
- The product list supports loading, empty, and error states.
- Authenticated users can create, edit, and delete products.
- Forms validate product name, price, and quantity.
- The layout works on desktop and mobile widths.

**Definition of Done**
- API calls are typed and centralized.
- The bearer token is attached through an HTTP interceptor.

### PM-07: Complete automated testing
**Acceptance criteria**
- Domain validation, application services, persistence, API behavior, and key Angular behavior have tests.
- Tests do not depend on production data.

**Definition of Done**
- Backend tests run with `dotnet test`.
- Frontend tests and production build run with npm scripts.

### PM-08: Document and demonstrate the application
**Acceptance criteria**
- README includes prerequisites, database setup, migrations, seed credentials, run commands, and API summary.
- The complete demo flow is documented: login, list, create, edit, and delete.

**Definition of Done**
- Documentation is reviewed against every requirement in `Exercise.md`.
