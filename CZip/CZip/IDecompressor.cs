using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip
{
    public interface IDecompressor
    {
        Task<Stream> Decompress(Stream input);
    }
}
