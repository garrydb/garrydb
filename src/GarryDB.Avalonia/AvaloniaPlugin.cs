using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using Avalonia;
using Avalonia.ReactiveUI;

using GarryDb.Plugins;

namespace GarryDB.Avalonia
{
    public class AvaloniaPlugin : Plugin
    {
        private static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace();
        }

        private static void Foo()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.Foo");

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(new string[0]);

            Debug.WriteLine($"{DateTimeOffset.Now:s} END AvaloniaPlugin.Foo");
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        protected override void Start()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.Start");
            
            var mainThread = new Thread(Foo);
            mainThread.SetApartmentState(ApartmentState.STA);
            mainThread.Start();
            
            Debug.WriteLine($"{DateTimeOffset.Now:s} END AvaloniaPlugin.Start");
        }

        protected override void Stop()
        {
        }
    }
}
