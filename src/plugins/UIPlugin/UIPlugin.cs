using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.ReactiveUI;

using GarryDB.Plugins;

using UIPlugin.Shared;

namespace UIPlugin
{
    public sealed class UIPlugin : Plugin, IDisposable
    {
        private readonly ReplaySubject<Extension> extensions;

        public UIPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
            extensions = new ReplaySubject<Extension>();
            Register<Extension>("extend", extension =>
            {
                extensions.OnNext(extension);
                return Task.CompletedTask;
            });
        }

        private void CreateApplication()
        {
            var configured = new AutoResetEvent(false);

            var mainThread = new Thread(_ =>
            {
                AppBuilder.Configure(() => CreateApplication(configured))
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
        }

        private App CreateApplication(AutoResetEvent configured)
        {
            var application = new App(
                () => SendAsync("GarryPlugin", "shutdown"),
                () =>
                {
                    extensions.Subscribe(extension => ((App)Application.Current).Extend(extension));
                    configured.Set();
                });

            return application;
        }

        protected override void Start()
        {
            CreateApplication();
        }

        public void Dispose()
        {
            extensions.Dispose();
        }
    }
}
