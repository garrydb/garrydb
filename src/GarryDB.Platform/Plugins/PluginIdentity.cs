using System;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     The identity of a <see cref="Plugin" />.
    /// </summary>
    public sealed class PluginIdentity
    {
        private readonly string version;

        /// <summary>
        ///     Initializes a new <see cref="PluginIdentity" />.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="version">The version of the plugin.</param>
        public PluginIdentity(string name, string version = "*")
        {
            Name = name;
            this.version = version;
        }

        /// <summary>
        ///     Gets the name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is PluginIdentity that && Equals(that);
        }

        private bool Equals(PluginIdentity other)
        {
            if (!Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (version == "*" || other.version == "*")
            {
                return true;
            }

            return version == other.version;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.ToLowerInvariant().GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name}.{version}";
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(PluginIdentity? a, PluginIdentity? b)
        {
            if (a is null)
            {
                return false;
            }

            if (b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(PluginIdentity? a, PluginIdentity? b)
        {
            return !(a == b);
        }
    }
}
