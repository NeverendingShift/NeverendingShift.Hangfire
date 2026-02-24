using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using System;

namespace NeverendingShift.Hangfire
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class RetentionTimeAttribute : JobFilterAttribute, IApplyStateFilter
    {
        private readonly RetentionPolicy _policy;

        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public RetentionTimeAttribute(
            RetentionPolicy retentionPolicy = RetentionPolicy.OnSucceeded)
        {
            _policy = retentionPolicy;
        }

        public void OnStateApplied(
            ApplyStateContext context,
            IWriteOnlyTransaction transaction)
        {
            if (!ShouldApply(context.NewState))
                return;

            var retention = new TimeSpan(Days, Hours, Minutes, 0);

            if (retention < TimeSpan.Zero)
                throw new InvalidOperationException(
                    $"Retention time cannot be negative but was: {retention}");

            context.JobExpirationTimeout = retention;
        }

        public void OnStateUnapplied(
            ApplyStateContext context,
            IWriteOnlyTransaction transaction)
        {
        }

        private bool ShouldApply(IState newState)
        {
            switch (_policy)
            {
                case RetentionPolicy.OnSucceeded:
                    return newState is SucceededState;

                case RetentionPolicy.OnFailed:
                    return newState is FailedState;

                case RetentionPolicy.Always:
                    return newState is FailedState
                        || newState is DeletedState
                        || newState is SucceededState;

                default:
                    return false;
            }
        }

        public enum RetentionPolicy
        {
            OnSucceeded,
            OnFailed,
            Always
        }
    }
}