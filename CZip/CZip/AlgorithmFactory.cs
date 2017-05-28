using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip
{
    public class AlgorithmFactory : IAlgorithmFactory
    {
        public ICompressor GetCompressor(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.Runlegnth:
                    return new RunLengthCompressor();
                default:
                case Algorithm.Unknown:
                    throw new ArgumentException(nameof(algorithm));
            }
        }

        public IDecompressor GetDecompressor(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.Runlegnth:
                    return new RunLengthDecompressor();
                default:
                case Algorithm.Unknown:
                    throw new ArgumentException(nameof(algorithm));
            }
        }
    }
}
