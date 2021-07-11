using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using GarryDB.Avalonia.ViewModels;
using GarryDB.Avalonia.Views;
using GarryDb.Plugins;

namespace GarryDB.Avalonia
{
    public class AvaloniaPlugin : Plugin
    {
        static AvaloniaPlugin()
        {
            var mainThread = new Thread(Foo);
#pragma warning disable CA1416
            mainThread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
            mainThread.Start();
        }
        
        private static void Foo()
        {
            AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .LogToTrace()
                    .UseReactiveUI()
                    .StartWithClassicDesktopLifetime(new string[0], ShutdownMode.OnExplicitShutdown);
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}