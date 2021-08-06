using Autofac;

using Avalonia.ReactiveUI;
using Avalonia.Threading;

using ReactiveUI;

using Splat;
using Splat.Autofac;

namespace UIPlugin.Modules
{
    public class AvaloniaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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
