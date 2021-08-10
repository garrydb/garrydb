using System;

using GarryDB.Platform.Plugins;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    ///     The address to a handler of a plugin.
    /// </summary>
    internal sealed class Address
    {
        /// <summary>
        ///     Intializes a new <see cref="Address" />.
        /// </summary>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="handler">The name of the handler.</param>
        public Address(PluginIdentity pluginIdentity, string handler)
        {
            PluginIdentity = pluginIdentity;
            Handler = handler;
        }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Gets the name of the handler.
        /// </summary>
        public string Handler { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Address that && PluginIdentity.Equals(that.PluginIdentity) &&
                   Handler.Equals(that.Handler, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(PluginIdentity, Handler);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{PluginIdentity}.{Handler}";
        }
    }
}
