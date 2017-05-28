using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip
{
    /// <summary>
    /// Compresses an input stream by condensing sequences of the same byte. For example, 1111 is compressed to 41 (four 1's).
    /// This compression algorithm typically works well with bitmap images.
    /// </summary>
    public class RunLengthCompressor : ICompressor
    {
        public const byte VERSION_ONE = 0x1;

        public async Task<Stream> CompressAsync(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Stream output = new MemoryStream();
            try
            {
                output.WriteByte(VERSION_ONE);

                int curr;
                int prev = -1;
                int consecutive = 0;
                while ((curr = input.ReadByte()) != -1)
                {
                    if (curr != prev)
                    {
                        if (consecutive > 0)
                        {
                            // Write out our existing run
                            await WriteByteRun(Convert.ToByte(prev), consecutive, output);
                        }
                        // And reset it for the new value
                        prev = curr;
                        consecutive = 1;
                    }
                    else
                    {
                        // Keep counting the run
                        consecutive++;
                    }
                }

                if (consecutive > 0)
                {
                    // Write out the last run in the stream
                    await WriteByteRun(Convert.ToByte(prev), consecutive, output);
                }

                output.Position = 0;
                return output;
            }
            catch
            {
                output.Dispose();
                throw;
            }
        }

        private static async Task WriteByteRun(byte value, int length, Stream output)
        {
            List<byte> runData = new List<byte>();
            do
            {
                int sliceSize = Math.Min(255, length);
                runData.AddRange(new[] { Convert.ToByte(sliceSize), value });
                length -= sliceSize;
            }
            while (length > 0);

            await output.WriteAsync(runData.ToArray(), 0, runData.Count);
        }
    }
}
