using System;
using System.Collections.Generic;

namespace NeverendingShift.Hangfire;

public static class PerformingContextAccessorExtensions
{
    /// <summary>
    /// Gets the current job ID from the accessor.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <returns>The job ID, or null if not in a job context</returns>
    public static string GetCurrentJobId(this IPerformingContextAccessor accessor)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        return accessor.Current?.BackgroundJob.Id;
    }

    /// <summary>
    /// Reports progress for the current job.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <param name="value">Progress value (0-100)</param>
    /// <param name="message">Optional progress message</param>
    public static void SetProgress(this IPerformingContextAccessor accessor, int value, string message = null)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        var context = accessor.Current;
        if (context == null)
        {
            return; // Silently ignore if not in job context
        }

        if (value < 0 || value > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Progress value must be between 0 and 100");
        }

        context.SetJobParameter("Progress", value);

        if (!string.IsNullOrWhiteSpace(message))
        {
            context.SetJobParameter("ProgressMessage", message);
        }
    }

    /// <summary>
    /// Sets a custom parameter on the current job.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Parameter value</param>
    public static void SetJobParameter(this IPerformingContextAccessor accessor, string name, object value)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be null or whitespace", nameof(name));
        }

        accessor.Current?.SetJobParameter(name, value);
    }

    /// <summary>
    /// Gets a custom parameter from the current job.
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="accessor">The context accessor</param>
    /// <param name="name">Parameter name</param>
    /// <returns>The parameter value, or default(T) if not found or not in job context</returns>
    public static T GetJobParameter<T>(this IPerformingContextAccessor accessor, string name)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be null or whitespace", nameof(name));
        }

        var context = accessor.Current;
        if (context == null)
        {
            return default(T);
        }

        return context.GetJobParameter<T>(name);
    }

    /// <summary>
    /// Checks if the current code is executing within a Hangfire job context.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <returns>True if in a job context, false otherwise</returns>
    public static bool IsInJobContext(this IPerformingContextAccessor accessor)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        return accessor.Current != null;
    }

    /// <summary>
    /// Gets the job creation time from the current job.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <returns>The creation time, or null if not in job context</returns>
    public static DateTime? GetJobCreatedAt(this IPerformingContextAccessor accessor)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        return accessor.Current?.BackgroundJob.CreatedAt;
    }

    /// <summary>
    /// Gets all items from the current performing context.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <returns>The items dictionary, or null if not in job context</returns>
    public static IDictionary<string, object> GetItems(this IPerformingContextAccessor accessor)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        return accessor.Current?.Items;
    }

    /// <summary>
    /// Sets an item in the current performing context.
    /// Items are scoped to the current job execution.
    /// </summary>
    /// <param name="accessor">The context accessor</param>
    /// <param name="key">The item key</param>
    /// <param name="value">The item value</param>
    public static void SetItem(this IPerformingContextAccessor accessor, string key, object value)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var items = accessor.Current?.Items;
        if (items != null)
        {
            items[key] = value;
        }
    }

    /// <summary>
    /// Gets an item from the current performing context.
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="accessor">The context accessor</param>
    /// <param name="key">The item key</param>
    /// <returns>The item value, or default(T) if not found</returns>
    public static T GetItem<T>(this IPerformingContextAccessor accessor, string key)
    {
        if (accessor == null)
        {
            throw new ArgumentNullException(nameof(accessor));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        var items = accessor.Current?.Items;
        if (items != null && items.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }

        return default(T);
    }
}