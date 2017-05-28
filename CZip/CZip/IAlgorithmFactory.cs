using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip
{
    public interface IAlgorithmFactory
    {
        ICompressor GetCompressor(Algorithm algorithm);
        IDecompressor GetDecompressor(Algorithm algorithm);
    }
}
