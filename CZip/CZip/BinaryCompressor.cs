using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZip
{
    public class BinaryCompressor : ICompressor
    {
        private const byte VERSION_FLAG = 0x1;

        public async Task<Stream> CompressAsync(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Stream output = new MemoryStream();
            try
            {
                // First byte is the version
                output.WriteByte(VERSION_FLAG);

                await compressStream(input, output);

                //AliasPatterns(output);

                // Move the stream to the beginning before returning it
                output.Position = 0;
            }
            catch
            {
                output.Dispose();
                throw;
            }

            return output;
        }

        private static async Task compressStream(Stream input, Stream output)
        {
            int outputOffset = (int)output.Position;
            byte[] buffer = new byte[128 < input.Length ? 128 : input.Length];
            while (await input.ReadAsync(buffer, 0, buffer.Length) > 0)
            {
                byte[] compressedSlice = await compressSlice(buffer);
                await output.WriteAsync(compressedSlice, 0, compressedSlice.Length);
                outputOffset = compressedSlice.Length;
            }
        }

        private static async Task<byte[]> compressSlice(byte[] slice)
        {
            // Compress the input based on sequences of 1s and 0s. Use the high bit to mark whether this byte is a 
            // sequence of 1s or 0s, and the rest of the byte as the count.
            // i.e. We want to turn 0000 1110 into 0000 0100 1000 0011 0000 0001
            
            BitArray bitArray = new BitArray(slice);

            Stream output = new MemoryStream();
            try
            {
                bool pattern = bitArray[bitArray.Length - 1];
                int contiguousCount = 1;
                for (int i = bitArray.Length - 2; i >= 0; i--)
                {
                    bool current = bitArray[i];
                    if (current == pattern)
                    {
                        contiguousCount++;
                        if (contiguousCount >= 1 << 6)
                        {                          
                             output.WriteByte((byte)(GetHighBit(pattern) | contiguousCount));                           
                        }
                    }
                    else
                    {
                        output.WriteByte((byte)(GetHighBit(pattern) | contiguousCount));
                        pattern = current;

                        contiguousCount = 1;
                    }
                }

                output.WriteByte((byte)(GetHighBit(pattern) | contiguousCount));

                // Read output stream into return buffer
                output.Position = 0;
                byte[] buffer = new byte[output.Length];
                await output.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
            catch
            {
                output.Dispose();
                throw;
            }         
        }

        private static byte GetHighBit(bool pattern)
        {
            return (byte)(pattern ? 1 << 7 : 0);
        }

        private static Stream AliasPatterns(Stream input)
        {
            MemoryStream output = new MemoryStream();
            try
            {
                // Count the most common bytes
                Dictionary<int, int> occurences = new Dictionary<int, int>();
                int streamVal;
                while((streamVal = input.ReadByte()) != -1)
                {
                    int val;
                    if (occurences.TryGetValue(streamVal, out val))
                    {
                        occurences[streamVal] = val++;
                    }
                    else
                    {
                        occurences[streamVal] = 1;
                    }
                }

                // And alias them into smaller values


                return null;
            }
            catch
            {
                output.Dispose();
                throw;
            }
        }
    }
}
