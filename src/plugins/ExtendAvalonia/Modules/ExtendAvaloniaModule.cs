using Autofac;

using UIPlugin.Shared;

namespace ExtendAvalonia.Modules
{
    public class ExtendAvaloniaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UIExtension>().As<Extension>().InstancePerDependency();
        }
    }
}
