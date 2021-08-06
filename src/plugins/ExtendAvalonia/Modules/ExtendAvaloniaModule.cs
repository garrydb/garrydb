using Autofac;

using UIPlugin.Shared;

namespace ExtendAvalonia.Modules
{
    internal sealed class ExtendAvaloniaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UIExtension>().As<Extension>().InstancePerDependency();
        }
    }
}
