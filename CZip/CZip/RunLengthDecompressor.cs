using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip
{
    public class RunLengthDecompressor : IDecompressor
    {
        public async Task<Stream> Decompress(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            int iVersion = input.ReadByte();
            if (iVersion == -1)
            {
                // The input stream was empty
                return Stream.Null;
            }

            // This is a safe conversion - the int will always fit into a byte unless it is -1
            byte version = Convert.ToByte(iVersion);
            switch (version)
            {
                case RunLengthCompressor.VERSION_ONE:
                    return await DecompressV1(input);
                default:
                    throw new InvalidOperationException($"Unknown version: {version}");
            }
        }

        private static async Task<Stream> DecompressV1(Stream input)
        {
            Stream output = new MemoryStream();
            try
            {
                byte[] buffer = new byte[2];
                while ((await input.ReadAsync(buffer, 0, 2)) > 0)
                {
                    byte runLength = buffer[0];
                    byte runValue = buffer[1];

                    byte[] bytes = Enumerable.Repeat(runValue, runLength).ToArray();
                    await output.WriteAsync(bytes, 0, bytes.Length);
                }

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
