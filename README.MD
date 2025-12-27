Understood. Below is a **proper README.md written in the same style, structure, and tone** as your example, but **accurately adapted to your .NET / Hangfire / Clean Architecture worker**.

This is **ready-to-use** documentation.
Paste it directly into `README.md`.

---

# worker

A **lightweight, extensible .NET background worker** built with **Clean Architecture** principles.
`worker` provides a structured foundation for defining, scheduling, and executing recurring background jobs using **Hangfire**, with **database-backed job state control** and strict separation of concerns.

The project is suitable for:

* Background processing services
* Microservices
* Scheduled and recurring jobs
* Systems requiring runtime job enable/disable control

---

## Features

* Clean Architecture (Application / Infrastructure / API separation)
* Recurring job scheduling via Hangfire
* Database-backed job state management (enable/disable jobs without redeploy)
* Infrastructure-agnostic application core
* SQL Server–based persistence (pluggable via abstractions)
* Fully async execution with graceful shutdown
* Dependency Injection–first design
* Easy to extend with new jobs or schedulers

---

## Table of contents

* [Quick start](#quick-start)
* [Requirements](#requirements)
* [Project structure](#project-structure)
* [Architecture overview](#architecture-overview)
* [Configuration](#configuration)
* [Usage](#usage)

    * [Defining a job](#defining-a-job)
    * [Scheduling jobs](#scheduling-jobs)
* [Job state management](#job-state-management)
* [Running with Docker](#running-with-docker)
* [Development](#development)
* [Testing](#testing)
* [Contributing](#contributing)
* [License](#license)
* [Maintainers](#maintainers)

---

## Quick start

1. Clone the repository:

   ```bash
   git clone https://github.com/HasanHasanbayli/worker.git
   cd worker
   ```

2. Restore and build:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Configure the database and Hangfire (see [Configuration](#configuration)).

4. Run the worker:

   ```bash
   dotnet run --project src/Worker.API
   ```

---

## Requirements

* .NET 10.0 (or configured target framework)
* SQL Server
* Hangfire

---

## Project structure

```text
src/
├── Worker.API
├── Worker.Application
└── Worker.Infrastructure
```

---

## Architecture overview

This project follows **Clean / Hexagonal Architecture**.

### Dependency rule

> Dependencies always point inward.
> `Worker.Application` has **no dependency** on frameworks, databases, or external systems.

### Layers

#### Worker.API

* Application entry point
* Configuration and hosting
* Dependency wiring

#### Worker.Application

* Business logic
* Job definitions
* Scheduling contracts
* Persistence abstractions

#### Worker.Infrastructure

* Hangfire integration
* SQL Server persistence
* Hosted startup services
* Framework-specific implementations

---

## Configuration

Configuration is provided via `appsettings.json` and environment variables.

### Example `appsettings.json`

```json
{
  "ConnectionStrings": {
    "HangfireConnection": "Server=localhost;Database=WorkerDb;Trusted_Connection=True;"
  }
}
```

---

## Usage

### Defining a job

Jobs live in `Worker.Application` and contain **business logic only**.

```csharp
public sealed class SmsJob : IRecurringJob
{
    public string JobId => "sms-job";
    public string Cron => "* * * * *";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // business logic
    }
}
```

* No Hangfire references
* No SQL references
* Fully testable in isolation

---

### Scheduling jobs

Scheduling is handled in Infrastructure via Hangfire.

```csharp
public sealed class HangfireRecurringJobScheduler
{
    public void RegisterAll()
    {
        // Registers jobs with Hangfire
    }
}
```

* Infrastructure connects scheduling metadata to execution
* Application remains scheduler-agnostic

---

## Job state management

Job execution can be enabled or disabled at runtime using a database-backed state.

```csharp
IJobStateService
```

This allows:

* Disabling jobs without redeploying
* Centralized operational control
* Future support for feature flags or tenants

---

## Development

* Follow Clean Architecture boundaries
* Do not introduce infrastructure dependencies into Application
* Prefer interfaces over concrete implementations
* Keep job logic deterministic and testable

---

---

## Contributing

Contributions are welcome.

1. Fork the repository
2. Create a branch: `feat/your-feature`
3. Submit a pull request with a clear description

---

## Maintainers

* HasanHasanbayli — [https://github.com/HasanHasanbayli](https://github.com/HasanHasanbayli)
