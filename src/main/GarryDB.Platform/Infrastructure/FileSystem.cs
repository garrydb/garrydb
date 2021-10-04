using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        ///     Determines whether a directory or file exists at <paramref name="path" />.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the directory or file exists, otherwise <c>false</c>.</returns>
        bool Exists(string path);

        /// <summary>
        ///     Create the directory.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        void CreateDirectory(string directory);

        /// <summary>
        ///     Load the file into a <see cref="Stream" />.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A <see cref="Stream" /> containing the file.</returns>
        Stream LoadFile(string path);
    }
}
