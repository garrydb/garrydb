using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Styling;

namespace UIPlugin.Shared
{
    public interface Extension
    {
        IEnumerable<IStyle> Styles { get; }
        IEnumerable<IResourceProvider> Resources { get; }
    }
}
