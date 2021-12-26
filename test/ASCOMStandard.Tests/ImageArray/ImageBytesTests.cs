using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ASCOM.Common.Alpaca;
using Xunit.Abstractions;
using System.Runtime.InteropServices;

namespace ASCOM.Alpaca.Tests.ImageArray
{
    public class ImageBytesTests
    {
        private ITestOutputHelper output;

        public ImageBytesTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ImageArray2DByte()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int32[,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = i + 10 * j;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Byte);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[12] { 0, 10, 20, 1, 11, 21, 2, 12, 22, 3, 13, 23 }));
        }

        [Fact]
        public void ImageArray2DInt16()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int32[,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = -2 + i + 10 * j;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int16);


            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[24] { 254, 255, 8, 0, 18, 0, 255, 255, 9, 0, 19, 0, 0, 0, 10, 0, 20, 0, 1, 0, 11, 0, 21, 0 }));
        }

        [Fact]
        public void ImageArray2DUInt16()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int32[,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = 32768 + i + 10 * j;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt16);


            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[24] { 0, 128, 10, 128, 20, 128, 1, 128, 11, 128, 21, 128, 2, 128, 12, 128, 22, 128, 3, 128, 13, 128, 23, 128 }));
        }

        [Fact]
        public void ImageArray2DInt32()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int32[,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = -32769 + i + 10 * j;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int32);


            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[48] { 255, 127, 255, 255, 9, 128, 255, 255, 19, 128, 255, 255, 0, 128, 255, 255, 10, 128, 255, 255, 20, 128, 255, 255, 1, 128, 255, 255, 11, 128, 255, 255, 21, 128, 255, 255, 2, 128, 255, 255, 12, 128, 255, 255, 22, 128, 255, 255 }));
        }

        [Fact]
        public void ImageArray2DUInt32()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            UInt32[,] imageArray = new UInt32[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (uint)(2147483648 + i + 10 * j);
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt32);


            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[48] { 0, 0, 0, 128, 10, 0, 0, 128, 20, 0, 0, 128, 1, 0, 0, 128, 11, 0, 0, 128, 21, 0, 0, 128, 2, 0, 0, 128, 12, 0, 0, 128, 22, 0, 0, 128, 3, 0, 0, 128, 13, 0, 0, 128, 23, 0, 0, 128 }));
        }

        [Fact]
        public void ArrayMetadataV1()
        {


            ArrayMetadataV1 metadata = new ArrayMetadataV1(AlpacaErrors.AlpacaNoError, 128, 255, ImageArrayElementTypes.Int32, ImageArrayElementTypes.UInt16, 3, 4, 3, 3);
            byte[] metadataBytes = metadata.ToByteArray<ArrayMetadataV1>();

            for (int i = 0; i < metadataBytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {metadataBytes[i]}");
            }

            Assert.True(CompareBytes(metadataBytes, new byte[44] { 1, 0, 0, 0, 0, 0, 0, 0, 128, 0, 0, 0, 255, 0, 0, 0, 44, 0, 0, 0, 2, 0, 0, 0, 8, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0 }));
        }



        private bool CompareBytes(byte[] supplied, byte[] required)
        {
            //if (required.Length != supplied.Length) return false;
            for (int i = 0; i < required.Length; i++)
            {
                // output.WriteLine($"CompareBytes: {i}: {supplied[i]} {required[i]}");
                if (required[i] != supplied[i])
                {
                    // output.WriteLine($"CompareBytes: Returning FALSE");
                    return false;
                }
            }

            // output.WriteLine($"CompareBytes: Returning TRUE");
            return true;
        }

    }
}
