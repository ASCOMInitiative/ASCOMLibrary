using ASCOM.Common.Alpaca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ASCOM.Alpaca.Tests.ImageArray
{
    public class ImageArrayClientTypeTests
    {
        private readonly ITestOutputHelper output;

        public ImageArrayClientTypeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        const int IMAGE_WIDTH = 4;
        const int IMAGE_HEIGHT = 3;

        Byte[,] byteImageArray = new Byte[IMAGE_WIDTH, IMAGE_HEIGHT];
        Int16[,] int16ImageArray = new Int16[IMAGE_WIDTH, IMAGE_HEIGHT];
        UInt16[,] uint16ImageArray = new UInt16[IMAGE_WIDTH, IMAGE_HEIGHT];
        Int32[,] int32ImageArray = new Int32[IMAGE_WIDTH, IMAGE_HEIGHT];
        UInt32[,] uint32ImageArray = new UInt32[IMAGE_WIDTH, IMAGE_HEIGHT];
        Int64[,] int64ImageArray = new Int64[IMAGE_WIDTH, IMAGE_HEIGHT];
        UInt64[,] uint64ImageArray = new UInt64[IMAGE_WIDTH, IMAGE_HEIGHT];
        Single[,] singleImageArray = new float[IMAGE_WIDTH, IMAGE_HEIGHT];
        Double[,] doubleImageArray = new double[IMAGE_WIDTH, IMAGE_HEIGHT];
        Object[,] objectImageArray = new object[IMAGE_WIDTH, IMAGE_HEIGHT] { { 0, 0, 0 }, { 1, 1, 1 }, { 2, 2, 2 }, { 3, 3, 3 } };

        [Fact]
        public void ReturnByteArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.Byte;

            // Confirm that these calls work
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnInt16ArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.Int16;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Byte);
            TestArrayElementType(ImageArrayElementTypes.Int16);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnUInt16ArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.UInt16;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Byte);
            TestArrayElementType(ImageArrayElementTypes.UInt16);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnInt32ArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.Int32;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Byte);
            TestArrayElementType(ImageArrayElementTypes.Int16);
            TestArrayElementType(ImageArrayElementTypes.UInt16);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnUInt32ArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.UInt32;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Byte);
            TestArrayElementType(ImageArrayElementTypes.UInt16);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnInt64ArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.Int64;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Byte);
            TestArrayElementType(ImageArrayElementTypes.Int16);
            TestArrayElementType(ImageArrayElementTypes.UInt16);
            TestArrayElementType(ImageArrayElementTypes.Int32);
            TestArrayElementType(ImageArrayElementTypes.UInt32);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnUInt64ArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.UInt64;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Byte);
            TestArrayElementType(ImageArrayElementTypes.UInt16);
            TestArrayElementType(ImageArrayElementTypes.UInt32);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnSingleArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.Single;

            // Confirm that these calls work
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(byteImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void ReturnDoubleArrayToClient()
        {
            ImageArrayElementTypes testElementType = ImageArrayElementTypes.Double;

            // Confirm that these calls work
            TestArrayElementType(ImageArrayElementTypes.Single);
            TestArrayElementType(testElementType);

            // Confirm that these calls fail
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(byteImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint16ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, testElementType, AlpacaErrors.AlpacaNoError, ""));
        }

        [Fact]
        public void CanAlwaysConvertToObject()
        {
            // Confirm that presentation as object always works
            Array ojjectArray = AlpacaTools.ToByteArray(byteImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(uint16ImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
            ojjectArray = AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, ImageArrayElementTypes.Object, AlpacaErrors.AlpacaNoError, "");
        }

        [Fact]
        public void CanNeverConvertElementTypeUnknown()
        {
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(byteImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int16ImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint16ImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int32ImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint32ImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(int64ImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(uint64ImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(singleImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(doubleImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
            Assert.Throws<InvalidValueException>(() => AlpacaTools.ToByteArray(objectImageArray, 1, 0, 0, ImageArrayElementTypes.Unknown, AlpacaErrors.AlpacaNoError, ""));
        }

        private void TestArrayElementType(ImageArrayElementTypes elementType)
        {
            TypeCode requiredArrayElementTypeCode;
            Array sourceArray;

            // Set up the required type and source array based on the element type to be returned to the client
            switch (elementType)
            {
                case ImageArrayElementTypes.Byte:
                    requiredArrayElementTypeCode = TypeCode.Byte;
                    sourceArray = byteImageArray;
                    break;

                case ImageArrayElementTypes.Int16:
                    requiredArrayElementTypeCode = TypeCode.Int16;
                    sourceArray = int16ImageArray;
                    break;

                case ImageArrayElementTypes.UInt16:
                    requiredArrayElementTypeCode = TypeCode.UInt16;
                    sourceArray = uint16ImageArray;
                    break;

                case ImageArrayElementTypes.Int32:
                    requiredArrayElementTypeCode = TypeCode.Int32;
                    sourceArray = int32ImageArray;
                    break;

                case ImageArrayElementTypes.UInt32:
                    requiredArrayElementTypeCode = TypeCode.UInt32;
                    sourceArray = uint32ImageArray;
                    break;

                case ImageArrayElementTypes.Int64:
                    requiredArrayElementTypeCode = TypeCode.Int64;
                    sourceArray = int64ImageArray;
                    break;

                case ImageArrayElementTypes.UInt64:
                    requiredArrayElementTypeCode = TypeCode.UInt64;
                    sourceArray = uint64ImageArray;
                    break;

                case ImageArrayElementTypes.Single:
                    requiredArrayElementTypeCode = TypeCode.Single;
                    sourceArray = singleImageArray;
                    break;

                case ImageArrayElementTypes.Double:
                    requiredArrayElementTypeCode = TypeCode.Double;
                    sourceArray = doubleImageArray;
                    break;

                // Add cases for other element types as needed
                default:
                    throw new InvalidValueException($"TestArrayElementType - {elementType} is invalid or not recognised.");
            }

            // Create a byte array based on the source array and convert it back to an image array for the client
            Byte[] byteArray = AlpacaTools.ToByteArray(sourceArray, 1, 0, 0, elementType, AlpacaErrors.AlpacaNoError, "");
            Array clientArray = byteArray.ToImageArray();

            // Get the type code of the returned array element type and confirm that it matches the expected type code
            TypeCode returnedArrayElementTypeCode = Type.GetTypeCode(clientArray.GetType().GetElementType());
            Assert.Equal(requiredArrayElementTypeCode, returnedArrayElementTypeCode);
        }
    }
}
