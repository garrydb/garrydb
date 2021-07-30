﻿using GarryDB.Platform.Messaging.Messages;
using GarryDB.Platform.Plugins;
using GarryDB.Plugins;
using GarryDB.Specs.Platform.Plugins.Builders;

using GarryDB.Specs.Plugins.Builders;

namespace GarryDB.Specs.Platform.Messaging.Messages.Builders
{
    public sealed class PluginLoadedBuilder : TestDataBuilder<PluginLoaded>
    {
        private PluginIdentity pluginIdentity;
        private Plugin plugin;

        protected override void OnPreBuild()
        {
            if (pluginIdentity == null && plugin == null)
            {
                For(new PluginIdentityBuilder().Build(), new PluginBuilder().Build());
            }
        }

        protected override PluginLoaded OnBuild()
        {
            return new PluginLoaded(pluginIdentity, plugin);
        }

        public PluginLoadedBuilder For(PluginIdentity pluginIdentity, Plugin plugin)
        {
            this.pluginIdentity = pluginIdentity;
            this.plugin = plugin;

            return this;
        }
    }
}
