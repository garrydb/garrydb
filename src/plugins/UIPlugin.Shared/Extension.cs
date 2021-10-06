using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Styling;

namespace UIPlugin.Shared
{
    /// <summary>
    ///     Extends the UI.
    /// </summary>
    public interface Extension
    {
        /// <summary>
        ///     Gets the styles to apply.
        /// </summary>
        IEnumerable<IStyle> Styles { get; }

        /// <summary>
        ///     Gets the resources to add.
        /// </summary>
        IEnumerable<IResourceProvider> Resources { get; }
    }
}
