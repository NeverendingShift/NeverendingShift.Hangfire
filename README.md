# NeverendingShift.Hangfire

Advanced Hangfire extensions providing scope-based job context access and infrastructure-ready execution filters.

Designed for enterprise-grade background processing.

---

## ✨ Features

- 🔹 Scope-based `JobContext` (no static Set/Clear)
- 🔹 Async-safe and stack-safe (nested jobs supported)
- 🔹 Zero dependency on Microsoft.Extensions.*
- 🔹 Dual-mode (works with and without DI)
- 🔹 Extensible `IJobContextScopeFactory`
- 🔹 Explicit registration (no hidden magic)
- 🔹 Production-oriented design

---

## 📦 Installation

```bash
dotnet add package NeverendingShift.Hangfire
```

## 🚀 Quick Start

Register the filter explicitly during Hangfire configuration:

``` csharp
GlobalConfiguration.Configuration
    .UseSqlServerStorage(connectionString)
    .UseNeverendingShiftJobContext();
```
Now you can access the current job context anywhere:
```csharp
var jobId = JobContext.Current?.BackgroundJob?.Id;
```

## 🧠 Why Scope-Based?

Naive implementations often use:
```csharp
static AsyncLocal<PerformingContext>
```
with manual Set() / Clear() calls.

This library instead uses a scope-based model:
```csharp
public interface IJobContextScopeFactory
{
    IDisposable BeginScope(PerformingContext context);
}
```
Benefits:
- No global mutable setters ✔
- Automatic cleanup via IDisposable ✔
- Nested job safety (stack-based context) ✔
- Extensible execution model ✔
- Enterprise-ready architecture ✔

## 🏗 Architecture
```
JobContext (static read API)
IJobContextAccessor
IJobContextScopeFactory
DefaultJobContextScopeFactory
JobContextFilter
```

Design principles:
- Single source of truth
- Explicit configuration
- Async-safe by design
- No hidden side effects
- Extensible without breaking core behavior

## 🔧 Custom Scope Factory

You can replace the default implementation:
```csharp
JobContextAccessor.SetFactory(new MyCustomFactory());
```

Custom factories must implement:

```csharp
IJobContextScopeFactory
IJobContextAccessor
```

This enables integration with:
- ILogger scopes
- Activity / OpenTelemetry
- Multi-tenant propagation
- Custom execution tracking
- Distributed correlation systems

## 🔐 Thread & Async Safety

- Works with async/await
- Supports nested job execution
- Cleans up automatically on scope disposal

❌ Raw threads without ExecutionContext flow are not supported
(standard AsyncLocal limitation)

## 📌 Roadmap

- UniqueAttribute (distributed-safe execution)
- RetentionTimeAttribute
- Activity auto-enrichment
- CorrelationId support
- Multi-tenant propagation helpers
- Logger scope integration

## 🧩 Versioning

This project follows Semantic Versioning.

Pre-1.0 releases may introduce breaking changes.

## 📜 License

MIT License © 2026 Daniel Barwikowski

This project is provided "as is", without warranty of any kind.
