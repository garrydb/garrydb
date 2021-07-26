using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins;

using GarryDb.Specs;
using GarryDb.Specs.Builders.Randomized;

using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Messaging.Builders
{
    public sealed class AddressBuilder : TestDataBuilder<Address>
    {
        private PluginIdentity pluginIdentity;
        private string handler;

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
            return new Address(pluginIdentity, handler);
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
