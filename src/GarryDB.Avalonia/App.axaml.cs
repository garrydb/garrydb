using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Shared.PlatformSupport;
using Avalonia.Themes.Fluent;
using GarryDB.Avalonia.ViewModels;
using GarryDB.Avalonia.Views;
using ReactiveUI;
using Splat;

namespace GarryDB.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            var styles = new FluentTheme((Uri) null!);
            var field = styles.GetType().GetField("_loaded", BindingFlags.NonPublic | BindingFlags.Instance);
            var type = field!.FieldType;
                
            var styles2 = AvaloniaXamlLoader.Load(new Uri("avares://Avalonia.Themes.Fluent/FluentLight.xaml", UriKind.Absolute), null);

            var xxx = AvaloniaPlugin.Context;
            global::Avalonia.Styling.IStyle ss = styles.Loaded;
            AvaloniaXamlLoader.Load(this);
            //this.Styles.Add(ss);
        }
        
        private class MyAssetLoader : IAssetLoader
        {
            private readonly IAssetLoader inner;
            public void SetDefaultAssembly(Assembly assembly)
            {
                inner.SetDefaultAssembly(assembly);
            }

            public bool Exists(Uri uri, Uri baseUri = null)
            {
                return inner.Exists(uri, baseUri);
            }

            public Stream Open(Uri uri, Uri baseUri = null)
            {
                return inner.Open(uri, baseUri);
            }

            public (Stream stream, Assembly assembly) OpenAndGetAssembly(Uri uri, Uri baseUri = null)
            {
                return inner.OpenAndGetAssembly(uri, baseUri);
            }

            public Assembly GetAssembly(Uri uri, Uri baseUri = null)
            {
                Assembly assembly = inner.GetAssembly(uri, baseUri);
                if (assembly != null)
                {
                    var result = AvaloniaPlugin.Context.Assemblies
                               .SingleOrDefault(x => x.GetName().Name == assembly.GetName().Name)
                           ?? assembly;
                    return result;
                }

                return assembly;
            }

            public IEnumerable<Uri> GetAssets(Uri uri, Uri baseUri)
            {
                return inner.GetAssets(uri, baseUri);
            }

            public MyAssetLoader()
            {
                inner = new AssetLoader();
            }
        }

        public override void RegisterServices()
        {
            base.RegisterServices();
            Locator.CurrentMutable.RegisterViewsForViewModels(GetType().Assembly);
            var vl = ReactiveUI.ViewLocator.Current;
            AvaloniaLocator.CurrentMutable
                .Bind<IAssetLoader>().ToConstant(new MyAssetLoader());
            Locator.CurrentMutable.Register(() => new FirstView(), typeof(IViewFor<FirstViewModel>));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}