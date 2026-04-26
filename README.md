# User Management API

ASP.NET Core Web API project for user management with CRUD endpoints, Entity Framework Core, SQLite, JWT authentication, validation, middleware, Swagger, and seed data.

## Features

- Full user CRUD with async EF Core queries
- DTOs so entity models are not exposed directly
- Data Annotation validation for required fields, email format, and password length
- Structured JSON responses
- JWT login endpoint
- `PUT` and `DELETE` user routes protected with JWT authentication
- Custom request logging middleware
- Global exception handling middleware
- Swagger/OpenAPI for testing
- SQLite database for simple local setup
- Seed admin user
- Comments in code documenting AI-assisted debugging examples for academic evaluation

## Folder Structure

```text
Controllers/
Data/
DTOs/
Exceptions/
Mapping/
Middleware/
Migrations/
Models/
Properties/
Services/
Program.cs
appsettings.json
UserManagementApi.csproj
```

## Prerequisites

- .NET 8 SDK or later

Check installation:

```bash
dotnet --version
```

## Setup and Run

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

The API will usually run at:

- `https://localhost:7000`
- `http://localhost:5000`

Swagger UI:

```text
https://localhost:7000/swagger
```

> The initial migration is included in the `Migrations/` folder. `Program.cs` also calls `Database.MigrateAsync()` on startup, so pending migrations are applied automatically when the app runs.

When you change the database model later, create a new migration:

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

## Seed Login

```json
{
  "email": "admin@example.com",
  "password": "Admin@123"
}
```

## API Endpoints

| Method | Endpoint | Auth | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/login` | No | Login and receive JWT token |
| `POST` | `/api/users` | No | Create user |
| `GET` | `/api/users` | No | Get all users |
| `GET` | `/api/users/{id}` | No | Get user by ID |
| `PUT` | `/api/users/{id}` | Yes | Update user |
| `DELETE` | `/api/users/{id}` | Yes | Delete user |

## Sample Requests

Create user:

```json
{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "password": "Strong123"
}
```

Login:

```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@example.com\",\"password\":\"Admin@123\"}"
```

Authenticated update:

```bash
curl -X PUT https://localhost:7000/api/users/1 \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Updated Name\",\"email\":\"updated@example.com\",\"password\":\"Updated123\"}"
```

## Response Shape

Successful responses use:

```json
{
  "success": true,
  "message": "User retrieved successfully.",
  "data": {}
}
```

Validation errors use:

```json
{
  "success": false,
  "message": "Validation failed.",
  "errors": {
    "Email": ["Invalid email format."]
  }
}
```

## Notes

- Passwords are stored as hashes using ASP.NET Core password hashing.
- Replace `JwtSettings:Key` before deploying.
- SQLite is used for simplicity. To switch to SQL Server, replace the EF provider package and update the connection string.
