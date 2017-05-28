using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CZip;
using Con = System.Console;

namespace CZip.Console
{
    class Program
    {
        private const string COMPRESS = "-c";
        private const string DECOMPRESS = "-d";

        private const string ALGORITHM = "-a";
        private const string ALGORITHM_RUNLENGTH = "rl";

        private const string INPUT = "-i";
       
        static void Main(string[] args)
        {
            // Console apps don't support async entry points, so we have to manually wait the task
            Task t = MainAsync(args);
            t.Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            bool compress = false;
            Algorithm alg = Algorithm.Unknown;
            string input = null;

            int i = 0;
            while(i < args.Length)
            {
                switch (args[i])
                {
                    case ALGORITHM:
                        i++;
                        if (string.Equals(args[i], ALGORITHM_RUNLENGTH, StringComparison.CurrentCultureIgnoreCase))
                        {
                            alg = Algorithm.Runlegnth;
                        }
                        else
                        {
                            Con.WriteLine($"Unknown algorithm: {args[i]}");
                            return;
                        }
                        break;
                    case INPUT:
                        i++;
                        input = args[i];
                        break;
                    case COMPRESS:
                        compress = true;
                        break;
                    case DECOMPRESS:
                        compress = false;
                        break;
                    default:
                        Con.WriteLine($"Unknown command: {args[i]}");
                        return;
                }
                i++;
            }

            // Validate inputs
            if (alg == Algorithm.Unknown || string.IsNullOrWhiteSpace(input))
            {
                Con.WriteLine("Algorithm and input path must be set");
                return;
            }
            if (!File.Exists(input))
            {
                Con.WriteLine($"Could not find file: {input}");
            }

            if (compress)
            {
                // Compress input
                switch (alg)
                {
                    case Algorithm.Runlegnth:
                        using (Stream output = await RunLengthCompress(input))
                        {
                            // TODO
                        }
                        break;
                    case Algorithm.Unknown:
                    default:
                        throw new Exception($"Invalid algorithm: {alg.ToString()}");
                }
            }
            else
            {
                // Decompress the input

            }
        }

        private static async Task<Stream> RunLengthCompress(string inputPath)
        {
            using (FileStream s = File.OpenRead(inputPath))
            {
                long originalSize = s.Length;
                Con.WriteLine($"Original size: {originalSize} bytes");
                Con.Write("Compressing...");

                RunLengthCompressor compressor = new RunLengthCompressor();
                Stream output = await compressor.CompressAsync(s);
                try
                {                
                    Con.WriteLine("\nDone");
                    long compressedLength = output.Length;

                    ConsoleColor originalColor = Con.ForegroundColor;
                    Con.ForegroundColor = ConsoleColor.DarkYellow;

                    Con.WriteLine($"Output size: {compressedLength} bytes");
                    Con.WriteLine($"Compression ratio: {(compressedLength * 100) / originalSize}%");
                    
                    Con.ForegroundColor = originalColor;

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
