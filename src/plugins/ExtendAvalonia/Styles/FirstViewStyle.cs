using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

using UIPlugin.Shared;

namespace ExtendAvalonia.Styles
{
    internal sealed class FirstViewStyle : Style
    {
        public FirstViewStyle()
        {
            Selector = new TypeSelector("GarryDb.Avalonia.Host.Views.FirstView");
            Setters.Add(new Setter(TemplatedControl.TemplateProperty, new DynamicResourceExtension("ForTheFirstView_Oud")));
        }
    }
}
