using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip.Shell
{
    /// <summary>
    /// Handles decompression in a console environment
    /// </summary>
    public interface IConsoleDecompressor
    {
        /// <summary>
        /// Decompresses a file read from the supplied path using the specified algorithm
        /// </summary>
        /// <param name="algorithm">The algorithm used to decompress the file.</param>
        /// <param name="path">The path of the file that will be decompressed.</param>
        /// <returns>A stream containing the decompressed file.</returns>
        Task<Stream> DecompressAsync(Algorithm algorithm, string path);
    }
}
