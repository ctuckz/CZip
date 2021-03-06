﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CZip.Test
{
    [TestFixture]
    [TestOf(typeof(RunLengthCompressor))]
    public class RunLengthCompressorTest
    {
        [Test]
        [TestCase(new byte[0], TestName = "CompressTest__EmptyInput", ExpectedResult = new byte[] { 0x1 })]
        [TestCase(new byte[] { 0x2 }, TestName = "CompressTest__SingleValue", ExpectedResult = new byte[] { 0x1, 0x1, 0x2 })]
        [TestCase(new byte[] { 0x2, 0x2, 0x2, 0x2 }, TestName = "CompressTest__RepeatingValue", ExpectedResult = new byte[] { 0x1, 0x4, 0x2 })]
        [TestCase(new byte[] { 0x2, 0x2, 0x3, 0x2, 0x3 }, TestName = "CompressTest__MixedRun", ExpectedResult = new byte[] { 0x1, 0x2, 0x2, 0x1, 0x3, 0x1, 0x2, 0x1, 0x3 })]
        public async Task<byte[]> CompressTest(byte[] input)
        {
            RunLengthCompressor compressor = new RunLengthCompressor();
            using (MemoryStream inputStream = new MemoryStream(input))
            using (Stream output = await compressor.CompressAsync(inputStream))
            {

                byte[] buffer = new byte[output.Length];
                await output.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        [Test]
        public async Task Compress__ReturnedStreamAtPos0()
        {
            RunLengthCompressor compressor = new RunLengthCompressor();

            using (MemoryStream input = new MemoryStream(new byte[] { 2, 2 }))
            using (Stream output = await compressor.CompressAsync(input))
            {
                Assert.That(output.Position, Is.EqualTo(0));
            }
        }
    }
}
