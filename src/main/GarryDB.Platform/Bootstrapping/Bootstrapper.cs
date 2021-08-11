using System;
using System.Collections.Generic;
using System.Linq;

using GarryDB.Platform.Actors;
using GarryDB.Platform.Infrastructure;
using GarryDB.Platform.Persistence;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;

namespace GarryDB.Platform.Bootstrapping
{
    /// <summary>
    /// 
    /// </summary>
    public static class Modifier
    {
        /// <summary>
        /// 
        /// </summary>
        public delegate Bootstrapper BootstrapperModifier(Bootstrapper bootstrapper);

        /// <summary>
        /// 
        /// </summary>
        public delegate IEnumerable<PluginDirectory> FindPlugins(string pluginsDirectory);

        /// <summary>
        /// 
        /// </summary>
        public delegate IEnumerable<PluginLoadContext> PreparePlugins(IReadOnlyList<PluginDirectory> pluginDirectories);

        /// <summary>
        /// 
        /// </summary>
        public delegate IEnumerable<PluginIdentity> RegisterPlugins(IReadOnlyList<PluginLoadContext> pluginLoadContexts);

        /// <summary>
        /// 
        /// </summary>
        public delegate Plugin? LoadPlugin(PluginIdentity pluginIdentity);

        /// <summary>
        /// 
        /// </summary>
        public delegate void ConfigurePlugin(PluginIdentity pluginIdentity, object? configuration);

        /// <summary>
        /// 
        /// </summary>
        public delegate void StartPlugins(IReadOnlyList<PluginIdentity> pluginIdentities);

        /// <summary>
        /// 
        /// </summary>
        public delegate void StopPlugins(IReadOnlyList<PluginIdentity> pluginIdentities);
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class Bootstrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public Bootstrapper()
        {
            Find = _ => Enumerable.Empty<PluginDirectory>();
            Prepare = _ => Enumerable.Empty<PluginLoadContext>();
            Register = _ => Enumerable.Empty<PluginIdentity>();
            Load = _ => null;
            Configure =  (_, _) => { };
            Start = _ => { };
            Stop = _ => { };

            PluginRegistry = new PluginRegistry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Finder(Func<Modifier.FindPlugins, Modifier.FindPlugins> replacer)
        {
            Find = replacer(Find);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Preparer(Func<Modifier.PreparePlugins, Modifier.PreparePlugins> replacer)
        {
            Prepare = replacer(Prepare);
            return this;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Loader(Func<Modifier.LoadPlugin, Modifier.LoadPlugin> replacer)
        {
            Load = replacer(Load);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Registrar(Func<Modifier.RegisterPlugins, Modifier.RegisterPlugins> replacer)
        {
            Register = replacer(Register);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Configurer(Func<Modifier.ConfigurePlugin, Modifier.ConfigurePlugin> replacer)
        {
            Configure = replacer(Configure);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Starter(Func<Modifier.StartPlugins, Modifier.StartPlugins> replacer)
        {
            Start = replacer(Start);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Stopper(Func<Modifier.StopPlugins, Modifier.StopPlugins> replacer)
        {
            Stop = replacer(Stop);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Replace(Func<PluginContextFactory, PluginContextFactory> replacer)
        {
            PluginContextFactory = replacer(PluginContextFactory);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Replace(Func<FileSystem, FileSystem> replacer)
        {
            FileSystem = replacer(FileSystem);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public Bootstrapper Replace(Func<ConnectionFactory, ConnectionFactory> replacer)
        {
            ConnectionFactory = replacer(ConnectionFactory);
            return this;
        }

        internal Modifier.FindPlugins Find { get; private set; }
        internal Modifier.PreparePlugins Prepare { get; private set; }
        internal Modifier.RegisterPlugins Register { get; private set; }
        internal Modifier.LoadPlugin Load { get; private set; }
        internal Modifier.ConfigurePlugin Configure { get; private set; }
        internal Modifier.StartPlugins Start { get; private set; }
        internal Modifier.StopPlugins Stop { get; private set; }
        internal PluginContextFactory PluginContextFactory { get; private set; } = null!;
        internal FileSystem FileSystem { get; private set; } = null!;
        internal ConnectionFactory ConnectionFactory { get; private set; } = null!;
        internal PluginRegistry PluginRegistry { get; }
    }
}
