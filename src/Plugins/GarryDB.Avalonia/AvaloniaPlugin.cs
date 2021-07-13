using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;

using GarryDB.Avalonia.Views;

using GarryDb.Plugins;

namespace GarryDB.Avalonia
{
    public class AvaloniaPlugin : Plugin
    {
        private void Foo()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.Foo");

            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace()
                .StartWithClassicDesktopLifetime(new string[0]);

            Debug.WriteLine($"{DateTimeOffset.Now:s} END AvaloniaPlugin.Foo");
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override void Start()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.Start");
            var startupCompleted = new AutoResetEvent(false);

            IDisposable eventSubscription =
                Window.WindowOpenedEvent.AddClassHandler(typeof(MainWindow), (_, _) => startupCompleted.Set());
            
            var mainThread = new Thread(Foo);
            mainThread.SetApartmentState(ApartmentState.STA);
            mainThread.Start();

            startupCompleted.WaitOne();
            startupCompleted.Dispose();

            Debug.WriteLine($"{DateTimeOffset.Now:s} END AvaloniaPlugin.Start");
        }

        protected override void Stop()
        {
        }
    }
}
