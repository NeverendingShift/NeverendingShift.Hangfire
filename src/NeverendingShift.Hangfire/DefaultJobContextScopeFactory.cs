using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NeverendingShift.Hangfire
{
    internal sealed class DefaultJobContextScopeFactory : IJobContextScopeFactory, IJobContextAccessor
    {
        private static readonly AsyncLocal<Stack<PerformingContext>> _stack = new AsyncLocal<Stack<PerformingContext>>();

        public PerformingContext Current
        {
            get
            {
                var stack = _stack.Value;
                return stack != null && stack.Count > 0
                    ? stack.Peek()
                    : null;
            }
        }

        public IDisposable BeginScope(PerformingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var stack = _stack.Value;
            if (stack == null)
            {
                stack = new Stack<PerformingContext>();
                _stack.Value = stack;
            }

            stack.Push(context);

            return new Scope(stack);
        }

        private sealed class Scope : IDisposable
        {
            private readonly Stack<PerformingContext> _stack;
            private bool _disposed;

            public Scope(Stack<PerformingContext> stack)
            {
                _stack = stack;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;

                if (_stack.Count > 0)
                    _stack.Pop();

                if (_stack.Count == 0)
                    _stack.Clear(); // optional
            }
        }
    }
}
