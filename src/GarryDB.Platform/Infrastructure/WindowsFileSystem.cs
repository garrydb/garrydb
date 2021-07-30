using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace GarryDB.Platform.Infrastructure
{
    /// <summary>
    ///     The <see cref="FileSystem" /> for Windows.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class WindowsFileSystem : FileSystem
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
