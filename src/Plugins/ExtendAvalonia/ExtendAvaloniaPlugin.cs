using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

using GarryDb.Plugins;

namespace ExtendAvalonia
{
    public class ExtendAvaloniaPlugin : Plugin
    {
        public ExtendAvaloniaPlugin(PluginContext pluginContext, Application application)
            : base(pluginContext)
        {
            var type = application.GetType().Assembly.GetType("GarryDB.UI.Views.MainWindow");

            application.Styles.Add(new Style(selector => selector.OfType(type))
            {
                Setters =
                {
                    new Setter(TemplatedControl.BackgroundProperty, Brush.Parse("#FF0000"))
                }
            });
        }
    }
}
