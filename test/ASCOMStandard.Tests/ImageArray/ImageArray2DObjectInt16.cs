using ASCOM.Common.Alpaca;
using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace ASCOM.Alpaca.Tests.ImageArray
{
    public class ImageArray2DObjectInt16
    {
        private readonly ITestOutputHelper output;

        public ImageArray2DObjectInt16(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (Int16)((i * j % 65536) - 32768);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int16);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(TestSupport.CompareArrays(imageArray, responseArray, false, output));
        }

    }
}
