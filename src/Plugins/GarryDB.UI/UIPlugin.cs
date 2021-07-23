using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

using GarryDb.Plugins;

using GarryDB.UI.Views;

namespace GarryDB.UI
{
    public class UIPlugin : Plugin
    {
        private readonly App app;

        public UIPlugin(PluginContext pluginContext, App app)
            : base(pluginContext)
        {
            this.app = app;
        }

        private void Foo()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN UIPlugin.Foo");
            app.Shutdown = () => SendAsync("Garry", "shutdown");

            AppBuilder
                .Configure(() => app)
                .UsePlatformDetect()
                .LogToTrace()
                .StartWithClassicDesktopLifetime(new string[0]);

            Debug.WriteLine($"{DateTimeOffset.Now:s} END UIPlugin.Foo");
        }



        public static App CreateApplication
        {
            get
            {
                var mainThread = new Thread(TestAppBuilder);
#pragma warning disable CA1416 // Validate platform compatibility
                mainThread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416 // Validate platform compatibility
                mainThread.Start();

                initialized.WaitOne();

                return instance;
            }
        }

        private static App instance;
        private static ClassicDesktopStyleApplicationLifetime lifetime;

        private static AutoResetEvent initialized = new AutoResetEvent(false);
        private static AutoResetEvent starting = new AutoResetEvent(false);

        private static void TestAppBuilder()
        {
            lifetime = new ClassicDesktopStyleApplicationLifetime()
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose
            };

            instance =
                (App) AppBuilder
                    .Configure<App>()
                    .UsePlatformDetect()
                    .LogToTrace()
                    .SetupWithLifetime(lifetime)
                    .Instance;

            initialized.Set();
            starting.WaitOne();

            lifetime.Start(new string[0]);
        }

        protected override void Start()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN UIPlugin.Start");
            var startupCompleted = new AutoResetEvent(false);

            IDisposable eventSubscription =
                Window.WindowOpenedEvent.AddClassHandler(typeof(MainWindow), (_, _) => startupCompleted.Set());

            instance.Shutdown = () => SendAsync("Garry", "shutdown");
            starting.Set();
            startupCompleted.WaitOne();
            startupCompleted.Dispose();
            eventSubscription.Dispose();

            Debug.WriteLine($"{DateTimeOffset.Now:s} END UIPlugin.Start");
        }
    }
}
