using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip.Shell
{
    internal class ConsoleCompressor : IConsoleCompressor
    {
        public ConsoleCompressor(IAlgorithmFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Factory = factory;
        }

        private IAlgorithmFactory Factory { get; }

        public async Task<Stream> CompressAsync(Algorithm algorithm, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            Console.WriteLine("Compressing...");
            using (FileStream s = File.OpenRead(path))
            {
                long originalSize = s.Length;
                Console.WriteLine($"Original size: {originalSize} bytes");

                ICompressor compressor = Factory.GetCompressor(algorithm);
                Stream output = await compressor.CompressAsync(s);
                try
                {
                    long compressedLength = output.Length;                  
                    Console.WriteLine($"Output size: {compressedLength} bytes");

                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;

                    Console.WriteLine($"Compression ratio: {(compressedLength * 100) / originalSize}%");

                    Console.ForegroundColor = originalColor;

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
