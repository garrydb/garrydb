using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.ReactiveUI;

using GarryDB.Plugins;

using UIPlugin.Shared;

namespace UIPlugin
{
    public sealed class UIPlugin : Plugin
    {
        public UIPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
            Register<Extension>("extend", extension =>
            {
                ((App)Application.Current).Extend(extension);
                return Task.CompletedTask;
            });
        }

        private void CreateApplication()
        {
            var configured = new AutoResetEvent(false);

            var mainThread = new Thread(_ =>
                                        {
                                            AppBuilder.Configure(() => new App(() => SendAsync("GarryPlugin", "shutdown")))
                                                      .UsePlatformDetect()
                                                      .UseReactiveUI()
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
