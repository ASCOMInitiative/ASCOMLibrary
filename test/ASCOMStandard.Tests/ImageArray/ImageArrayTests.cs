using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ASCOM.Common.Alpaca;
using Xunit.Abstractions;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ASCOM.Alpaca.Tests.ImageArray
{
    public class ImageArrayTests
    {
        private ITestOutputHelper output;

        public ImageArrayTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region Int16 Tests

        [Fact]
        public void ImageArrayInt162DByte()
        {
            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int16[,] imageArray = new Int16[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (short)(i + 10 * j);
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Byte);
            Assert.True(metadata.ImageElementType == ImageArrayElementTypes.Int16);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[12] { 0, 10, 20, 1, 11, 21, 2, 12, 22, 3, 13, 23 }));

            Int16[,] responseArray = (Int16[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));

        }

        [Fact]
        public void ImageArrayInt162DInt16()
        {
            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int16[,] imageArray = new Int16[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (short)(-1 + i + 10 * j);
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int16);
            Assert.True(metadata.ImageElementType == ImageArrayElementTypes.Int16);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[24]
            {
                255, 255,
                9, 0,
                19, 0,
                0, 0,
                10, 0,
                20, 0,
                1, 0,
                11, 0,
                21, 0,
                2, 0,
                12, 0,
                22, 0
            }));

            Int16[,] responseArray = (Int16[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));

        }

        [Fact]
        public void ImageArrayUInt162DByte()
        {
            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            UInt16[,] imageArray = new UInt16[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (UInt16)(i + 10 * j);
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Byte);
            Assert.True(metadata.ImageElementType == ImageArrayElementTypes.UInt16);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[12] { 0, 10, 20, 1, 11, 21, 2, 12, 22, 3, 13, 23 }));
            UInt16[,] responseArray = (UInt16[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));


        }

        [Fact]
        public void ImageArrayUInt162DUInt16()
        {
            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            UInt16[,] imageArray = new UInt16[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (UInt16)(32768 + i + 10 * j);
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt16);
            Assert.True(metadata.ImageElementType == ImageArrayElementTypes.UInt16);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[24]
            {
                0, 128,
                10, 128,
                20, 128,
                1, 128,
                11, 128,
                21, 128,
                2, 128,
                12, 128,
                22, 128,
                3, 128,
                13, 128,
                23, 128
            }));
            UInt16[,] responseArray = (UInt16[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
        }

        #endregion

        #region Object 2D Tests

        [Fact]
        public void ImageArray2DObjectByte()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (Byte)((i * j) % 256);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Byte);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectInt16()
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
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectUInt16()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (UInt16)((i * j % 65536));
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt16);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }
        [Fact]
        public void ImageArray2DObjectInt32()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = -32769 + i + 10 * j;
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int32);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectUInt32()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (UInt32)(i + 10 * j);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt32);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectInt64()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (Int64)(i + 10 * j);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int64);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectUInt64()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (UInt64)(i + 10 * j);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt64);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectSingle()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (Single)(i + 10 * j);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Single);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray2DObjectDouble()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (Double)(i + 10 * j);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Double);

            sw.Restart();
            Object[,] responseArray = (Object[,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        #endregion

        #region Object 3D Tests

        [Fact]
        public void ImageArray3DObjectByte()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (Byte)k;
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata - ImageArrayElementType: {metadata.ImageElementType}, transmission Type: {metadata.TransmissionElementType}, Array Rank: {metadata.Rank}, Dim1: {metadata.Dimension1}, Dim2: {metadata.Dimension2}, Dim3: {metadata.Dimension3}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Byte);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectInt16()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (Int16)((i * j % 65536) - 32768);
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int16);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectUInt16()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (UInt16)((i * j % 65536));
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt16);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }
        [Fact]
        public void ImageArray3DObjectInt32()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = -32769 + i + 10 * j;
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int32);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectUInt32()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (UInt32)(i + 10 * j);
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt32);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectInt64()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (Int64)(i + 10 * j);
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int64);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectUInt64()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (UInt64)(i + 10 * j);
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt64);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectSingle()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (Single)(i + 10 * j);
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Single);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        [Fact]
        public void ImageArray3DObjectDouble()
        {
            const int IMAGE_WIDTH = 4000;
            const int IMAGE_HEIGHT = 3000;

            Object[,,] imageArray = new Object[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (Double)(100 * i + 10 * j + k);
                    }
                }
            }

            Stopwatch sw = Stopwatch.StartNew();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            output.WriteLine($"Time to create byte array: {sw.Elapsed.TotalMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            output.WriteLine($"Metadata Image Element type: {metadata.ImageElementType}, Transmission Element Type: {metadata.TransmissionElementType}");
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Double);

            sw.Restart();
            Object[,,] responseArray = (Object[,,])bytes.ToImageArray();
            output.WriteLine($"Time to create return array: {sw.Elapsed.TotalMilliseconds:0.0}");
            Assert.True(CompareArrays(imageArray, responseArray, false));
        }

        #endregion

        #region ImageArray.ToByteArray() 2D Tests

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

            Int32[,] responseArray = (Int32[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
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

            Int32[,] responseArray = (Int32[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
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

            Int32[,] responseArray = (Int32[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
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

            Int32[,] responseArray = (Int32[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
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

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[48]
            {
                0, 0, 0, 128,
                10, 0, 0, 128,
                20, 0, 0, 128,
                1, 0, 0, 128,
                11, 0, 0, 128,
                21, 0, 0, 128,
                2, 0, 0, 128,
                12, 0, 0, 128,
                22, 0, 0, 128,
                3, 0, 0, 128,
                13, 0, 0, 128,
                23, 0, 0, 128
            }));

            UInt32[,] responseArray = (UInt32[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
        }

        [Fact]
        public void ImageArray2DInt64()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            Int64[,] imageArray = new Int64[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = -2147483649 + i + 10 * j;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int64);


            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[96]
            { 255, 255, 255, 127, 255, 255, 255, 255,
                9, 0, 0,128, 255, 255, 255, 255,
                19, 0, 0, 128, 255, 255, 255, 255,
                0, 0, 0, 128, 255, 255, 255, 255,
                10, 0, 0, 128, 255, 255, 255, 255,
                20, 0, 0, 128, 255, 255, 255, 255,
                1, 0, 0, 128, 255, 255, 255, 255,
                11, 0, 0, 128, 255, 255, 255, 255,
                21, 0, 0, 128, 255, 255, 255, 255,
                2, 0, 0, 128, 255, 255, 255, 255,
                12, 0, 0, 128, 255, 255, 255, 255,
                22, 0, 0, 128, 255, 255, 255, 255,
            }));

            Int64[,] responseArray = (Int64[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
        }

        [Fact]
        public void ImageArray2DUInt64()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            UInt64[,] imageArray = new UInt64[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = (UInt64)(4294967296 + i + 10 * j);
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt64);


            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[96]
            {
                0, 0, 0, 0, 1, 0, 0, 0,
                10, 0, 0, 0, 1, 0, 0, 0,
                20, 0, 0, 0, 1, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0, 0,
                11, 0, 0, 0, 1, 0, 0, 0,
                21, 0, 0, 0, 1, 0, 0, 0,
                2, 0, 0, 0, 1, 0, 0, 0,
                12, 0, 0, 0, 1, 0, 0, 0,
                22, 0, 0, 0, 1, 0, 0, 0,
                3, 0, 0, 0, 1, 0, 0, 0,
                13, 0, 0,0, 1, 0, 0, 0,
                23, 0, 0, 0, 1, 0, 0, 0,
            }));

            UInt64[,] responseArray = (UInt64[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
        }

        [Fact]
        public void ImageArray2DSingle()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            const Single SINGLE_VALUE = 2.3456E20F;

            Single[,] imageArray = new Single[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = SINGLE_VALUE;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Single);


            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[48]
            {
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
            }));

            Single[,] responseArray = (Single[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
        }

        [Fact]
        public void ImageArray2DDouble()
        {

            const int IMAGE_WIDTH = 4;
            const int IMAGE_HEIGHT = 3;

            const Double DOUBLE_VALUE = 2.3456E100;

            Double[,] imageArray = new double[IMAGE_WIDTH, IMAGE_HEIGHT];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    imageArray[i, j] = DOUBLE_VALUE;
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Double);


            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[96]
            {
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
            }));

            Double[,] responseArray = (Double[,])bytes.ToImageArray();
            Assert.True(CompareArrays(imageArray, responseArray));
        }

        #endregion

        #region ImageArray.ToByteArray() 3D Tests

        [Fact]
        public void ImageArray3DByte()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            Int32[,,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = k + 10 * j + 100 * i;
                    }
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Byte);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[36]
            {
                0,
                1,
                2,
                10,
                11,
                12,
                20,
                21,
                22,
                30,
                31,
                32,
                100,
                101,
                102,
                110,
                111,
                112,
                120,
                121,
                122,
                130,
                131,
                132,
                200,
                201,
                202,
                210,
                211,
                212,
                220,
                221,
                222,
                230,
                231,
                232,
            }));
        }

        [Fact]
        public void ImageArray3DInt16()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            Int32[,,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = -1 + k + 10 * j + 100 * i;
                    }
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int16);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[72]
            {
                255, 255,
                0, 0,
                1, 0,
                9, 0,
                10, 0,
                11, 0,
                19, 0,
                20, 0,
                21, 0,
                29, 0,
                30, 0,
                31, 0,
                99, 0,
                100, 0,
                101, 0,
                109, 0,
                110, 0,
                111, 0,
                119, 0,
                120, 0,
                121, 0,
                129, 0,
                130, 0,
                131, 0,
                199, 0,
                200, 0,
                201, 0,
                209, 0,
                210, 0,
                211, 0,
                219, 0,
                220, 0,
                221, 0,
                229, 0,
                230, 0,
                231, 0
            }));
        }

        [Fact]
        public void ImageArray3DUInt16()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Int32[,,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = 32768 + k + 10 * j + 100 * i;
                    }
                }
            }
            // output.WriteLine($"Initialisation time:{sw.Elapsed.TotalMilliseconds:0.000}");

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            // output.WriteLine($"ToByteArray call time:{sw.Elapsed.TotalMilliseconds:0.000}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt16);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[72]
            {
                0, 128,
                1, 128,
                2, 128,
                10, 128,
                11, 128,
                12, 128,
                20, 128,
                21, 128,
                22, 128,
                30, 128,
                31, 128,
                32, 128,
                100, 128,
                101, 128,
                102, 128,
                110, 128,
                111, 128,
                112, 128,
                120, 128,
                121, 128,
                122, 128,
                130, 128,
                131, 128,
                132, 128,
                200, 128,
                201, 128,
                202, 128,
                210, 128,
                211, 128,
                212, 128,
                220, 128,
                221, 128,
                222, 128,
                230, 128,
                231, 128,
                232, 128
            }));
            // output.WriteLine($"Finish time:{sw.Elapsed.TotalMilliseconds:0.000}");

        }

        [Fact]
        public void ImageArray3DInt32()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            Int32[,,] imageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = -32769 + k + 10 * j + 100 * i;
                    }
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int32);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[144]
            {
                255, 127, 255, 255,
                0, 128, 255, 255,
                1, 128, 255, 255,
                9, 128, 255, 255,
                10, 128, 255, 255,
                11, 128, 255, 255,
                19, 128, 255, 255,
                20, 128, 255, 255,
                21, 128, 255, 255,
                29, 128, 255, 255,
                30, 128, 255, 255,
                31, 128, 255, 255,
                99, 128, 255, 255,
                100, 128, 255, 255,
                101, 128, 255, 255,
                109, 128, 255, 255,
                110, 128, 255, 255,
                111, 128, 255, 255,
                119, 128, 255, 255,
                120, 128, 255, 255,
                121, 128, 255, 255,
                129, 128, 255, 255,
                130, 128, 255, 255,
                131, 128, 255, 255,
                199, 128, 255, 255,
                200, 128, 255, 255,
                201, 128, 255, 255,
                209, 128, 255, 255,
                210, 128, 255, 255,
                211, 128, 255, 255,
                219, 128, 255, 255,
                220, 128, 255, 255,
                221, 128, 255, 255,
                229, 128, 255, 255,
                230, 128, 255, 255,
                231, 128, 255, 255
            }));
        }

        [Fact]
        public void ImageArray3DUInt32()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            UInt32[,,] imageArray = new UInt32[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (uint)(2147483648 + k + 10 * j + 100 * i);
                    }
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt32);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[144]
            {
                0, 0, 0, 128,
                1, 0, 0, 128,
                2, 0, 0, 128,
                10, 0, 0, 128,
                11, 0, 0, 128,
                12, 0, 0, 128,
                20, 0, 0, 128,
                21, 0, 0, 128,
                22, 0, 0, 128,
                30, 0, 0, 128,
                31, 0, 0, 128,
                32, 0, 0, 128,
                100, 0, 0, 128,
                101, 0, 0, 128,
                102, 0, 0, 128,
                110, 0, 0, 128,
                111, 0, 0, 128,
                112, 0, 0, 128,
                120, 0, 0, 128,
                121, 0, 0, 128,
                122, 0, 0, 128,
                130, 0, 0, 128,
                131, 0, 0, 128,
                132, 0, 0, 128,
                200, 0, 0, 128,
                201, 0, 0, 128,
                202, 0, 0, 128,
                210, 0, 0, 128,
                211, 0, 0, 128,
                212, 0, 0, 128,
                220, 0, 0, 128,
                221, 0, 0, 128,
                222, 0, 0, 128,
                230, 0, 0, 128,
                231, 0, 0, 128,
                232, 0, 0, 128
            }));

        }

        [Fact]
        public void ImageArray3DInt64()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            Int64[,,] imageArray = new Int64[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = -2147483649 + k + 10 * j + 100 * i;
                    }
                }
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            sw.Stop();
            //output.WriteLine($"ToByteArray call time:{sw.ElapsedMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Int64);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[288]
            {
                255, 255, 255, 127, 255, 255, 255, 255,
                0, 0, 0, 128, 255, 255, 255, 255,
                1, 0, 0, 128, 255, 255, 255, 255,
                9, 0, 0, 128, 255, 255, 255, 255,
                10, 0, 0, 128, 255, 255, 255, 255,
                11, 0, 0, 128, 255, 255, 255, 255,
                19, 0, 0, 128, 255, 255, 255, 255,
                20, 0, 0, 128, 255, 255, 255, 255,
                21, 0, 0, 128, 255, 255, 255, 255,
                29, 0, 0, 128, 255, 255, 255, 255,
                30, 0, 0, 128, 255, 255, 255, 255,
                31, 0, 0, 128, 255, 255, 255, 255,
                99, 0, 0, 128, 255, 255, 255, 255,
                100, 0, 0, 128, 255, 255, 255, 255,
                101, 0, 0, 128, 255, 255, 255, 255,
                109, 0, 0, 128, 255, 255, 255, 255,
                110, 0, 0, 128, 255, 255, 255, 255,
                111, 0, 0, 128, 255, 255, 255, 255,
                119, 0, 0, 128, 255, 255, 255, 255,
                120, 0, 0, 128, 255, 255, 255, 255,
                121, 0, 0, 128, 255, 255, 255, 255,
                129, 0, 0, 128, 255, 255, 255, 255,
                130, 0, 0, 128, 255, 255, 255, 255,
                131, 0, 0, 128, 255, 255, 255, 255,
                199, 0, 0, 128, 255, 255, 255, 255,
                200, 0, 0, 128, 255, 255, 255, 255,
                201, 0, 0, 128, 255, 255, 255, 255,
                209, 0, 0, 128, 255, 255, 255, 255,
                210, 0, 0, 128, 255, 255, 255, 255,
                211, 0, 0, 128, 255, 255, 255, 255,
                219, 0, 0, 128, 255, 255, 255, 255,
                220, 0, 0, 128, 255, 255, 255, 255,
                221, 0, 0, 128, 255, 255, 255, 255,
                229, 0, 0, 128, 255, 255, 255, 255,
                230, 0, 0, 128, 255, 255, 255, 255,
                231, 0, 0, 128, 255, 255, 255, 255
            }));

        }

        [Fact]
        public void ImageArray3DUInt64()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            UInt64[,,] imageArray = new UInt64[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = (UInt64)(4294967296 + k + 10 * j + 100 * i);
                    }
                }
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            sw.Stop();
            //output.WriteLine($"ToByteArray call time:{sw.ElapsedMilliseconds:0.0}");

            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.UInt64);

            for (int i = 0; i < bytes.Length; i++)
            {
                //output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[288]
            {
                0, 0, 0, 0, 1, 0, 0, 0,
                1, 0, 0, 0, 1, 0, 0, 0,
                2, 0, 0, 0, 1, 0, 0, 0,
                10, 0, 0, 0, 1, 0, 0, 0,
                11, 0, 0, 0, 1, 0, 0, 0,
                12, 0, 0, 0, 1, 0, 0, 0,
                20, 0, 0, 0, 1, 0, 0, 0,
                21, 0, 0, 0, 1, 0, 0, 0,
                22, 0, 0, 0, 1, 0, 0, 0,
                30, 0, 0, 0, 1, 0, 0, 0,
                31, 0, 0, 0, 1, 0, 0, 0,
                32, 0, 0, 0, 1, 0, 0, 0,
                100, 0, 0, 0, 1, 0, 0, 0,
                101, 0, 0, 0, 1, 0, 0, 0,
                102, 0, 0, 0, 1, 0, 0, 0,
                110, 0, 0, 0, 1, 0, 0, 0,
                111, 0, 0, 0, 1, 0, 0, 0,
                112, 0, 0, 0, 1, 0, 0, 0,
                120, 0, 0, 0, 1, 0, 0, 0,
                121, 0, 0, 0, 1, 0, 0, 0,
                122, 0, 0, 0, 1, 0, 0, 0,
                130, 0, 0, 0, 1, 0, 0, 0,
                131, 0, 0, 0, 1, 0, 0, 0,
                132, 0, 0, 0, 1, 0, 0, 0,
                200, 0, 0, 0, 1, 0, 0, 0,
                201, 0, 0, 0, 1, 0, 0, 0,
                202, 0, 0, 0, 1, 0, 0, 0,
                210, 0, 0, 0, 1, 0, 0, 0,
                211, 0, 0, 0, 1, 0, 0, 0,
                212, 0, 0, 0, 1, 0, 0, 0,
                220, 0, 0, 0, 1, 0, 0, 0,
                221, 0, 0, 0, 1, 0, 0, 0,
                222, 0, 0, 0, 1, 0, 0, 0,
                230, 0, 0, 0, 1, 0, 0, 0,
                231, 0, 0, 0, 1, 0, 0, 0,
                232, 0, 0, 0, 1, 0, 0, 0
            }));

        }

        [Fact]
        public void ImageArray3DSingle()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            const Single SINGLE_VALUE = 2.3456E20F;

            Single[,,] imageArray = new Single[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = SINGLE_VALUE;
                    }
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Single);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[144]
            {
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97,
                200, 114, 75, 97
            }));
        }

        [Fact]
        public void ImageArray3DDouble()
        {
            const int IMAGE_WIDTH = 3;
            const int IMAGE_HEIGHT = 4;

            const Double DOUBLE_VALUE = 2.3456E100;

            Double[,,] imageArray = new Double[IMAGE_WIDTH, IMAGE_HEIGHT, 3];
            for (int i = 0; i < IMAGE_WIDTH; i++)
            {
                for (int j = 0; j < IMAGE_HEIGHT; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        imageArray[i, j, k] = DOUBLE_VALUE;
                    }
                }
            }

            byte[] bytes = imageArray.ToByteArray(1, 0, 0, AlpacaErrors.AlpacaNoError, "");
            ArrayMetadataV1 metadata = bytes.GetMetadataV1();
            Assert.True(metadata.TransmissionElementType == ImageArrayElementTypes.Double);

            for (int i = 0; i < bytes.Length; i++)
            {
                // output.WriteLine($"Byte[{i}]: {bytes[i]}");
            }

            Assert.True(CompareBytes(bytes.GetImageData(), new byte[288]
            {
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84,
                104, 158, 1, 135, 171, 114, 197, 84
            }));
        }

        #endregion

        #region Metadata Tests

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

        #endregion

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

        private bool CompareArrays(Array sourceArray, Array responseArray)
        {
            return CompareArrays(sourceArray, responseArray, true);
        }

        private bool CompareArrays(Array sourceArray, Array responseArray, bool includeElementTypeTest)
        {
            if (sourceArray is null)
            {
                output.WriteLine($"Source Array is NULL!");
                return false;
            }
            if (responseArray is null)
            {
                output.WriteLine($"Response Array is NULL!");
                return false;
            }
            if (sourceArray.Rank != responseArray.Rank)
            {
                output.WriteLine($"Array ranksare not equal. Source: {sourceArray.Rank}, Response: {responseArray.Rank}");
                return false;
            }

            if (sourceArray.GetLength(0) != responseArray.GetLength(0))
            {
                output.WriteLine($"Dimension 1 lengths are not equal. Source: {sourceArray.GetLength(0)}, Response: {responseArray.GetLength(0)}");
                return false;
            }
            if (sourceArray.GetLength(1) != responseArray.GetLength(1))
            {
                output.WriteLine($"Dimension 2 lengths are not equal. Source: {sourceArray.GetLength(1)}, Response: {responseArray.GetLength(1)}");
                return false;
            }
            if (sourceArray.Rank == 3)
            {
                if (sourceArray.GetLength(2) != responseArray.GetLength(2))
                {
                    output.WriteLine($"Dimension 3 lengths are not equal. Source: {sourceArray.GetLength(2)}, Response: {responseArray.GetLength(2)}");
                    return false;
                }
            }
            if (includeElementTypeTest)
            {
                if (sourceArray.GetType().GetElementType() != responseArray.GetType().GetElementType())
                {
                    output.WriteLine($"Element types are not the same. Source: {sourceArray.GetType().GetElementType()}, Response: {responseArray.GetType().GetElementType()}");
                    return false;
                }
            }
            try
            {
                switch (sourceArray.Rank)
                {
                    case 2:
                        output.WriteLine($"Array element types. Source: {sourceArray.GetType().GetElementType()}, Response: {responseArray.GetType().GetElementType()}");
                        output.WriteLine($"Array element values at index [0,0]. Source: {sourceArray.GetValue(0, 0)}, Response: {responseArray.GetValue(0, 0)}");

                        for (int i = 0; i < sourceArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < sourceArray.GetLength(1); j++)
                            {
                                if (Convert.ToDouble(sourceArray.GetValue(i, j)) != Convert.ToDouble(responseArray.GetValue(i, j)))
                                {
                                    output.WriteLine($"Array element values at index [{i},{j}] are not equal. Source: {sourceArray.GetValue(i, j)}, Response: {responseArray.GetValue(i, j)}");
                                    return false;
                                }
                            }
                        }
                        break;

                    case 3:
                        output.WriteLine($"Array element types. Source: {sourceArray.GetType().GetElementType()}, Response: {responseArray.GetType().GetElementType()}");
                        output.WriteLine($"Array element values at index [0,0,0]. Source: {sourceArray.GetValue(0, 0, 0)}, Response: {responseArray.GetValue(0, 0, 0)}");
                        for (int i = 0; i < sourceArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < sourceArray.GetLength(1); j++)
                            {
                                for (int k = 0; k < sourceArray.GetLength(2); k++)
                                {
                                    if (Convert.ToDouble(sourceArray.GetValue(i, j, k)) != Convert.ToDouble(responseArray.GetValue(i, j, k)))
                                    {
                                        output.WriteLine($"Array element values at index [{i},{j},{k}] are not equal. Source: {sourceArray.GetValue(i, j, k)}, Response: {responseArray.GetValue(i, j, k)}");
                                        return false;
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        output.WriteLine($"Unsupported rank:{sourceArray.Rank}");
                        return false;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return true;
        }
    }
}
