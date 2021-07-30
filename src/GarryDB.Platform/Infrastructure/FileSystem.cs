using System.Collections.Generic;

namespace GarryDB.Platform.Infrastructure
{
    /// <summary>
    ///     Interacts with the file system.
    /// </summary>
    public interface FileSystem
    {
        /// <summary>
        ///     Returns the full paths of the files in the <paramref name="directory" /> that matches <paramref name="pattern" />.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>The files in the <paramref name="directory" /> that matches <paramref name="pattern" />.</returns>
        IEnumerable<string> GetFiles(string directory, string pattern = "*.*");

        /// <summary>
        ///     Returns the full paths of the directories found in <paramref name="directory" />.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <returns>The directories in <paramref name="directory" />.</returns>
        IEnumerable<string> GetTopLevelDirectories(string directory);
    }
}
