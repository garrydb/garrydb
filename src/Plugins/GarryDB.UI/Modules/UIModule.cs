using System.Diagnostics;

using Autofac;

using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Threading;

using GarryDB.UI.ViewModels;

using ReactiveUI;

using Splat;
using Splat.Autofac;

namespace GarryDB.UI.Modules
{
    public class UIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Debug.WriteLine("Loading Autofac module");

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsClosedTypesOf(typeof(IViewFor<>))
                .InstancePerDependency();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<ViewModel>()
                .InstancePerDependency();

            builder
                .RegisterType<App>()
                .AsSelf()
                .As<Application>()
                .SingleInstance();

            AutofacDependencyResolver resolver = builder.UseAutofacDependencyResolver();
            builder.RegisterInstance(resolver);
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            resolver.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            builder.RegisterBuildCallback(scope => resolver.SetLifetimeScope(scope));
        }
    }
}
