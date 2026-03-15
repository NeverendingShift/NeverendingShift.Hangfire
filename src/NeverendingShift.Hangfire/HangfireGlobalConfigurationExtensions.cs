using Hangfire;
using System;

#if NETSTANDARD2_0 || NET5_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

namespace NeverendingShift.Hangfire;

#if NETSTANDARD2_0 || NET5_0_OR_GREATER
/// <summary>
/// Extension methods for configuring Hangfire PerformingContext accessor services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the PerformingContext accessor services to the service collection.
    /// This includes registering the accessor and the filter that populates it.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    /// <example>
    /// <code>
    /// services.AddHangfirePerformingContextAccessor();
    /// services.AddHangfire(config => 
    /// {
    ///     config.UseSqlServerStorage("connectionString");
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddHangfirePerformingContextAccessor(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register the accessor as singleton (same lifetime as IHttpContextAccessor)
        services.TryAddSingleton<IPerformingContextAccessor, PerformingContextAccessor>();

        // Register the filter as singleton
        services.TryAddSingleton<PerformingContextAccessorFilter>();

        return services;
    }
}
#endif

/// <summary>
/// Extension methods for configuring Hangfire global configuration.
/// Works with both .NET Framework and .NET Core/.NET 5+
/// </summary>
public static class GlobalConfigurationExtensions
{
    /// <summary>
    /// Configures Hangfire to use the PerformingContext accessor by adding the global filter.
    /// </summary>
    /// <param name="configuration">The Hangfire configuration</param>
    /// <param name="accessor">The PerformingContext accessor instance</param>
    /// <returns>The configuration for chaining</returns>
    /// <example>
    /// <code>
    /// // .NET Framework 4.8 usage:
    /// GlobalConfiguration.Configuration.UsePerformingContextAccessor();
    /// 
    /// // .NET 5+ with DI:
    /// services.AddHangfirePerformingContextAccessor();
    /// var serviceProvider = services.BuildServiceProvider();
    /// GlobalConfiguration.Configuration.UsePerformingContextAccessor(
    ///     serviceProvider.GetRequiredService&lt;IPerformingContextAccessor&gt;());
    /// </code>
    /// </example>
    public static IGlobalConfiguration UsePerformingContextAccessor(
        this IGlobalConfiguration configuration,
        IPerformingContextAccessor accessor = null)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var filter = new PerformingContextAccessorFilter(accessor ?? new PerformingContextAccessor());
        GlobalJobFilters.Filters.Add(filter);

        return configuration;
    }

#if NETSTANDARD2_0 || NET5_0_OR_GREATER
    /// <summary>
    /// Configures Hangfire to use the PerformingContext accessor.
    /// The accessor is resolved from the service provider.
    /// </summary>
    /// <param name="configuration">The Hangfire configuration</param>
    /// <param name="serviceProvider">The service provider to resolve the accessor</param>
    /// <returns>The configuration for chaining</returns>
    /// <example>
    /// <code>
    /// services.AddHangfirePerformingContextAccessor();
    /// services.AddHangfire((sp, config) =>
    /// {
    ///     config
    ///         .UseSqlServerStorage("connectionString")
    ///         .UsePerformingContextAccessor(sp);
    /// });
    /// </code>
    /// </example>
    public static IGlobalConfiguration UsePerformingContextAccessor(
        this IGlobalConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        var accessor = (IPerformingContextAccessor)serviceProvider.GetService(typeof(IPerformingContextAccessor));
        if (accessor == null)
        {
            throw new InvalidOperationException(
                "IPerformingContextAccessor is not registered. " +
                "Call services.AddHangfirePerformingContextAccessor() first.");
        }

        return UsePerformingContextAccessor(configuration, accessor);
    }
#endif
}