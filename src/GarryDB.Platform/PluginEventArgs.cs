using System;

using GarryDb.Plugins;

namespace GarryDb.Platform
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PluginEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginIdentity"></param>
        public PluginEventArgs(PluginIdentity pluginIdentity)
        {
            PluginIdentity = pluginIdentity;
        }

        /// <summary>
        /// 
        /// </summary>
        public PluginIdentity PluginIdentity { get; }
    }
}