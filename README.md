::: {align="center"}
# SaaS Micro Finance Platform

**A multi-tenant microfinance banking platform built with ASP.NET Core,
Entity Framework Core, SQL Server and Finbuckle.MultiTenant.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![SQL
Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Multi-tenancy](https://img.shields.io/badge/Architecture-Database%20per%20Tenant-2563EB)](https://www.finbuckle.com/MultiTenant)
[![Status](https://img.shields.io/badge/Status-In%20Progress-orange)](#project-status)
:::

------------------------------------------------------------------------

## Table of Contents

-   [Overview](#overview)
-   [Project Goals](#project-goals)
-   [Key Capabilities](#key-capabilities)
-   [Technology Stack](#technology-stack)
-   [Architecture](#architecture)
    -   [System Context](#system-context)
    -   [Layered Architecture](#layered-architecture)
    -   [Project Dependencies](#project-dependencies)
-   [Multi-Tenancy](#multi-tenancy)
    -   [Tenant Resolution](#tenant-resolution)
    -   [Database Isolation](#database-isolation)
    -   [Request Lifecycle](#request-lifecycle)
-   [Solution and Folder Structure](#solution-and-folder-structure)
-   [Database Design](#database-design)
    -   [Database Separation](#database-separation)
    -   [Tenant Database ER Diagram](#tenant-database-er-diagram)
    -   [Core Entity Responsibilities](#core-entity-responsibilities)
-   [Operational Workflows](#operational-workflows)
    -   [Tenant-aware API Request](#tenant-aware-api-request)
    -   [Customer and Employee Identity
        Provisioning](#customer-and-employee-identity-provisioning)
    -   [Account and Loan Operations](#account-and-loan-operations)
-   [Getting Started](#getting-started)
-   [Configuration and Secrets](#configuration-and-secrets)
-   [Entity Framework Core
    Migrations](#entity-framework-core-migrations)
-   [Running the Application](#running-the-application)
-   [Testing and Quality](#testing-and-quality)
-   [Security Considerations](#security-considerations)
-   [Deployment and Operations](#deployment-and-operations)
-   [Project Status and Roadmap](#project-status-and-roadmap)
-   [Contributing](#contributing)
-   [License](#license)
-   [Author](#author)

------------------------------------------------------------------------

## Overview

The **SaaS Micro Finance Platform** is a multi-tenant banking
application designed to allow multiple microfinance institutions to
operate through a shared application while keeping each institution's
operational data logically and physically separated at the database
level.

The platform uses a **database-per-tenant** approach. Each institution
is represented as a tenant, and requests are associated with the
appropriate tenant before tenant-specific data is accessed.
Finbuckle.MultiTenant provides the tenant-resolution and tenant-context
integration.

The backend is built with ASP.NET Core Web API, Entity Framework Core,
SQL Server and ASP.NET Core Identity. The solution is organised into
separate projects to keep API, persistence, models and shared utilities
responsibilities distinct.

### Project objectives

-   Provide a reusable software foundation for multiple microfinance
    institutions.
-   Isolate each institution's data using a dedicated database.
-   Support tenant-specific users, roles and access control.
-   Establish maintainable API and data-access boundaries.
-   Provide a foundation for customer, employee, account and lending
    workflows.
-   Prepare the application for a separately developed frontend and
    eventual cloud deployment.

> **Project status:** Work in progress. The backend foundation and
> multi-tenant data-access approach have been established. Frontend
> completion, testing, deployment and validation of the full business
> workflows remain part of the delivery plan. Refer to the roadmap for
> current scope.

## Project Goals

  -----------------------------------------------------------------------
  Goal                                Engineering approach
  ----------------------------------- -----------------------------------
  Tenant isolation                    Dedicated SQL Server database for
                                      each tenant

  Tenant identification               Finbuckle.MultiTenant and the
                                      configured tenant identifier

  Authentication and identity         ASP.NET Core Identity

  Data persistence                    Entity Framework Core and SQL
                                      Server

  Maintainability                     Layered solution with separate
                                      projects

  Extensibility                       Domain model that can support
                                      additional banking and lending
                                      modules

  Operational readiness               Configuration externalisation,
                                      testing and deployment
                                      documentation
  -----------------------------------------------------------------------

## Key Capabilities

The following table distinguishes the established technical foundation
from business capabilities that still require end-to-end verification.

  -----------------------------------------------------------------------
  Area                                Current scope
  ----------------------------------- -----------------------------------
  Multi-tenancy                       Finbuckle.MultiTenant integration

  Tenant persistence                  Database-per-tenant design

  API                                 ASP.NET Core REST API

  ORM                                 Entity Framework Core

  Database                            Microsoft SQL Server

  Identity                            ASP.NET Core Identity integration

  Domain model                        Customer, employee and
                                      microfinance-related entities

  Frontend                            Separate frontend work; completion
                                      in progress

  Automated verification              Test coverage and tenant-isolation
                                      verification to be completed
  -----------------------------------------------------------------------

Do not interpret the presence of a domain entity as proof that its
complete business workflow is production-ready.

## Technology Stack

  -----------------------------------------------------------------------
  Component                           Technology
  ----------------------------------- -----------------------------------
  Programming language                C# 12

  Backend framework                   ASP.NET Core Web API

  Runtime                             .NET 8

  ORM                                 Entity Framework Core

  Relational database                 Microsoft SQL Server

  Multi-tenancy                       Finbuckle.MultiTenant

  Identity management                 ASP.NET Core Identity

  Development environment             Visual Studio

  API documentation                   Swagger / OpenAPI, subject to the
                                      configured application setup
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# Architecture

## System Context

The system accepts requests from a web or mobile client. The API
identifies the tenant associated with the request, establishes the
tenant context and uses the tenant-specific database for application
data.

``` mermaid
flowchart LR
    U["Web / Mobile Client"]
    API["ASP.NET Core Web API"]

    subgraph APP["SaaS Micro Finance Application"]
        MW["Middleware and Request Pipeline"]
        TEN["Finbuckle.MultiTenant<br/>Tenant Resolution"]
        CTRL["API Controllers"]
        DAL["Data Access Layer"]
        ID["ASP.NET Core Identity"]
        MW --> TEN
        TEN --> CTRL
        CTRL --> DAL
        CTRL --> ID
    end

    subgraph DB["SQL Server"]
        CAT[("Tenant Catalog / Registry")]
        T1[("Tenant A Database")]
        T2[("Tenant B Database")]
        TN[("Tenant N Database")]
    end

    U -->|"HTTPS request + tenant identifier"| API
    API --> MW
    TEN -.->|"Resolve tenant"| CAT
    DAL --> T1
    DAL --> T2
    DAL --> TN
```

The diagram represents the intended request and data flow. The actual
middleware order, tenant store implementation and Identity context
registration should be confirmed against the current application
configuration.

## Layered Architecture

The solution separates HTTP concerns, persistence, shared models and
utilities.

``` mermaid
flowchart TB
    CLIENT["API Consumer"]

    subgraph PRESENTATION["Presentation Layer"]
        API["Saas-Micro-Finance.API<br/>Controllers · Middleware · DI · Startup"]
    end

    subgraph PERSISTENCE["Persistence Layer"]
        DAL["Saas-Micro-Finance.DataAccess<br/>DbContexts · Repositories · Migrations"]
    end

    subgraph SHARED["Shared Projects"]
        MODELS["Saas-Micro-Finance.Models<br/>Entities · DTOs · Models"]
        UTILITY["Saas-Micro-Finance.Utility<br/>Helpers · Constants · Shared Utilities"]
    end

    DB[("SQL Server Databases")]

    CLIENT --> API
    API --> DAL
    API --> MODELS
    API --> UTILITY
    DAL --> MODELS
    DAL --> UTILITY
    DAL --> DB
```

### Layer responsibilities

  -----------------------------------------------------------------------
  Project                             Responsibility
  ----------------------------------- -----------------------------------
  `Saas-Micro-Finance.API`            Application entry point,
                                      controllers, HTTP pipeline,
                                      dependency injection and API
                                      configuration

  `Saas-Micro-Finance.DataAccess`     Entity Framework Core contexts,
                                      migrations, repositories and
                                      database access

  `Saas-Micro-Finance.Models`         Domain entities, DTOs and shared
                                      application models

  `Saas-Micro-Finance.Utility`        Shared helper classes, constants
                                      and reusable utilities
  -----------------------------------------------------------------------

The diagram describes the solution's architectural responsibilities. It
is not a substitute for checking actual `.csproj` references.

## Project Dependencies

``` mermaid
flowchart TD
    API["Saas-Micro-Finance.API"]
    DAL["Saas-Micro-Finance.DataAccess"]
    MOD["Saas-Micro-Finance.Models"]
    UTIL["Saas-Micro-Finance.Utility"]

    API --> DAL
    API --> MOD
    API --> UTIL
    DAL --> MOD
    DAL --> UTIL
```

------------------------------------------------------------------------

# Multi-Tenancy

## Tenant Resolution

The application uses **Finbuckle.MultiTenant** to associate an incoming
request with a tenant.

The tenant identifier is supplied through the configured
tenant-resolution strategy. In the current project setup, the tenant
header convention is:

``` http
__tenant__: firstbank
```

The header value is an example tenant identifier; it must match a tenant
registered in the tenant store.

Example request:

``` http
GET /api/customers
Host: localhost:5001
__tenant__: firstbank
Authorization: Bearer <access-token>
```

The exact endpoint and authentication requirements depend on the
controller configuration.

> A tenant identifier is not an authentication credential. The server
> must still authenticate the caller and authorise access to the
> requested operation. Do not allow a client-supplied tenant value alone
> to grant access to another institution's data.

## Database Isolation

The platform follows a **database-per-tenant** model.

``` mermaid
flowchart TB
    APP["Shared ASP.NET Core Application"]
    STORE[("Tenant Registry / Catalog")]

    APP --> STORE

    STORE --> A["Tenant: firstbank"]
    STORE --> B["Tenant: union"]
    STORE --> C["Tenant: citi"]
    STORE --> D["Tenant: palmpay"]
    STORE --> E["Tenant: sterlin"]

    A --> DB1[("SaasBank_firstbank")]
    B --> DB2[("SaasBank_union")]
    C --> DB3[("SaasBank_citi")]
    D --> DB4[("SaasBank_palmpay")]
    E --> DB5[("SaasBank_sterlin")]
```

The database names above reflect the project's development tenant
examples. They are not credentials or connection strings.

  -----------------------------------------------------------------------
  Component                           Purpose
  ----------------------------------- -----------------------------------
  Tenant registry                     Stores tenant identification and
                                      configuration used during tenant
                                      resolution

  `AdminDbContext`                    Represents the master/admin
                                      database context

  `SaasBankDbContext`                 Represents tenant-specific
                                      application data

  Tenant database                     Stores the operational data for an
                                      individual institution
  -----------------------------------------------------------------------

### Isolation responsibilities

-   Resolve the tenant before accessing tenant-scoped data.
-   Use the resolved tenant's connection configuration when creating the
    tenant DbContext.
-   Keep tenant records and tenant-specific Identity data within the
    intended database boundary.
-   Ensure repositories and services use the correct tenant-scoped
    DbContext.
-   Test that a user associated with one tenant cannot access another
    tenant's records.
-   Protect tenant connection strings and administrative operations.

## Request Lifecycle

``` mermaid
sequenceDiagram
    autonumber
    participant Client
    participant API as ASP.NET Core API
    participant MT as Finbuckle Tenant Resolver
    participant Store as Tenant Store
    participant Context as SaasBankDbContext
    participant DB as Tenant SQL Database

    Client->>API: HTTP request + __tenant__ header
    API->>MT: Resolve tenant
    MT->>Store: Find tenant identifier
    Store-->>MT: Tenant information
    MT-->>API: Tenant context established
    API->>Context: Execute tenant-scoped operation
    Context->>DB: Execute SQL query
    DB-->>Context: Query result
    Context-->>API: Application data
    API-->>Client: HTTP response
```

------------------------------------------------------------------------

# Solution and Folder Structure

The solution contains four primary projects. The structure below
documents the responsibilities and expected locations of the main
components. Some subfolders are illustrative; keep this section aligned
with the actual repository as it evolves.

``` text
Saas-Micro-Finance/
│
├── Saas-Micro-Finance.sln
├── README.md
├── .gitignore
│
├── Saas-Micro-Finance.API/
│   ├── Controllers/
│   ├── Middleware/              # If present
│   ├── Swagger/                 # If present
│   ├── Program.cs
│   ├── appsettings.json
│   └── Saas-Micro-Finance.API.csproj
│
├── Saas-Micro-Finance.DataAccess/
│   ├── Data/
│   │   ├── AdminDbContext.cs
│   │   └── SaasBankDbContext.cs
│   ├── Migrations/
│   ├── Repository/
│   └── Saas-Micro-Finance.DataAccess.csproj
│
├── Saas-Micro-Finance.Models/
│   ├── Entities/
│   ├── DTOs/
│   └── Saas-Micro-Finance.Models.csproj
│
└── Saas-Micro-Finance.Utility/
    ├── Constants/                # If present
    ├── Helpers/                  # If present
    └── Saas-Micro-Finance.Utility.csproj
```

### Folder and component guide

  -----------------------------------------------------------------------
  Folder / file                       Responsibility
  ----------------------------------- -----------------------------------
  `Controllers/`                      Receives HTTP requests and returns
                                      API responses

  `Data/`                             Contains database context
                                      definitions

  `Migrations/`                       Contains EF Core migration history
                                      and generated schema changes

  `Repository/`                       Encapsulates data-access operations
                                      and repository abstractions

  `Entities/`                         Contains persistent domain entities

  `DTOs/`                             Defines data contracts for API
                                      requests and responses

  `Program.cs`                        Configures services, middleware,
                                      Identity, database contexts and
                                      tenant integration

  `appsettings.json`                  Holds non-secret application
                                      configuration
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# Database Design

## Database Separation

The database design separates tenant registration and administration
from institution-specific operational data.

``` mermaid
flowchart TB
    subgraph MASTER["Master / Administrative Database"]
        ADMIN["AdminDbContext"]
        REG["Tenant Registry"]
        CONFIG["Tenant Configuration"]
        ADMIN --> REG
        ADMIN --> CONFIG
    end

    subgraph TENANTS["Tenant Databases"]
        T1[("SaasBank_firstbank")]
        T2[("SaasBank_union")]
        T3[("SaasBank_citi")]
        TN[("Other Tenant DBs")]
    end

    APP["Shared Application"]
    APP --> ADMIN
    APP --> T1
    APP --> T2
    APP --> T3
    APP --> TN
```

The master database is responsible for the administrative/tenant
registry concerns represented by the application's `AdminDbContext`.
Each tenant database is accessed through `SaasBankDbContext`.

The precise table names and columns in the master database should be
taken from its actual entity and migration definitions. They are
intentionally not fabricated here.

## Tenant Database ER Diagram

The following is a **conceptual domain relationship diagram** based on
the project entities and microfinance modules described for this
repository. It is intended to communicate the domain, not to claim that
every relationship or column has already been implemented exactly as
drawn.

``` mermaid
erDiagram
    ASPNETUSERS ||--o| CUSTOMER : "identity link"
    ASPNETUSERS ||--o| EMPLOYEE : "identity link"
    ASPNETUSERS ||--o{ ASPNETUSERROLES : assigned
    ASPNETROLES ||--o{ ASPNETUSERROLES : grants

    CUSTOMER ||--o{ KYC : has
    CUSTOMER ||--o{ ACCOUNT : owns
    ACCOUNTTYPE ||--o{ ACCOUNT : classifies
    CUSTOMER ||--o{ LOAN : requests
    LOANPRODUCT ||--o{ LOAN : defines
    LOAN ||--o{ LOANREPAYMENT : receives

    BRANCH ||--o{ EMPLOYEE : assigns
    BRANCH ||--o{ ACCOUNT : services

    ASPNETUSERS {
        string Id PK
        string UserName
        string Email
        string PasswordHash
    }

    ASPNETROLES {
        string Id PK
        string Name
    }

    ASPNETUSERROLES {
        string UserId FK
        string RoleId FK
    }

    CUSTOMER {
        int Id PK
        string ApplicationUserId FK
    }

    EMPLOYEE {
        int Id PK
        string ApplicationUserId FK
        int BranchId FK
    }

    KYC {
        int Id PK
        int CustomerId FK
    }

    ACCOUNTTYPE {
        int Id PK
        string Name
    }

    ACCOUNT {
        int Id PK
        int CustomerId FK
        int AccountTypeId FK
        int BranchId FK
        string AccountNumber
    }

    LOANPRODUCT {
        int Id PK
        string Name
    }

    LOAN {
        int Id PK
        int CustomerId FK
        int LoanProductId FK
    }

    LOANREPAYMENT {
        int Id PK
        int LoanId FK
    }

    BRANCH {
        int Id PK
        string Name
    }
```

### Important schema notes

-   `ApplicationUserId` represents the intended relationship between a
    domain customer or employee record and an ASP.NET Core Identity
    user.
-   Identity tables are shown conceptually. Their actual names and
    columns are controlled by the Identity model configuration.
-   The diagram omits many implementation-specific properties, indexes,
    constraints and audit fields for readability.
-   The cardinalities and foreign-key names must be checked against the
    actual EF Core entity configurations and migrations before treating
    this as the definitive physical schema.
-   Do not use this conceptual ERD as a replacement for a generated
    database diagram.

## Core Entity Responsibilities

  -----------------------------------------------------------------------
  Entity / module                     Responsibility
  ----------------------------------- -----------------------------------
  `ApplicationUser`                   Authentication identity for a user

  `Customer`                          Customer profile and
                                      customer-related information

  `Employee`                          Staff profile and organisational
                                      assignment

  `KYC`                               Customer identity and verification
                                      information

  `Branch`                            Institutional branch information

  `Account`                           Customer account record

  `AccountType`                       Classification of account products

  `Loan`                              Customer loan record

  `LoanProduct`                       Loan product configuration

  `LoanRepayment`                     Repayment records associated with a
                                      loan

  Payment / subscription entities     Payment and subscription-related
                                      functionality, subject to the
                                      implemented model

  Audit entities                      Records of relevant application
                                      activities, subject to the
                                      implemented model
  -----------------------------------------------------------------------

### Database engineering considerations

1.  **Primary and foreign keys:** Define relationships explicitly and
    ensure FK properties use compatible types.
2.  **Identity linkage:** When creating a customer or employee that
    requires a login, provision the Identity user and domain record
    consistently. Use a transaction where the participating stores
    support it.
3.  **Monetary precision:** Use appropriate `decimal` precision and
    scale for financial amounts. Avoid floating-point types for money.
4.  **Indexes:** Index frequently queried fields such as account
    numbers, tenant identifiers and relevant foreign keys.
5.  **Auditability:** Financial operations should retain sufficient
    information to investigate who performed an action and when.
6.  **Data integrity:** Use database constraints and application-level
    validation together.
7.  **Migrations:** Apply schema changes to the correct context and the
    intended tenant database.

------------------------------------------------------------------------

# Operational Workflows

The diagrams in this section describe the expected application-level
workflows. They should be refined as the corresponding service and
controller implementations are verified.

## Tenant-aware API Request

``` mermaid
flowchart TD
    START([Incoming API Request])
    HEADER["Read configured tenant identifier"]
    RESOLVE["Resolve tenant using Finbuckle"]
    FOUND{"Tenant found?"}
    CONTEXT["Establish tenant context"]
    AUTH["Authenticate and authorise caller"]
    DB["Use tenant-specific DbContext"]
    EXEC["Execute requested operation"]
    RESPONSE["Return API response"]
    REJECT["Reject request"]

    START --> HEADER --> RESOLVE --> FOUND
    FOUND -- No --> REJECT
    FOUND -- Yes --> CONTEXT --> AUTH
    AUTH --> DB --> EXEC --> RESPONSE
```

A production implementation should reject unresolved tenants and
unauthorised cross-tenant access. Exact HTTP status codes should follow
the API's exception and authentication configuration.

## Customer and Employee Identity Provisioning

``` mermaid
flowchart TD
    START([Create Customer or Employee])
    VALIDATE["Validate submitted data"]
    USER["Create ASP.NET Identity user"]
    USEROK{"Identity user created?"}
    DOMAIN["Create corresponding domain record"]
    DOMAINOK{"Domain record saved?"}
    COMMIT["Commit transaction / persist changes"]
    SUCCESS["Return success response"]
    FAIL["Handle error and rollback where supported"]

    START --> VALIDATE --> USER --> USEROK
    USEROK -- No --> FAIL
    USEROK -- Yes --> DOMAIN --> DOMAINOK
    DOMAINOK -- No --> FAIL
    DOMAINOK -- Yes --> COMMIT --> SUCCESS
```

The diagram describes the intended consistency requirement. The actual
transaction boundary must account for the contexts and database
connections used by the implementation.

## Account and Loan Operations

``` mermaid
flowchart TD
    subgraph ACCOUNT["Account Lifecycle"]
        A1([Customer requests account])
        A2["Validate customer and account details"]
        A3["Create account using account type"]
        A4["Persist account in tenant database"]
        A5([Account available for authorised operations])
        A1 --> A2 --> A3 --> A4 --> A5
    end

    subgraph LOAN["Loan Lifecycle"]
        L1([Customer submits loan request])
        L2["Validate customer and loan product"]
        L3["Assess and process loan request"]
        L4{"Approved?"}
        L5["Record loan and applicable terms"]
        L6["Record repayment activity"]
        L7([Loan lifecycle updated])
        L8([Reject or return request])

        L1 --> L2 --> L3 --> L4
        L4 -- Yes --> L5 --> L6 --> L7
        L4 -- No --> L8
    end
```

This is a high-level domain workflow, not a claim that a complete
accounting ledger, automated loan assessment, approval engine or
repayment schedule has already been delivered.

------------------------------------------------------------------------

# Getting Started

## Prerequisites

Install the following before running the application:

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   [Visual Studio](https://visualstudio.microsoft.com/) with the
    ASP.NET and web development workload
-   Microsoft SQL Server (local instance or remote server)
-   Git
-   Entity Framework Core CLI tools, if using `dotnet ef`

Check the installed SDK:

``` bash
dotnet --list-sdks
```

Install or update the EF Core CLI tool if required:

``` bash
dotnet tool install --global dotnet-ef
```

If already installed:

``` bash
dotnet tool update --global dotnet-ef
```

## Clone the Repository

``` bash
git clone https://github.com/collarfil/Saas-Micro-Finance.git

cd Saas-Micro-Finance
```

## Restore Dependencies

``` bash
dotnet restore Saas-Micro-Finance.sln
```

## Build the Solution

``` bash
dotnet build Saas-Micro-Finance.sln
```

Resolve build errors before applying migrations or running the API.

# Configuration and Secrets

Application configuration should be supplied through
environment-specific configuration, environment variables or .NET User
Secrets.

Typical configuration responsibilities include:

  -----------------------------------------------------------------------
  Configuration                       Purpose
  ----------------------------------- -----------------------------------
  Master database connection          Connects to the administrative
                                      database

  Tenant database configuration       Provides tenant-specific database
                                      connections

  Identity options                    Password, lockout and user settings

  Authentication settings             Token validation and signing
                                      configuration, where applicable

  Logging                             Application and diagnostic logging

  External integrations               Credentials for configured payment
                                      or other providers
  -----------------------------------------------------------------------

### User Secrets for local development

From the API project directory:

``` bash
cd Saas-Micro-Finance.API

dotnet user-secrets init
```

Set a connection string using the configuration key expected by your
application:

``` bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

The key above is an example. Use the exact keys consumed by `Program.cs`
and the DbContext registration.

### Environment variables

.NET configuration supports environment variables. For example:

``` bash
ConnectionStrings__DefaultConnection="<your-connection-string>"
```

Use the correct configuration key for each context.

**Never commit database passwords, API keys, access tokens, private
certificates or signing keys to GitHub.** Keep production secrets in an
appropriate secret manager.

------------------------------------------------------------------------

# Entity Framework Core Migrations

This solution uses separate contexts for administrative and
tenant-specific data:

-   `AdminDbContext` --- administrative/master database.
-   `SaasBankDbContext` --- tenant application database.

Migrations must target the correct context and database.

## Package Manager Console

The following is the Visual Studio Package Manager Console style:

``` powershell
Add-Migration InitialCreate -Context SaasBankDbContext
```

Apply the migration:

``` powershell
Update-Database -Context SaasBankDbContext
```

For the administrative context:

``` powershell
Add-Migration InitialAdminCreate -Context AdminDbContext
Update-Database -Context AdminDbContext
```

Run these commands from the solution/package-manager context configured
for the DataAccess project. If the Package Manager Console's default
project or startup project differs, select the appropriate projects
first.

## .NET CLI alternative

The equivalent CLI commands require the actual project paths and startup
configuration:

``` bash
dotnet ef migrations add InitialCreate \
  --context SaasBankDbContext \
  --project Saas-Micro-Finance.DataAccess \
  --startup-project Saas-Micro-Finance.API
```

Apply the migration:

``` bash
dotnet ef database update \
  --context SaasBankDbContext \
  --project Saas-Micro-Finance.DataAccess \
  --startup-project Saas-Micro-Finance.API
```

> **Important for database-per-tenant deployments:** Do not assume that
> running one migration command automatically updates every tenant
> database. Tenant provisioning and migration orchestration must
> explicitly apply the appropriate migrations to each tenant database.
> Validate this process before production use.

------------------------------------------------------------------------

# Running the Application

From the repository root:

``` bash
dotnet run --project Saas-Micro-Finance.API
```

Use the URL printed by the application in the terminal. If Swagger is
enabled for the selected environment, open its configured Swagger
endpoint to inspect and exercise the available API endpoints.

For a tenant-aware request, include the configured tenant identifier:

``` http
__tenant__: firstbank
```

A successful request also requires a valid authentication context when
the endpoint is protected.

------------------------------------------------------------------------

# Testing and Quality

Run the solution's test suite:

``` bash
dotnet test Saas-Micro-Finance.sln
```

The following are important verification areas for this architecture:

  -----------------------------------------------------------------------
  Test area                           What to verify
  ----------------------------------- -----------------------------------
  Tenant resolution                   A valid tenant identifier resolves
                                      to the intended tenant

  Unknown tenant                      An unregistered tenant is rejected
                                      safely

  Database selection                  The correct tenant database is used

  Tenant isolation                    Data belonging to one tenant is
                                      inaccessible to another

  Identity                            User creation, login and role
                                      assignment behave as expected

  Relationships                       Customer/employee Identity foreign
                                      keys are valid

  Persistence                         EF Core mappings and migrations
                                      apply successfully

  API validation                      Invalid requests are rejected
                                      consistently

  Financial data                      Monetary precision and data
                                      integrity are preserved
  -----------------------------------------------------------------------

Only report tests as passing when they have actually been implemented
and executed successfully.

------------------------------------------------------------------------

# Security Considerations

Financial applications require particular attention to identity,
authorisation, data integrity and auditability.

### Authentication and authorisation

-   Authenticate users before granting access to protected operations.
-   Enforce authorisation on the server, not only in the frontend.
-   Apply least-privilege role and permission assignments.
-   Protect administrative endpoints separately from tenant-facing
    operations.
-   Do not treat the tenant header as proof of a user's right to access
    that tenant.

### Tenant isolation

-   Resolve tenant context consistently for every tenant-scoped request.
-   Ensure all tenant-specific queries use the correct tenant DbContext.
-   Test attempts to access records belonging to another tenant.
-   Protect tenant provisioning and connection configuration.
-   Avoid logging sensitive tenant or customer information
    unnecessarily.

### Application and database security

-   Validate and normalise incoming data.
-   Use EF Core parameterised operations and avoid unsafe dynamic SQL.
-   Keep secrets outside source control.
-   Use HTTPS in deployed environments.
-   Apply appropriate password and account-lockout policies.
-   Use least-privilege database credentials.
-   Back up tenant databases and test restoration procedures.
-   Log security-relevant events without exposing credentials or
    sensitive personal data.

### Financial integrity

Before production use, financial workflows should be reviewed for
transaction consistency, idempotency, audit trails, reversal handling,
concurrency and reconciliation requirements.

------------------------------------------------------------------------

# Deployment and Operations

The deployment model should preserve the database-per-tenant
architecture while allowing the API application to be deployed and
maintained independently.

## Deployment Overview

``` mermaid
flowchart TB
    DEV["Developer"]
    GIT["GitHub Repository"]
    CI["CI Pipeline<br/>Restore · Build · Test"]
    DEPLOY["Deployment Environment"]
    API["ASP.NET Core API"]
    MASTER[("Master Database")]
    TDB1[("Tenant Database A")]
    TDB2[("Tenant Database B")]
    LOG["Application Logs and Monitoring"]
    BACKUP["Database Backup and Recovery"]

    DEV --> GIT --> CI --> DEPLOY --> API
    API --> MASTER
    API --> TDB1
    API --> TDB2
    API --> LOG
    MASTER --> BACKUP
    TDB1 --> BACKUP
    TDB2 --> BACKUP
```

This is the target operational view, not a claim that a CI/CD pipeline,
monitoring service or automated backup system is already configured.

## Recommended operational responsibilities

  -----------------------------------------------------------------------
  Operation                           Responsibility
  ----------------------------------- -----------------------------------
  Build                               Restore dependencies and compile
                                      the solution

  Automated tests                     Run unit/integration tests before
                                      release

  Configuration                       Supply environment-specific
                                      settings and secrets

  Database migration                  Apply reviewed migrations to the
                                      intended context and databases

  Tenant provisioning                 Register a tenant and prepare its
                                      database/configuration

  Deployment                          Publish and deploy the API

  Monitoring                          Track application health, errors
                                      and performance

  Backup                              Back up master and tenant databases

  Recovery                            Test restoration and tenant
                                      database recovery procedures
  -----------------------------------------------------------------------

## Suggested CI Pipeline

A basic GitHub Actions pipeline can be introduced to restore, build and
test the solution on each push or pull request.

Example workflow outline:

``` yaml
name: Build and Test

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Restore
        run: dotnet restore Saas-Micro-Finance.sln

      - name: Build
        run: dotnet build Saas-Micro-Finance.sln --no-restore

      - name: Test
        run: dotnet test Saas-Micro-Finance.sln --no-build
```

This is a starter workflow. Confirm that the repository has a test
project and that its tests can run in the selected CI environment before
enabling it as a release gate. Pin third-party actions to reviewed
versions or commit SHAs as part of a production hardening process.

------------------------------------------------------------------------

# Project Status and Roadmap

The project is being developed incrementally. The checklist below should
be updated as work is completed and verified.

  -----------------------------------------------------------------------
  Area                                Status
  ----------------------------------- -----------------------------------
  Layered solution foundation         Established

  Finbuckle.MultiTenant integration   Established

  EF Core / SQL Server integration    Established

  Master and tenant DbContext         Established in the project design
  separation                          

  Tenant isolation testing            To be completed and verified

  Customer and employee workflows     Verify end-to-end

  Core account and loan workflows     Continue implementation and
                                      verification

  Frontend                            In progress

  Automated test coverage             To be completed

  API documentation                   Review and complete

  Tenant provisioning automation      Planned / verify current
                                      implementation

  CI pipeline                         Planned

  Deployment guide                    Planned

  Screenshots and demonstration video Planned
  -----------------------------------------------------------------------

### Next engineering milestones

-   [ ] Verify tenant resolution and cross-tenant isolation with
    integration tests.
-   [ ] Complete and test customer and employee identity relationships.
-   [ ] Verify account, loan and repayment workflows end to end.
-   [ ] Review database constraints, indexes and monetary precision.
-   [ ] Complete frontend integration.
-   [ ] Document verified API endpoints and request/response examples.
-   [ ] Implement tenant provisioning and migration orchestration.
-   [ ] Add GitHub Actions build and test workflow.
-   [ ] Prepare deployment configuration and secret management.
-   [ ] Add screenshots and a short product demonstration.

------------------------------------------------------------------------

# Contributing

Contributions, suggestions and technical feedback are welcome.

1.  Fork the repository.

2.  Create a feature branch:

    ``` bash
    git checkout -b feature/your-feature
    ```

3.  Commit your changes:

    ``` bash
    git commit -m "Describe your changes"
    ```

4.  Push your branch:

    ``` bash
    git push origin feature/your-feature
    ```

5.  Open a Pull Request describing the change and its tests.

## License

No license has been specified in this repository documentation. Add a
`LICENSE` file and update this section before presenting the project as
open source.

## Author

**Collins Aigbekhai**

-   GitHub: [@collarfil](https://github.com/collarfil)

------------------------------------------------------------------------

::: {align="center"}
**SaaS Micro Finance Platform**\
*Building a scalable foundation for multi-institution microfinance
operations.*
:::