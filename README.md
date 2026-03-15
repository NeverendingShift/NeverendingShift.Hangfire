# NeverendingShift.Hangfire

[![NuGet](https://img.shields.io/nuget/v/NeverendingShift.Hangfire.svg)](https://www.nuget.org/packages/NeverendingShift.Hangfire/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Advanced Hangfire extensions providing thread-safe access to `PerformingContext` throughout your background jobs.
Works with both .NET Framework 4.8 and .NET 5+.

## Features

- ✅ **Thread-safe** `PerformingContext` access using `AsyncLocal<T>`
- ✅ **Works with .NET Framework 4.6.2 and .NET 5+**
- ✅ **Simple registration** with or without DI
- ✅ **Extension methods** for common operations
- ✅ **Handy attributes** so you don't have to figure them out yourself

## Installation

```bash
dotnet add package NeverendingShift.Hangfire
```

Or via Package Manager:
```
Install-Package NeverendingShift.Hangfire
```

## Quick Start

### .NET 5+ / .NET Core (with Dependency Injection)

```csharp
using NeverendingShift.Hangfire;

// Program.cs or Startup.cs
services.AddHangfirePerformingContextAccessor();

services.AddHangfire((sp, config) =>
{
    config
        .UseSqlServerStorage("YourConnectionString")
        .UsePerformingContextAccessor(sp);
});

services.AddHangfireServer();
```

### .NET Framework 4.8 (without DI)

```csharp
using NeverendingShift.Hangfire;

// Global.asax.cs or App_Start
var accessor = new PerformingContextAccessor();

GlobalConfiguration.Configuration
    .UseSqlServerStorage("YourConnectionString")
    .UsePerformingContextAccessor(accessor);

// Make accessor available to your jobs (e.g., via a static property or DI container)
JobHelper.PerformingContextAccessor = accessor;
```

### .NET Framework 4.8 with DI Container (e.g., Autofac)

```csharp
using NeverendingShift.Hangfire;

// Register in your container
builder.RegisterType<PerformingContextAccessor>()
    .As<IPerformingContextAccessor>()
    .SingleInstance();

builder.RegisterType<PerformingContextAccessorFilter>()
    .SingleInstance();

// Configure Hangfire
var accessor = container.Resolve<IPerformingContextAccessor>();
GlobalConfiguration.Configuration
    .UseSqlServerStorage("YourConnectionString")
    .UsePerformingContextAccessor(accessor);
```

## Usage Examples

### Basic Job with Progress Reporting

```csharp
public class DataProcessingJob
{
    private readonly IPerformingContextAccessor _contextAccessor;

    public DataProcessingJob(IPerformingContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public async Task ProcessAsync(string dataId)
    {
        // Report progress
        _contextAccessor.SetProgress(0, "Starting...");
        
        // Get job information
        var jobId = _contextAccessor.GetCurrentJobId();
        _contextAccessor.WriteLine($"Processing data {dataId} in job {jobId}");

        await Step1();
        _contextAccessor.SetProgress(33, "Step 1 complete");

        await Step2();
        _contextAccessor.SetProgress(66, "Step 2 complete");

        await Step3();
        _contextAccessor.SetProgress(100, "Complete!");
        
        _contextAccessor.WriteLine("Processing complete", ConsoleTextColor.Green);
    }
}
```

### Progress Tracking with Loop

```csharp
public async Task ProcessRecordsAsync(List<Record> records)
{
    var total = records.Count;
    
    for (int i = 0; i < total; i++)
    {
        await ProcessRecord(records[i]);
        
        var progress = (int)((i + 1) / (double)total * 100);
        _contextAccessor.SetProgress(progress, $"Processed {i + 1}/{total}");
    }
}
```

### Storing Custom Metadata

```csharp
public async Task ProcessOrderAsync(int orderId)
{
    var startTime = DateTime.UtcNow;
    
    // Store custom parameters that persist with the job
    _contextAccessor.SetJobParameter("OrderId", orderId);
    _contextAccessor.SetJobParameter("StartTime", startTime);
    
    await ProcessOrder(orderId);
    
    var duration = DateTime.UtcNow - startTime;
    _contextAccessor.SetJobParameter("Duration", duration.TotalSeconds);
    _contextAccessor.SetJobParameter("Status", "Completed");
}
```

### Conditional logic (Works Inside and Outside Jobs)

```csharp
public class SharedService
{
    private readonly IPerformingContextAccessor _contextAccessor;

    public async Task DoWorkAsync()
    {
        if (_contextAccessor.IsInJobContext())
        {
            // Running in Hangfire - use Hangfire console
            _contextAccessor.Current.WriteLine("Processing work item");
            _contextAccessor.SetProgress(50);
        }
        else
        {
            // Running outside Hangfire - use regular logging
            Console.WriteLine("Processing work item");
        }
        
        await ProcessWork();
    }
}
```

### Using Context Items for Scoped Data

```csharp
public async Task ProcessWorkflowAsync()
{
    // Store data in context items (scoped to this job execution)
    _contextAccessor.SetItem("WorkflowId", Guid.NewGuid());
    _contextAccessor.SetItem("StartTime", DateTime.UtcNow);
    
    await Step1(); // Can access items from nested methods
    await Step2();
    await Step3();
    
    // Retrieve stored data
    var workflowId = _contextAccessor.GetItem<Guid>("WorkflowId");
    var startTime = _contextAccessor.GetItem<DateTime>("StartTime");
    var duration = DateTime.UtcNow - startTime;
    
    _contextAccessor.WriteLine($"Workflow {workflowId} completed in {duration.TotalSeconds:F2}s");
}

private async Task Step1()
{
    var workflowId = _contextAccessor.GetItem<Guid>("WorkflowId");
    _contextAccessor.WriteLine($"Step 1 for workflow {workflowId}");
    await Task.Delay(100);
}
```

## API Reference

### IPerformingContextAccessor

```csharp
public interface IPerformingContextAccessor
{
    PerformingContext Current { get; set; }
}
```

### Extension Methods

```csharp
// Get current job ID
string GetCurrentJobId(this IPerformingContextAccessor accessor)

// Report progress (0-100)
void SetProgress(this IPerformingContextAccessor accessor, int value, string message = null)

// Store custom job parameter
void SetJobParameter(this IPerformingContextAccessor accessor, string name, object value)

// Retrieve job parameter
T GetJobParameter<T>(this IPerformingContextAccessor accessor, string name)

// Check if in job context
bool IsInJobContext(this IPerformingContextAccessor accessor)

// Get job creation time
DateTime? GetJobCreatedAt(this IPerformingContextAccessor accessor)

// Context items (scoped storage)
void SetItem(this IPerformingContextAccessor accessor, object key, object value)
T GetItem<T>(this IPerformingContextAccessor accessor, object key)
IDictionary<object, object> GetItems(this IPerformingContextAccessor accessor)
```

## Framework Compatibility

| Framework | Supported | Notes |
|-----------|-----------|-------|
| .NET Framework 4.8 | ✅ | Full support, use manual registration |
| .NET Framework 4.7.2 | ✅ | Via netstandard2.0 |
| .NET Framework 4.6.1 | ✅ | Via netstandard2.0 |
| .NET Standard 2.0 | ✅ | Full support |
| .NET 5 | ✅ | Full support with DI extensions |
| .NET 6+ | ✅ | Full support with DI extensions |

## Best Practices

### 1. Always Inject as Dependency
```csharp
// ✅ Good - use dependency injection
public MyJob(IPerformingContextAccessor contextAccessor) { }

// ❌ Bad - don't create instances directly
var accessor = new PerformingContextAccessor();
```

### 2. Check Context Before Use
```csharp
// ✅ Good - check if in job context
if (_contextAccessor.IsInJobContext())
{
    _contextAccessor.WriteLine("Running in Hangfire");
}

// ❌ Bad - assumes context exists
_contextAccessor.Current.WriteLine("This may throw!");
```

### 3. Use Extension Methods
```csharp
// ✅ Good - use extension methods
_contextAccessor.SetProgress(50, "Half done");
var jobId = _contextAccessor.GetCurrentJobId();

// ❌ Less ideal - direct property access
_contextAccessor.Current?.SetJobParameter("Progress", 50);
```

## Thread Safety

The implementation uses `AsyncLocal<T>` to maintain context across async/await boundaries:

```csharp
// ✅ Thread-safe PerformingContext access
BackgroundJob.Enqueue(() => SomeMethodAsync());
BackgroundJob.Enqueue(() => SomeMethodAsync());
```

## Performance

- **Minimal Overhead**: Uses `AsyncLocal<T>` with negligible performance impact
- **No Locking**: Thread-safe without locks or synchronization
- **Singleton**: Registered as singleton to minimize allocations

## Troubleshooting

### Context is Always Null

**Problem**: `Current` property returns null in jobs.

**Solutions**:
1. Ensure you registered the accessor (`.AddHangfirePerformingContextAccessor()` or manual registration)
2. Verify `.UsePerformingContextAccessor()` is called in Hangfire configuration
3. Check that jobs are executed by Hangfire server (not called directly)

### Progress Not Updating

**Problem**: Progress values set but not visible in dashboard.

**Solutions**:
1. Use a storage provider that supports job parameters (SQL Server, PostgreSQL, etc.)
2. Ensure values are between 0-100
3. Check your Hangfire dashboard version

### .NET Framework Compilation Errors

**Problem**: Extension methods not available in .NET Framework project.

**Solution**: Ensure your project targets .NET Framework 4.6.1 or higher, which is compatible with netstandard2.0.

## Example Projects

Check the [samples directory](https://github.com/NeverendingShift/NeverendingShift.Hangfire/tree/main/samples) for complete working examples:

- **NetCore.Sample** - .NET 6+ with DI
- **NetFramework48.Sample** - .NET Framework 4.8 without DI
- **NetFramework48.Autofac.Sample** - .NET Framework 4.8 with Autofac

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Daniel Barwikowski**  
GitHub: [@dbarwikowski](https://github.com/dbarwikowski)

## Support

- 🐛 [Report a bug](https://github.com/NeverendingShift/NeverendingShift.Hangfire/issues)
- 💡 [Request a feature](https://github.com/NeverendingShift/NeverendingShift.Hangfire/issues)
- 📖 [Documentation](https://github.com/NeverendingShift/NeverendingShift.Hangfire/wiki)

---

**Made with ❤️ for the Hangfire community**