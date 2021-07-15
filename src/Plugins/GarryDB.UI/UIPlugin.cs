using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;

using GarryDb.Plugins;

using GarryDB.UI.Views;

namespace GarryDB.UI
{
    public class UIPlugin : Plugin
    {
        private void Foo()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN UIPlugin.Foo");

            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace()
                .StartWithClassicDesktopLifetime(new string[0]);

            Debug.WriteLine($"{DateTimeOffset.Now:s} END UIPlugin.Foo");
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override void Start()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN UIPlugin.Start");
            var startupCompleted = new AutoResetEvent(false);

            IDisposable eventSubscription =
                Window.WindowOpenedEvent.AddClassHandler(typeof(MainWindow), (_, _) => startupCompleted.Set());
            
            var mainThread = new Thread(Foo);
            mainThread.SetApartmentState(ApartmentState.STA);
            mainThread.Start();

            startupCompleted.WaitOne();
            startupCompleted.Dispose();
            eventSubscription.Dispose();

            Debug.WriteLine($"{DateTimeOffset.Now:s} END UIPlugin.Start");
        }

        protected override void Stop()
        {
        }
    }
}
