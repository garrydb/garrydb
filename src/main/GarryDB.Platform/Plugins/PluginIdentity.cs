using System;
using System.Linq;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins
{
    /// <summary>
    ///     The identity of a <see cref="Plugin" />.
    /// </summary>
    public sealed class PluginIdentity
    {
        private const string AnyVersion = "*";

        private readonly string version;

        /// <summary>
        ///     Initializes a new <see cref="PluginIdentity" />.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="version">The version of the plugin.</param>
        private PluginIdentity(string name, string version = AnyVersion)
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
            if (!Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (version == AnyVersion || other.version == AnyVersion)
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
            return $"{Name}:{version}";
        }

        /// <summary>
        ///     Parse the string into a valid <see cref="PluginIdentity" />.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>The <see cref="PluginIdentity" />.</returns>
        public static PluginIdentity Parse(string value)
        {
            string[] nameVersionPair = value.Split(':');

            return new PluginIdentity(nameVersionPair.First(), nameVersionPair.Skip(1).LastOrDefault() ?? AnyVersion);
        }

        /// <summary>
        ///     Determines whether two instances of <see cref="PluginIdentity" /> represent the same <see cref="PluginIdentity" />.
        /// </summary>
        /// <param name="a">The first <see cref="PluginIdentity" />.</param>
        /// <param name="b">The second <see cref="PluginIdentity" />.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="a" /> is the same <see cref="PluginIdentity" /> as <paramref name="b" />,
        ///     otherwise <c>false</c>.
        /// </returns>
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
        ///     Determines whether two instances of <see cref="PluginIdentity" /> represent a different <see cref="PluginIdentity" />.
        /// </summary>
        /// <param name="a">The first <see cref="PluginIdentity" />.</param>
        /// <param name="b">The second <see cref="PluginIdentity" />.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="a" /> is different <see cref="PluginIdentity" /> as <paramref name="b" />,
        ///     otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(PluginIdentity? a, PluginIdentity? b)
        {
            return !(a == b);
        }
    }
}
