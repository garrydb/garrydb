using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Styling;

namespace GarryDB.UI.Shared
{
    public interface Extension
    {
        IEnumerable<IStyle> Styles { get; }
        IEnumerable<IResourceProvider> Resources { get; }
    }
}
