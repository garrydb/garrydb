using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GarryDB.Avalonia.ViewModels;
using GarryDB.Avalonia.Views;
using ReactiveUI;
using Splat;

namespace GarryDB.Avalonia
{
    /// <summary>
    /// 
    /// </summary>
    public class Hack
    {
        private static AppBuilder BuildAvaloniaApp()
        {
            var context = AvaloniaPlugin.Context;
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Foo()
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(new string[0]);
        }
    }
}