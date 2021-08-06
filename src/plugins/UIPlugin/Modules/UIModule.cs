using Autofac;

using Avalonia;

using ReactiveUI;

using UIPlugin.ViewModels;

namespace UIPlugin.Modules
{
    public class UIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly).AsClosedTypesOf(typeof(IViewFor<>)).InstancePerDependency();

            builder.RegisterAssemblyTypes(ThisAssembly).AssignableTo<ViewModel>().InstancePerDependency();

            builder.RegisterType<App>().As<Application>().SingleInstance();
        }
    }
}
