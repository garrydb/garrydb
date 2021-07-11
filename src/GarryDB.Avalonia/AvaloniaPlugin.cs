using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaLib = Avalonia;

using GarryDB.Avalonia.ViewModels;
using GarryDB.Avalonia.Views;
using GarryDb.Plugins;

namespace GarryDB.Avalonia
{
    using AvaloniaLib.Collections;
    using AvaloniaLib.Controls;
    using AvaloniaLib.Controls.ApplicationLifetimes;
    using AvaloniaLib.ReactiveUI;
    using AvaloniaLib.Styling;
    using AvaloniaLib.Themes.Fluent;
    using AvaloniaLib.Threading;
    public class AvaloniaPlugin : Plugin
    {
        public static AssemblyLoadContext Context;
        
        public AvaloniaPlugin(AssemblyLoadContext context)
        {
            Context = context;
        }

        private static void Foo()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.Foo");

            var type = Type.GetType("GarryDB.Avalonia.Hack")!;
            dynamic hack = Activator.CreateInstance(type)!;
            hack.Foo();
            Debug.WriteLine($"{DateTimeOffset.Now:s} END AvaloniaPlugin.Foo");
            
            // AppBuilder.Configure<App>()
            //     .UsePlatformDetect()
            //     .LogToTrace()
            //     .UseReactiveUI()
            //     .SetupWithoutStarting();
            // .StartWithClassicDesktopLifetime(new string[0], ShutdownMode.OnExplicitShutdown);
        }

        public override void Start()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.Start");
            var mainThread = new Thread(Foo);
#pragma warning disable CA1416
            mainThread.SetApartmentState(ApartmentState.STA);
#pragma warning restore CA1416
            mainThread.Start();
            Debug.WriteLine($"{DateTimeOffset.Now:s} END AvaloniaPlugin.Start");
        }

        public override void AllStarted()
        {
            Debug.WriteLine($"{DateTimeOffset.Now:s} BEGIN AvaloniaPlugin.AllStarted");
        }

        public override void Stop()
        {
        }
    }
}