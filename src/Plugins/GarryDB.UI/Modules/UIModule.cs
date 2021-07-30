using Autofac;

using Avalonia;

using GarryDB.UI.ViewModels;

using ReactiveUI;

namespace GarryDB.UI.Modules
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
