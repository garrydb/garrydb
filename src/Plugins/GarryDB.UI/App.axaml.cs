using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using GarryDB.UI.ViewModels;
using GarryDB.UI.Views;

using ReactiveUI;

using Splat;

namespace GarryDB.UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void RegisterServices()
        {
            //var builder = new ContainerBuilder();
            //AutofacDependencyResolver resolver = builder.UseAutofacDependencyResolver();
            //builder.RegisterInstance(resolver);
            //resolver.InitializeReactiveUI();
            
            base.RegisterServices();
            Locator.CurrentMutable.RegisterViewsForViewModels(GetType().Assembly);
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
