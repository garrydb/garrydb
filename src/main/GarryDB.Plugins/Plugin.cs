using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace GarryDB.Plugins
{
    /// <summary>
    ///     A plugin.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Default | ImplicitUseTargetFlags.WithInheritors)]
    public abstract class Plugin
    {
        private readonly IDictionary<string, Func<object, Task<object?>>> handlers;
        private readonly PluginContext pluginContext;

        /// <summary>
        ///     Initializes a new plugin.
        /// </summary>
        /// <param name="pluginContext">The plugin context.</param>
        protected Plugin(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;
            handlers = new Dictionary<string, Func<object, Task<object?>>>();

            Register("start", (object _) => StartAsync());
            Register("stop", async (object _) =>
            {
                await StopAsync();
                return new object();
            });
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
                                                             TResult? result = await handler((TMessage)message).ConfigureAwait(false);

                                                             return result;
                                                         };

            handlers[name] = handlerWrapper;
        }

        /// <summary>
        ///     Route the message to the route.
        /// </summary>
        /// <param name="name">The name of the route.</param>
        /// <param name="message">The message to handle.</param>
        /// <returns>The result of handling the message.</returns>
        internal Task<object?> RouteAsync(string name, object message)
        {
            if (!handlers.ContainsKey(name))
            {
                return Task.FromResult<object?>(null);
            }

            Func<object, Task<object?>> handler = handlers[name];

            return handler(message);
        }

        /// <summary>
        ///     Sends <paramref name="message" /> to <paramref name="destination" />.
        /// </summary>
        /// <param name="destination">The destination plugin.</param>
        /// <param name="handler">The name of the handler.</param>
        /// <param name="message">The (optional) message.</param>
        protected Task SendAsync(string destination, string handler, object? message = null)
        {
            return pluginContext.SendAsync(destination, handler, message ?? new object());
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
