using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using GarryDB.Platform.Infrastructure;

namespace GarryDB.Wpf.Host
{
    /// <summary>
    ///     The <see cref="FileSystem" /> for Windows.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class WindowsFileSystem : FileSystem
    {
        /// <inheritdoc />
        public IEnumerable<string> GetFiles(string directory, string pattern = "*.*")
        {
            return Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetTopLevelDirectories(string directory)
        {
            return Directory.GetDirectories(directory, "*.*", SearchOption.TopDirectoryOnly);
        }
    }
}
