using System;
using System.Threading.Tasks;

using GarryDb.Plugins.Messaging;

namespace GarryDb.Plugins
{
    /// <summary>
    ///     A plugin.
    /// </summary>
    public abstract class Plugin
    {
        private readonly MessageRouter messageRouter;

        /// <summary>
        ///     Initializes a new plugin.
        /// </summary>
        protected Plugin()
        {
            messageRouter = new MessageRouter();
            
            Register("start", (object _) => StartAsync());
            Register("stop", (object _) => StopAsync());
        }

        /// <summary>
        ///     Register a handler for a named route.
        /// </summary>
        /// <param name="name">The name of the route.</param>
        /// <param name="handler">The handler.</param>
        /// <typeparam name="TMessage">The type of messages the handler supports.</typeparam>
        protected void Register<TMessage>(string name, Func<TMessage, Task> handler)
        {
            Register<TMessage, object?>(name, async message =>
            {
                await handler(message).ConfigureAwait(false);
                return null;
            });
        }

        /// <summary>
        ///     Register a handler for a named route.
        /// </summary>
        /// <param name="name">The name of the route.</param>
        /// <param name="handler">The handler.</param>
        /// <typeparam name="TMessage">The type of messages the handler supports.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        protected void Register<TMessage, TResult>(string name, Func<TMessage, Task<TResult?>> handler)
        {
            Func<object, Task<object?>> handlerWrapper = async message =>
            {
                TResult? result = await handler((TMessage)message);
                return result;
            };

            messageRouter.ConfigureRoute(name, handlerWrapper);
        }

        /// <summary>
        ///     Route the message to the route.
        /// </summary>
        /// <param name="name">The name of the route.</param>
        /// <param name="message">The message to handle.</param>
        /// <returns>The result of handling the message.</returns>
        internal Task<object?> RouteAsync(string name, object message)
        {
            return messageRouter.RouteAsync(name, message);
        }

        /// <summary>
        ///     Called when the plugin is being started.
        /// </summary>
        protected virtual void Start()
        {
        }
        
        /// <summary>
        ///     Called when the plugin is being started.
        /// </summary>
        protected virtual Task StartAsync()
        {
            Start();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Called when the plugin is being stopped.
        /// </summary>
        protected virtual void Stop()
        {
        }

        /// <summary>
        ///     Called when the plugin is being stopped.
        /// </summary>
        protected virtual Task StopAsync()
        {
            Stop();
            return Task.CompletedTask;
        }
    }
}
