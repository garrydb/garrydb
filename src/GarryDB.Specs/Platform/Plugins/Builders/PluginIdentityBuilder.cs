using GarryDB.Platform.Plugins;
using GarryDB.Specs.Builders.Randomized;

namespace GarryDB.Specs.Platform.Plugins.Builders
{
    public sealed class PluginIdentityBuilder : TestDataBuilder<PluginIdentity>
    {
        private string name;
        private string version;

        protected override void OnPreBuild()
        {
            if (name == null)
            {
                Named(new RandomStringBuilder().Build());
            }

            if (version == null)
            {
                int major = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(20).Build();
                int minor = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(20).Build();
                int patch = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(20).Build();

                WithVersion($"{major}.{minor}.{patch}");
            }
        }

        protected override PluginIdentity OnBuild()
        {
            return new PluginIdentity(name, version);
        }

        public PluginIdentityBuilder Named(string name)
        {
            this.name = name;
            return this;
        }

        public PluginIdentityBuilder WithVersion(string version)
        {
            this.version = version;
            return this;
        }
    }
}
