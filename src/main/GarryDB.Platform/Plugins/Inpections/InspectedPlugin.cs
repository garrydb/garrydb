using System.Collections.Generic;
using System.Collections.Immutable;

using GarryDB.Plugins;

namespace GarryDB.Platform.Plugins.Inpections
{
    /// <summary>
    ///     Contains the information gathered when inspecting a <see cref="Plugin" />.
    /// </summary>
    public sealed class InspectedPlugin
    {
        /// <summary>
        ///     Initializes a new <see cref="InspectedPlugin" />.
        /// </summary>
        /// <param name="pluginAssembly">The assembly containing the <see cref="Plugin" />.</param>
        /// <param name="providedAssemblies">The assemblies the <see cref="Plugin" /> provides.</param>
        /// <param name="referencedAssemblies">The assemblies the <see cref="Plugin" /> references,</param>
        public InspectedPlugin(PluginAssembly pluginAssembly,
                               IEnumerable<ProvidedAssembly> providedAssemblies,
                               IEnumerable<ReferencedAssembly> referencedAssemblies
        )
        {
            Path = pluginAssembly.Path;
            PluginAssembly = pluginAssembly;
            ProvidedAssemblies = providedAssemblies.ToImmutableList();
            ReferencedAssemblies = referencedAssemblies.ToImmutableList();
        }

        /// <summary>
        ///     Gets the startup order.
        /// </summary>
        public int StartupOrder
        {
            get { return PluginAssembly.StartupOrder; }
        }

        /// <summary>
        ///     Gets the <see cref="PluginIdentity" /> of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity
        {
            get { return PluginAssembly.PluginIdentity; }
        }

        /// <summary>
        ///     Gets the path to the <see cref="Plugin" />.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Gets the assembly that contains the <see cref="Plugin" />.
        /// </summary>
        public PluginAssembly PluginAssembly { get; }

        /// <summary>
        ///     Gets the assemblies that the plugin provides.
        /// </summary>
        public IImmutableList<ProvidedAssembly> ProvidedAssemblies { get; }

        /// <summary>
        ///     Gets the assemblies that the plugin references.
        /// </summary>
        public IImmutableList<ReferencedAssembly> ReferencedAssemblies { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is InspectedPlugin that && PluginAssembly.PluginIdentity == that.PluginAssembly.PluginIdentity;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PluginAssembly.PluginIdentity.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return PluginAssembly.PluginIdentity.ToString();
        }
    }
}
