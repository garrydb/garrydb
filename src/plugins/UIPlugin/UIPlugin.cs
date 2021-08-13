using System;
using System.Collections.Generic;
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
        private readonly Queue<Extension> extensions;

        public UIPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
            extensions = new Queue<Extension>();

            Register<Extension>("extend", extension =>
            {
                extensions.Enqueue(extension);
                return Task.CompletedTask;
            });
        }

        private void CreateApplication()
        {
            var configured = new AutoResetEvent(false);

            var mainThread = new Thread(_ =>
            {
                AppBuilder.Configure(() => new App(() => SendAsync("GarryPlugin", "shutdown"), () => configured.Set()))
                    .UsePlatformDetect()
                    .UseReactiveUI()
                    .LogToTrace()
                    .StartWithClassicDesktopLifetime(Array.Empty<string>());
            });

            if (OperatingSystem.IsWindows())
            {
                mainThread.SetApartmentState(ApartmentState.STA);
            }

            mainThread.Start();
            configured.WaitOne();

            while (extensions.TryDequeue(out Extension extension))
            {
                ((App)Application.Current).Extend(extension);
            }
        }

        protected override void Start()
        {
            CreateApplication();
        }
    }
}
