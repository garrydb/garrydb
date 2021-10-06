using System;
using System.Threading;
using System.Threading.Tasks;

using GarryDb.Platform.Plugins;
using GarryDb.Plugins;

namespace GarryDb.Platform
{
    internal sealed class GarryPlugin : Plugin, IDisposable
    {
        public static readonly PluginIdentity PluginIdentity = PluginIdentity.Parse("Garry:1.0");
        private readonly AutoResetEvent shutdownRequested;

        public GarryPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
            shutdownRequested = new AutoResetEvent(false);

            Register("shutdown", (object _) =>
            {
                shutdownRequested.Set();

                return Task.CompletedTask;
            });

            Register("start/reply", (object _) =>
            {
            });
        }

        public void WaitUntilShutdownRequested()
        {
            shutdownRequested.WaitOne();
        }

        public void Dispose()
        {
            shutdownRequested.Dispose();
        }
    }
}
