using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using GarryDB.Avalonia.ViewModels;
using GarryDB.Avalonia.Views;

namespace GarryDB.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new SplashScreenWindow
                {
                    DataContext = new SplashScreenWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}