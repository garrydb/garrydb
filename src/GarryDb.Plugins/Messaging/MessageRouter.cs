using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GarryDb.Plugins.Messaging
{
    /// <summary>
    ///     Routes messages to handlers.
    /// </summary>
    internal sealed class MessageRouter
    {
        private readonly IDictionary<string, Func<object, Task<object?>>> handlers;

        /// <summary>
        ///     Initializes a new <see cref="MessageRouter" />.
        /// </summary>
        internal MessageRouter()
        {
            handlers = new Dictionary<string, Func<object, Task<object?>>>();
        }

        /// <summary>
        ///     Register a handler for a named route.
        /// </summary>
        /// <param name="name">The name of the route.</param>
        /// <param name="handler">The handler.</param>
        internal void ConfigureRoute(string name, Func<object, Task<object?>> handler)
        {
            handlers[name] = handler;
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
    }
}
