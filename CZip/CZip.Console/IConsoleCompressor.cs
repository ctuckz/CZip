using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip.Shell
{
    /// <summary>
    /// Handles compression in a console environment
    /// </summary>
    public interface IConsoleCompressor
    {
        /// <summary>
        /// Compresses a file using the requested algorithm.
        /// </summary>
        /// <param name="algorithm">The algorithm that will be used to compress the file.</param>
        /// <param name="path">The path to the file.</param>
        /// <returns>A stream containing the compressed file.</returns>
        Task<Stream> CompressAsync(Algorithm algorithm, string path);
    }
}
