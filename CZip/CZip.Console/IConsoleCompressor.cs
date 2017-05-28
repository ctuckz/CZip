using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip.Shell
{
    public interface IConsoleCompressor
    {
        Task<Stream> Compress(Algorithm algorithm, string path);
    }
}
