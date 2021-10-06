using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GarryDB.Specs
{
/// <summary>
    ///     Provides a pump that supports running asynchronous methods on the current thread.
    /// </summary>
    /// <remarks>
    ///     https://blogs.msdn.microsoft.com/pfxteam/2012/02/02/await-synchronizationcontext-and-console-apps-part-3/
    /// </remarks>
    [DebuggerStepThrough]
    internal static class AsyncPump
    {
        /// <summary>
        ///     Runs the specified asynchronous method.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        public static void Run(Action asyncMethod)
        {
            SynchronizationContext prevCtx = SynchronizationContext.Current!;
            try
            {
                // Establish the new context
                var syncCtx = new SingleThreadSynchronizationContext(true);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                // Invoke the function
                syncCtx.OperationStarted();
                asyncMethod();
                syncCtx.OperationCompleted();

                // Pump continuations and propagate any exceptions
                syncCtx.RunOnCurrentThread();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        /// <summary>
        ///     Runs the specified asynchronous method.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        public static void Run(Func<Task> asyncMethod)
        {
            SynchronizationContext prevCtx = SynchronizationContext.Current!;
            try
            {
                // Establish the new context
                var syncCtx = new SingleThreadSynchronizationContext(false);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                // Invoke the function and alert the context to when it completes
                Task t = asyncMethod();
                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                // Pump continuations and propagate any exceptions
                syncCtx.RunOnCurrentThread();
                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        /// <summary>
        ///     Runs the specified asynchronous method.
        /// </summary>
        /// <param name="asyncMethod">The asynchronous method to execute.</param>
        public static T Run<T>(Func<Task<T>> asyncMethod)
        {
            SynchronizationContext prevCtx = SynchronizationContext.Current!;
            try
            {
                // Establish the new context
                var syncCtx = new SingleThreadSynchronizationContext(false);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                // Invoke the function and alert the context to when it completes
                Task<T> t = asyncMethod();
                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                // Pump continuations and propagate any exceptions
                syncCtx.RunOnCurrentThread();
                return t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        /// <summary>
        ///     Provides a SynchronizationContext that's single-threaded.
        /// </summary>
        private sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
        {
            /// <summary>The queue of work items.</summary>
            private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object?>> queue =
                new BlockingCollection<KeyValuePair<SendOrPostCallback, object?>>();

            /// <summary>Whether to track operations operationCount.</summary>
            private readonly bool trackOperations;

            /// <summary>The number of outstanding operations.</summary>
            private int operationCount;

            /// <summary>Initializes the context.</summary>
            /// <param name="trackOperations">Whether to track operation count.</param>
            public SingleThreadSynchronizationContext(bool trackOperations)
            {
                this.trackOperations = trackOperations;
            }

            /// <summary>
            ///     Dispatches an asynchronous message to the synchronization context.
            /// </summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object? state)
            {
                queue.Add(new KeyValuePair<SendOrPostCallback, object?>(d, state));
            }

            /// <summary>
            ///     Not supported.
            /// </summary>
            public override void Send(SendOrPostCallback d, object? state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            /// <summary>
            ///     Runs an loop to process all queued work items.
            /// </summary>
            public void RunOnCurrentThread()
            {
                foreach (KeyValuePair<SendOrPostCallback, object?> workItem in queue.GetConsumingEnumerable())
                {
                    workItem.Key(workItem.Value);
                }
            }

            /// <summary>
            ///     Notifies the context that no more work will arrive.
            /// </summary>
            public void Complete()
            {
                queue.CompleteAdding();
            }

            /// <summary>
            ///     Invoked when an async operation is started.
            /// </summary>
            public override void OperationStarted()
            {
                if (trackOperations)
                {
                    Interlocked.Increment(ref operationCount);
                }
            }

            /// <summary>
            ///     Invoked when an async operation is completed.
            /// </summary>
            public override void OperationCompleted()
            {
                if (trackOperations && Interlocked.Decrement(ref operationCount) == 0)
                {
                    Complete();
                }
            }

            public void Dispose()
            {
                queue.Dispose();
            }
        }
    }}
