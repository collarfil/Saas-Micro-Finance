# SaaS Micro Finance Platform

A multi-tenant microfinance banking application built with C#, ASP.NET Core, Entity Framework Core, SQL Server, and Finbuckle.MultiTenant.

The project explores the design of a SaaS banking platform in which multiple financial institutions can operate within a shared application architecture while maintaining tenant-specific data isolation.

> **Project status:** In progress — backend implementation is established, with testing and frontend completion remaining.

## Overview

The SaaS Micro Finance Platform is designed to support core microfinance operations through a tenant-aware backend API and a database-per-tenant architecture.

The application uses Finbuckle.MultiTenant to support tenant resolution and tenant-aware application behavior. Each tenant is intended to use its own application database, separating institution-specific operational data.

The project is being developed with an emphasis on maintainable application structure, tenant isolation, and extensible business functionality.

## Technology Stack

- **Language:** C#
- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** Microsoft SQL Server
- **Multi-tenancy:** Finbuckle.MultiTenant
- **Development environment:** Visual Studio
- **Target framework:** .NET 8

## Architecture

The solution is organized into separate projects to keep API concerns, data access, domain models, and shared utilities distinct.

| Project | Responsibility |
|---|---|
| `Saas-Micro-Finance.API` | HTTP API and application entry point |
| `Saas-Micro-Finance.DataAccess` | Data access and persistence concerns |
| `Saas-Micro-Finance.Models` | Application models and related types |
| `Saas-Micro-Finance.Utility` | Shared utilities and supporting components |

The solution is maintained in `Saas-Micro-Finance.sln`.

### Multi-Tenant Design

The application uses a database-per-tenant approach.

The intended request flow is:

1. A request identifies its tenant using the application's configured tenant-resolution mechanism.
2. The application resolves the tenant context.
3. Tenant-aware services and data access use the appropriate tenant configuration.
4. Tenant-specific operations are performed against the corresponding database.

The exact tenant-resolution configuration, database provisioning process, and isolation guarantees should be verified against the implementation before deployment.

## Key Engineering Areas

The project focuses on:

- Tenant-aware API design.
- Database-per-tenant persistence.
- Separation of API, data access, models, and shared utilities.
- Entity Framework Core integration.
- Extensible foundations for microfinance business workflows.

The implemented business features and their current test status will be documented as they are verified.

## Prerequisites

The development environment is expected to include:

- .NET 8 SDK.
- Visual Studio with the relevant .NET development workload.
- Microsoft SQL Server.
- Access to the required NuGet packages.

Additional configuration requirements will be documented after reviewing the application's startup configuration and database setup.

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/collarfil/Saas-Micro-Finance.git
cd Saas-Micro-Finance
```

### 2. Open the solution

Open `Saas-Micro-Finance.sln` in Visual Studio.

### 3. Configure the application

Configure the required database connections and tenant settings using the application's supported configuration mechanism.

Do not commit passwords, production connection strings, API keys, or other secrets to source control.

### 4. Restore dependencies

```bash
dotnet restore Saas-Micro-Finance.sln
```

### 5. Build the solution

```bash
dotnet build Saas-Micro-Finance.sln
```

### 6. Configure the database and run the API

Follow the database migration, tenant provisioning, and startup instructions documented for the verified implementation.

Database creation and migration commands will be added after confirming the contexts, startup project, and migration strategy in the repository.

## Testing

Testing is part of the ongoing development process.

This section will document the available automated tests and verified scenarios, including tenant resolution, tenant database selection, data isolation, API behavior, and relevant business workflows.

Only tests that exist and have been executed successfully will be reported as passing.

## Screenshots and Demo

Screenshots and a short demonstration video will be added as the frontend and key workflows are finalized.

The demonstration will focus on actual implemented functionality and will use safe sample data.

## Current Development Status

The project is under active development.

Current priorities include:

- Completing and validating outstanding tests.
- Finishing the frontend.
- Verifying tenant-resolution and database-isolation behavior.
- Improving setup and developer documentation.
- Preparing screenshots and a short project demonstration.

## Security Notes

- Keep credentials and connection strings out of source control.
- Use appropriate authentication and authorization for protected operations.
- Verify tenant isolation and access-control behavior before deployment.
- Do not use real customer or financial information in demonstration environments.

## Future Documentation

As implementation and verification progress, this repository will include more detail on:

- Verified business modules and API endpoints.
- Database and tenant provisioning.
- Authentication and authorization.
- Automated testing.
- Architecture decisions.
- Deployment and operational configuration.

## Author

**Collins Aigbekhai**

GitHub: [@collarfil](https://github.com/collarfil)
