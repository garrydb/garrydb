using System;

using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace ExtendAvalonia.Styles
{
    internal sealed class FirstViewStyle : Style
    {
        public FirstViewStyle()
        {
            Selector = Garry("UIPlugin.Views.FirstView");
            Setters.Add(new Setter(TemplatedControl.TemplateProperty, new DynamicResourceExtension("ForTheFirstView_Oud")));
            // Setters.Add(new Setter(TemplatedControl.TemplateProperty, new ControlTemplate
            // {
            // Content = new DynamicResourceExtension("ForTheFirstView")
            // }));
            // Setters.Add(new Setter(UserControl.ContentProperty, new DynamicResourceExtension("ForTheFirstView")));
        }

        private static Selector Garry(string type)
        {
            return default(Selector).Is(Type.GetType($"{type}, UIPlugin"));
        }
    }
}
