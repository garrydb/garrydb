using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;

using GarryDB.Plugins;

namespace GarryDB.UI
{
    public class UIPlugin : Plugin
    {
        private readonly Func<Func<Task>, Application> appBuilder;

        public UIPlugin(PluginContext pluginContext, Func<Func<Task>, Application> appBuilder)
            : base(pluginContext)
        {
            this.appBuilder = appBuilder;
        }

        private void CreateApplication()
        {
            var configured = new AutoResetEvent(false);

            var mainThread = new Thread(_ =>
                                        {
                                            AppBuilder.Configure(() => appBuilder(() => SendAsync("Garry", "shutdown")))
                                                      .UsePlatformDetect()
                                                      .LogToTrace()
                                                      .AfterSetup(_ => configured.Set())
                                                      .StartWithClassicDesktopLifetime(Array.Empty<string>());
                                        });

            if (OperatingSystem.IsWindows())
            {
                mainThread.SetApartmentState(ApartmentState.STA);
            }

            mainThread.Start();
            configured.WaitOne();
        }

        protected override void Start()
        {
            CreateApplication();
        }
    }
}
