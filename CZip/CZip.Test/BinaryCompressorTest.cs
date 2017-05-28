using System;
using System.IO;
using System.Threading.Tasks;
using CZip;
using NUnit.Framework;

namespace CZip
{
    [TestFixture]
    public class BinaryCompressorTest
    {
        [Test]
        [TestCase(new byte[0], TestName = "CompressTest__EmptyInput", ExpectedResult = new byte[] { 0x1 })]
        [TestCase(new byte[] { 0xE }, TestName = "CompressTest__OneByteInput", ExpectedResult = new byte[] { 0x1, 0x4, 0x83, 0x1 })]
        public async Task<byte[]> CompressTest(byte[] input)
        {
            BinaryCompressor compressor = new BinaryCompressor();
            using (MemoryStream ms = new MemoryStream(input))
            {
                using (Stream output = await compressor.CompressAsync(ms))
                {
                    byte[] buffer = new byte[output.Length];
                    output.Read(buffer, 0, buffer.Length);
                    return buffer;
                }                
            }
        }

        [Test]
        public async Task RealFileTest()
        {
            BinaryCompressor compressor = new BinaryCompressor();

            using (Stream input = File.OpenRead(@"C:\Users\ctuck\Desktop\Wedding Photos\715B1081.jpg"))
            {
                using (Stream output = await compressor.CompressAsync(input))
                {
                    byte[] buffer = new byte[output.Length];
                    output.Read(buffer, 0, buffer.Length);
                }
            }
        }

        [Test]
        public async Task TextFileTest()
        {
            BinaryCompressor compressor = new BinaryCompressor();

            using (Stream input = File.OpenRead(@"C:\Users\ctuck\OneDrive\Documents\Wedding Invite Counts.txt"))
            {
                using (Stream output = await compressor.CompressAsync(input))
                {
                    byte[] buffer = new byte[output.Length];
                    output.Read(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
