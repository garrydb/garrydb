using System;
using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

using UIPlugin.Shared;

namespace ExtendAvalonia
{
    internal sealed class UIExtension : Extension
    {
        public IEnumerable<IStyle> Styles
        {
            get
            {
                yield return new StyleInclude(new Uri("avares://ExtendAvalonia/"))
                             {
                                 Source = new Uri("Resources/Styles.axaml", UriKind.Relative)
                             };
            }
        }

        public IEnumerable<IResourceProvider> Resources
        {
            get
            {
                yield return new ResourceInclude
                             {
                                 Source = new Uri("avares://ExtendAvalonia/Resources/ResourceDictionary.axaml")
                             };
            }
        }
    }
}
