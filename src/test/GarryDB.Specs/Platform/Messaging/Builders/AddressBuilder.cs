using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins;
using GarryDB.Specs.Builders.Randomized;
using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Messaging.Builders
{
    internal sealed class AddressBuilder : TestDataBuilder<Address>
    {
        private string handler;
        private PluginIdentity pluginIdentity;

        protected override void OnPreBuild()
        {
            if (pluginIdentity == null)
            {
                For(new PluginIdentityBuilder().Build());
            }

            if (handler == null)
            {
                For(new RandomStringBuilder().Build());
            }
        }

        protected override Address OnBuild()
        {
            return new(pluginIdentity, handler);
        }

        public AddressBuilder For(PluginIdentity pluginIdentity)
        {
            return For(pluginIdentity, handler);
        }

        public AddressBuilder For(string handler)
        {
            return For(pluginIdentity, handler);
        }

        public AddressBuilder For(PluginIdentity pluginIdentity, string handler)
        {
            this.pluginIdentity = pluginIdentity;
            this.handler = handler;

            return this;
        }
    }
}
