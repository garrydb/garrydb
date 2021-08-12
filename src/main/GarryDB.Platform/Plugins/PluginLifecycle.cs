using System.Collections.Generic;

using GarryDB.Plugins;

#pragma warning disable 1591

namespace GarryDB.Platform.Plugins
{
    public abstract class PluginLifecycle
    {
        protected PluginLifecycle(PluginLifecycle next)
        {
            Next = next;
        }

        protected PluginLifecycle Next { get; }

        public virtual IEnumerable<PluginPackage> Find(string pluginsDirectory)
        {
            return Next.Find(pluginsDirectory);
        }

        public virtual void Prepare(IEnumerable<PluginPackage> pluginPackages)
        {
            Next.Prepare(pluginPackages);
        }

        public virtual PluginIdentity? Register(PluginContextFactory pluginContextFactory, PluginPackage pluginPackage)
        {
            return Next.Register(pluginContextFactory, pluginPackage);
        }

        public virtual Plugin? Load(PluginIdentity pluginIdentity)
        {
            return Next.Load(pluginIdentity);
        }

        public virtual object? Configure(PluginIdentity pluginIdentity)
        {
            return Next.Configure(pluginIdentity);
        }

        public virtual void Start(IEnumerable<PluginIdentity> pluginIdentities)
        {
            Next.Start(pluginIdentities);
        }

        public virtual void Stop(IEnumerable<PluginIdentity> pluginIdentities)
        {
            Next.Stop(pluginIdentities);
        }
    }
}
