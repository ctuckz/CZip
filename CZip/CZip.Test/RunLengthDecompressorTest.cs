using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CZip.Test
{
    [TestFixture]
    [TestOf(typeof(RunLengthDecompressor))]
    public class RunLengthDecompressorTest
    {
        private const byte UNKNOWN_VERSION = 255;
        [Test]
        public async Task Decompress__EmptyInput()
        {
            RunLengthDecompressor decompressor = new RunLengthDecompressor();
            using (Stream output = await decompressor.DecompressAsync(Stream.Null))
            {
                Assert.That(output, Is.Not.Null);
                Assert.That(output.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void Decompress__UnknownVersion()
        {
            RunLengthDecompressor decompressor = new RunLengthDecompressor();
            using (Stream input = new MemoryStream(new[] { UNKNOWN_VERSION }))
            {
                Assert.That(() => decompressor.DecompressAsync(input), Throws.InvalidOperationException);
            }
        }

        [Test]
        public async Task Decompress__ReturnedStreamAtPos0()
        {
            RunLengthDecompressor decompressor = new RunLengthDecompressor();

            using (Stream input = new MemoryStream(new byte[] { 1, 2, 3 }))
            using (Stream output = await decompressor.DecompressAsync(input))
            {
                Assert.That(output.Position, Is.EqualTo(0));
            }
        }

        [Test]
        [TestCase(new byte[] { 1, 2, 3 },ExpectedResult = new byte[] { 3, 3 })]
        [TestCase(new byte[] { 1, 1, 1, 1, 1 }, ExpectedResult = new byte[] { 1, 1 })]
        [TestCase(new byte[] { 1, 2, 255, 3, 254, 1, 253 }, ExpectedResult = new byte[] { 255, 255, 254, 254, 254, 253 })]
        public async Task<byte[]> Decompress(byte[] inputBytes)
        {
            RunLengthDecompressor decompressor = new RunLengthDecompressor();

            using (Stream input = new MemoryStream(inputBytes))
            using (Stream output = await decompressor.DecompressAsync(input))
            {
                Assume.That(output.Position, Is.EqualTo(0));

                byte[] outputBytes = new byte[output.Length];
                await output.ReadAsync(outputBytes, 0, outputBytes.Length);
                return outputBytes;
            }
        }
    }
}
