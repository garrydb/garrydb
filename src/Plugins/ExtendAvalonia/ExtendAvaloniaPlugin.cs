using System;

using Autofac;

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

using GarryDb.Plugins;

namespace ExtendAvalonia
{
    public class ExtendAvaloniaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BackgroundColorStyle>().As<Style>().InstancePerDependency();
        }
    }
    
    public class BackgroundColorStyle : Style
    {
        public BackgroundColorStyle()
        {
            var type = Type.GetType("GarryDB.UI.Views.FirstView, GarryDB.UI");

            Selector = default(Selector).OfType(type);
            Setters.Add(new Setter(TemplatedControl.BackgroundProperty, Brush.Parse("#FF0000")));
        }
        
        
    }
    
    public class ExtendAvaloniaPlugin : Plugin
    {
        public ExtendAvaloniaPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
        }
    }
}
