using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip.Console
{
    public static class AlgorithmFactory
    {
        public static ICompressor GetCompressor(Algorithm alg)
        {
            switch (alg)
            {
                case Algorithm.Runlegnth:
                    return new RunLengthCompressor();
                default:
                case Algorithm.Unknown:
                    throw new ArgumentException(nameof(alg));
            }
        }
    }
}
