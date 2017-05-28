using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CZip;

namespace CZip.Shell
{
    class Program
    {
        private const string COMPRESS = "-c";
        private const string DECOMPRESS = "-d";

        private const string ALGORITHM = "-a";
        private const string ALGORITHM_RUNLENGTH = "rl";

        private const string INPUT = "-i";

        private const string OUTPUT = "-o";
       
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
            string inputPath = null;
            string outputPath = null;

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
                            Console.WriteLine($"Unknown algorithm: {args[i]}");
                            return;
                        }
                        break;
                    case INPUT:
                        i++;
                        inputPath = args[i];
                        break;
                    case OUTPUT:
                        i++;
                        outputPath = args[i];
                        break;
                    case COMPRESS:
                        compress = true;
                        break;
                    case DECOMPRESS:
                        compress = false;
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {args[i]}");
                        return;
                }
                i++;
            }

            // Validate inputs
            if (alg == Algorithm.Unknown || string.IsNullOrWhiteSpace(inputPath))
            {
                Console.WriteLine("Algorithm and input path must be set");
                return;
            }
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                Console.WriteLine("An input path is required. Use -i to set the input path.");
            }
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"Could not find file: {inputPath}");
            }
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Console.WriteLine("An output path is required. Use -o to set the output path.");
            }
            if (File.Exists(outputPath))
            {
                bool? overwrite = null;
                do
                {
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write($"A file already exists at {outputPath}. Overwrite it? (Y/N): ");
                    Console.ForegroundColor = originalColor;

                    string entry = Console.ReadKey().KeyChar.ToString();
                    Console.WriteLine();

                    if (string.Equals(entry, "y", StringComparison.CurrentCultureIgnoreCase))
                    {
                        overwrite = true;
                    }
                    else if (string.Equals(entry, "n", StringComparison.CurrentCultureIgnoreCase))
                    {
                        overwrite = false;
                    }
                }
                while (overwrite == null);

                if (overwrite != true)
                {
                    return;
                }
            }

            if (compress)
            {
                // Compress input
                // TODO: Set up DI container so we can DI all this stuff
                using (Stream output = await new ConsoleCompressor(new AlgorithmFactory()).CompressAsync(alg, inputPath))          
                using (FileStream outStream = new FileStream(outputPath, FileMode.Create))
                {
                    await output.CopyToAsync(outStream);
                }            
            }
            else
            {
                // Decompress the input
                // TODO: DI this too
                using (Stream output = await new ConsoleDecompressor(new AlgorithmFactory()).DecompressAsync(alg, inputPath))
                using (FileStream outStream = new FileStream(outputPath, FileMode.Create))
                {
                    await output.CopyToAsync(outStream);
                }
            }
        }
    }
}
