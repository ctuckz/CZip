using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip.Shell
{
    internal class ConsoleDecompressor : IConsoleDecompressor
    {
        public ConsoleDecompressor(IAlgorithmFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Factory = factory;
        }

        private IAlgorithmFactory Factory { get; }

        public async Task<Stream> DecompressAsync(Algorithm algorithm, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            Console.WriteLine("Decompressing...");
            using (FileStream s = File.OpenRead(path))
            {
                IDecompressor decompressor = Factory.GetDecompressor(algorithm);

                Stream output = await decompressor.DecompressAsync(s);
                try
                {
                    Console.WriteLine("Done");
                    return output;
                }
                catch
                {
                    output.Dispose();
                    throw;
                }
            }
        }
    }
}
