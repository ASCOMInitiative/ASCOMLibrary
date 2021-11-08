using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.Common.Alpaca
{
    public static class AlpacaTools
    {
        #region Constants

        // ImageArrayBytes constants
        public const string BYTE_ARRAY_ENPOINT_NAME = "imagearraybytes";
        public const int ARRAY_METADATAV1_LENGTH = 36; // Length of the array metadata version 1 structure

        // GetBase64Image constants
        public const string BASE64RESPONSE_COMMAND_NAME = "GetBase64Image";
        public const int BASE64RESPONSE_VERSION_NUMBER = 1;
        public const int BASE64RESPONSE_VERSION_POSITION = 0;
        public const int BASE64RESPONSE_OUTPUTTYPE_POSITION = 4;
        public const int BASE64RESPONSE_TRANSMISSIONTYPE_POSITION = 8;
        public const int BASE64RESPONSE_RANK_POSITION = 12;
        public const int BASE64RESPONSE_DIMENSION0_POSITION = 16;
        public const int BASE64RESPONSE_DIMENSION1_POSITION = 20;
        public const int BASE64RESPONSE_DIMENSION2_POSITION = 24;
        public const int ARRAY_METADATA_VERSION = 1;


        #endregion

        #region Public Functions

        public static byte[] ErrorMessageToByteArray(int metadataVersion, AlpacaErrors alpacaErrorNumber, string errorMessage)
        {
            // Validate supplied parameters
            if (metadataVersion != 1) throw new InvalidValueException($"ErrorMessageToByteArray - Unsupported metadata version: {metadataVersion}.");

            if (alpacaErrorNumber == AlpacaErrors.AlpacaNoError) throw new InvalidValueException($"ErrorMessageToByteArray - Supplied error number is {alpacaErrorNumber}, this indicates 'Success' rather than an 'Error'.");

            if ((alpacaErrorNumber < AlpacaErrors.AlpacaNoError) | (alpacaErrorNumber > AlpacaErrors.DriverMax)) throw new InvalidValueException($"ErrorMessageToByteArray - Invalid Alpaca error number: {alpacaErrorNumber}.");

            if (string.IsNullOrEmpty(errorMessage)) throw new InvalidValueException($"ErrorMessageToByteArray - Error message is either null or an empty string.");

            switch (metadataVersion)
            {
                case 1:
                    // Create a metadata structure containing the supplied error number
                    ArrayMetadataV1 arrayMetadataV1 = new ArrayMetadataV1(ImageArrayElementTypes.Unknown, ImageArrayElementTypes.Unknown, 0, 0, 0, 0, alpacaErrorNumber);

                    // Create a byte array from the metadata structure
                    byte[] arrayMetadataV1Bytes = arrayMetadataV1.ToByteArray<ArrayMetadataV1>();

                    // Create a byte array containing the UTF8 encoded string
                    byte[] errorMessagebytes = Encoding.UTF8.GetBytes(errorMessage);

                    // Create a return byte array that is large enough to contain both the metadata bytes and the UTF8 encoded message bytes
                    byte[] returnByteArray = new byte[ARRAY_METADATAV1_LENGTH + errorMessagebytes.Length];

                    // Copy the metadata bytes to the start of the return array
                    Array.Copy(arrayMetadataV1Bytes, returnByteArray, arrayMetadataV1Bytes.Length);

                    // Copy the error message bytes after the metadata bytes
                    Array.Copy(errorMessagebytes, 0, returnByteArray, ARRAY_METADATAV1_LENGTH, errorMessagebytes.Length);

                    // Return the composite byte array
                    return returnByteArray;

                default:
                    throw new InvalidValueException($"ErrorMessageToByteArray - Unsupported metadata version: {metadataVersion}");
            }
        }

        #endregion

        #region Public Extensions

        public static byte[] ToByteArray(this Array imageArray, int metadataVersion, AlpacaErrors errorNumber, string errorMessage)
        {
            int transmissionElementSize; // Managed size of transmitted elements

            // Handle error conditions
            if ((errorNumber != 0) | (!string.IsNullOrWhiteSpace(errorMessage)))
            {
                // Validate error parameters
                if ((errorNumber == 0) & (!string.IsNullOrWhiteSpace(errorMessage))) throw new InvalidValueException($"ToByteArray - Error number is {errorNumber} but an error message has been supplied: '{errorMessage}'");
                if ((errorNumber != 0) & (string.IsNullOrWhiteSpace(errorMessage))) throw new InvalidValueException($"ToByteArray - Error number is {errorNumber} but no error message has been supplied: '{errorMessage}'");

                // Handle metadata versions
                switch (metadataVersion)
                {
                    case 1:
                        byte[] errorMessageBytes = Encoding.UTF8.GetBytes(errorMessage);
                        ArrayMetadataV1 metadataVersion1 = new ArrayMetadataV1();
                        metadataVersion1.ErrorNumber = errorNumber;

                        byte[] metadataBytes = metadataVersion1.ToByteArray();
                        byte[] returnBytes = new byte[metadataBytes.Length + errorMessageBytes.Length];
                        Array.Copy(metadataBytes, returnBytes, metadataBytes.Length);
                        Array.Copy(errorMessageBytes, 0, returnBytes, metadataBytes.Length, errorMessageBytes.Length);
                        return returnBytes;

                    default:
                        throw new InvalidValueException($"Unsupported metadata version: {metadataVersion}");
                }
            }

            // At this point we have a successful transaction so validate the incoming array
            if (imageArray is null) throw new InvalidValueException("ToByteArray - Supplied array is null.");
            if ((imageArray.Rank < 2) | (imageArray.Rank > 3)) throw new InvalidValueException($"IToByteArray - Only arrays of rank 2 and 3 are supported. The supplied array has a rank of {imageArray.Rank}.");

            // We can't handle object arrays so test for this
            string arrayTypeName = imageArray.GetType().Name;
            if (arrayTypeName.ToLowerInvariant().Contains("object")) throw new InvalidValueException($"ToByteArray - Object arrays of type {arrayTypeName} are not supported.");

            // Set the array type
            ImageArrayElementTypes intendedElementType = ImageArrayElementTypes.Unknown;
            ImageArrayElementTypes transmissionElementType = ImageArrayElementTypes.Unknown;

            // Get the type code of the array elements
            TypeCode arrayElementTypeCode = Type.GetTypeCode(imageArray.GetType().GetElementType());

            // Set the element type of the intended array and default the transmission element type to be the same as the intended type
            switch (arrayElementTypeCode)
            {
                case TypeCode.Byte:
                    intendedElementType = ImageArrayElementTypes.Byte;
                    transmissionElementType = ImageArrayElementTypes.Byte;
                    transmissionElementSize = 1;
                    break;

                case TypeCode.Int16:
                    intendedElementType = ImageArrayElementTypes.Int16;
                    transmissionElementType = ImageArrayElementTypes.Int16;
                    transmissionElementSize = 2;
                    break;

                case TypeCode.Int32:
                    intendedElementType = ImageArrayElementTypes.Int32;
                    transmissionElementType = ImageArrayElementTypes.Int32;
                    transmissionElementSize = 4;
                    break;

                case TypeCode.Int64:
                    intendedElementType = ImageArrayElementTypes.Int64;
                    transmissionElementType = ImageArrayElementTypes.Int64;
                    transmissionElementSize = 8;
                    break;

                case TypeCode.Single:
                    intendedElementType = ImageArrayElementTypes.Single;
                    transmissionElementType = ImageArrayElementTypes.Single;
                    transmissionElementSize = 4;
                    break;

                case TypeCode.Double:
                    intendedElementType = ImageArrayElementTypes.Double;
                    transmissionElementType = ImageArrayElementTypes.Double;
                    transmissionElementSize = 8;
                    break;

                case TypeCode.Decimal:
                    intendedElementType = ImageArrayElementTypes.Decimal;
                    transmissionElementType = ImageArrayElementTypes.Decimal;
                    transmissionElementSize = 16;
                    break;

                default:
                    throw new InvalidValueException($"ToByteArray - Received an unsupported return array type: {imageArray.GetType().Name}, with elements of type: {imageArray.GetType().GetElementType().Name}");
            }

            // Special handling for Int32 arrays - see if we can convert them to Int16 or UInt16 arrays
            if (arrayElementTypeCode == TypeCode.Int32)
            {
                // NOTE
                // NOTE - This algorithm uses a UInt16 array to transmit an array with Int16 values because we are only interested in the byte values for this purpose,
                // NOTE - not the arithmetic interpretation of those bytes.
                // NOTE

                bool arrayIsInt16 = true; // Flag indicating whether the supplied array conforms to the Int16 value range -32768 to +32767. Start by assuming success
                bool arrayIsUint16 = true; // Flag indicating whether the supplied array conforms to the UInt16 value range 0 to +65535. Start by assuming success

                // Handle 2D and 3D arrays
                switch (imageArray.Rank)
                {
                    case 2:
                        UInt16[,] uInt16Array = new UInt16[imageArray.GetLength(0), imageArray.GetLength(1)]; // Array to hold the 16bit transmission array (either Int16 or UInt16 values)

                        // Get the device's Int32 image array
                        int[,] int2dArray = (int[,])imageArray;

                        // Parellelise the array copy to improve performance
                        Parallel.For(0, imageArray.GetLength(0), (i) => // Iterate over the slowest changing dimension
                        {
                            Int32 int32ElementValue; // Local variable to hold the Int32 element being tested (saves calculating an array offset later in the process)
                            Int16 int16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)
                            UInt16 uInt16ElementValue; // Local variable to hold the Unt16 element value (saves calculating an array offset later in the process)
                            bool arrayIsInt16Internal = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsInt16 variable.
                            bool arrayIsUint16Internal = true; // Local variable to hold the UInt16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsInt16 variable.

                            // Iterate over the fastest changing dimension
                            for (int j = 0; j < imageArray.GetLength(1); j++)
                            {
                                // Get the current array element value
                                int32ElementValue = int2dArray[i, j];

                                // Truncate the supplied 4-byte Int32 value to create a 2-byte UInt16 value
                                uInt16ElementValue = (UInt16)int32ElementValue;

                                // Truncate the supplied 4-byte Int32 value to create a 2-byte Int16 value
                                int16ElementValue = (Int16)int32ElementValue;

                                // Store the UInt16 value in the array.
                                uInt16Array[i, j] = uInt16ElementValue;


                                // Compare the Int16 and Int32 values. 
                                if (int16ElementValue != int32ElementValue) arrayIsInt16Internal = false;

                                // Compare the UInt16 and Int32 values. 
                                if (uInt16ElementValue != int32ElementValue) arrayIsUint16Internal = false;
                            }

                            // Update the master arrayIsInt16 and arrayIsUint16 variables as the logical AND of the mater and update values.
                            arrayIsInt16 &= arrayIsInt16Internal;
                            arrayIsUint16 &= arrayIsUint16Internal;
                        });

                        // Return the appropriate array if either a UInt16 or Int16 array was provided by the device
                        if (arrayIsUint16) // Supplied array has UInt16 values so return the shorter array in place of the supplied Int32 array
                        {
                            imageArray = uInt16Array; // Assign the Int16 array to the imageArray variable in place of the Int32 array
                            transmissionElementType = ImageArrayElementTypes.UInt16; // Flag that the array elements are UInt16
                            transmissionElementSize = sizeof(UInt16); // Indicate that the transmitted elements are of UInt16 size rather than Int32 size
                        }
                        else if (arrayIsInt16) // Supplied array has Int16 values so return the shorter array in place of the supplied Int32 array
                        {
                            imageArray = uInt16Array; // Assign the UInt16 array to the imageArray variable in place of the Int32 array (at the byte level Int32 and UInt32 are equivalent - both consist of two bytes)
                            transmissionElementType = ImageArrayElementTypes.Int16; // Flag that the array elements are Int16
                            transmissionElementSize = sizeof(Int16); // Indicate that the transmitted elements are of UInt16 size rather than Int32 size
                        }
                        else
                        {
                            // No action, continue to use the supplied Int32 array because its values fall outside the Uint16 and Int16 number ranges
                        }
                        break;

                    case 3:
                        UInt16[,,] uInt163dArray = new UInt16[imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2)];  // Array to hold the 16bit transmission array (either Int16 or UInt16 values)

                        // Get the device's Int32 image array
                        Int32[,,] int3dArray = (Int32[,,])imageArray;

                        // Parellelise the array copy to improve performance
                        Parallel.For(0, imageArray.GetLength(0), (i) => // Iterate over the slowest changing dimension
                        {
                            bool arrayIsInt16Internal1 = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayisInt16 variable.
                            bool arrayIsUint16Internal1 = true; // Local variable to hold the UInt16 status within this particular thread. Used to reduce thread conflict when updating the arrayisInt16 variable.

                            // Iterate over the mid changing dimension
                            for (int j = 0; j < imageArray.GetLength(1); j++)
                            {
                                Int32 int32ElementValue; // Local variable to hold the Int32 element being tested (saves calculating an array offset later in the process)
                                Int16 int16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)
                                UInt16 uInt16ElementValue; // Local variable to hold the UInt16 element value (saves calculating an array offset later in the process)
                                bool arrayIsInt16Internal2 = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsInt16 variable.
                                bool arrayIsUInt16Internal2 = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsUInt16 variable.

                                // Iterate over the fastest changing dimension
                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                {
                                    // Get the current array element value
                                    int32ElementValue = int3dArray[i, j, k];

                                    // Truncate the supplied 4-byte Int32 value to create a 2-byte UInt16 value
                                    uInt16ElementValue = (UInt16)int32ElementValue;

                                    // Truncate the supplied 4-byte Int32 value to create a 2-byte Int16 value
                                    int16ElementValue = (Int16)int32ElementValue;

                                    // Copy the Int32 value to the corresponding Int16 array element. 
                                    uInt163dArray[i, j, k] = uInt16ElementValue;

                                    // Compare the Int16 and Int32 values. 
                                    if (int16ElementValue != int32ElementValue) arrayIsInt16Internal2 = false; // If they are not the same the Int32 value was outside the range of Int16 and arrayIsInt16Internal will be set false

                                    // Compare the UInt16 and Int32 values. 
                                    if (uInt16ElementValue != int32ElementValue) arrayIsUInt16Internal2 = false;
                                }

                                // Update the arrayIsInt16Internal1 and arrayIsUint16Internal1 variables as the logical AND of the mater and update values.
                                arrayIsInt16Internal1 &= arrayIsInt16Internal2;
                                arrayIsUint16Internal1 &= arrayIsUInt16Internal2;

                            }

                            // Update the master arrayIsInt16 and arrayIsUint16 variables as the logical AND of the mater and update values.
                            arrayIsInt16 &= arrayIsInt16Internal1;
                            arrayIsUint16 &= arrayIsUint16Internal1;

                        });

                        // Return the appropriate array if either a UInt16 or Int16 array was provided by the device
                        if (arrayIsUint16) // Supplied array has UInt16 values so return the shorter array in place of the supplied Int32 array
                        {
                            imageArray = uInt163dArray; // Assign the Int16 array to the imageArray variable in place of the Int32 array
                            transmissionElementType = ImageArrayElementTypes.UInt16; // Flag that the array elements are UInt16
                            transmissionElementSize = sizeof(UInt16); // Indicate that the transmitted elements are of UInt16 size rather than Int32 size
                        }
                        else if (arrayIsInt16) // Supplied array has Int16 values so return the shorter array in place of the supplied Int32 array
                        {
                            imageArray = uInt163dArray; // Assign the UInt16 array to the imageArray variable in place of the Int32 array (at the byte level Int32 and UInt32 are equivalent - both consist of two bytes)
                            transmissionElementType = ImageArrayElementTypes.Int16; // Flag that the array elements are Int16
                            transmissionElementSize = sizeof(Int16); // Indicate that the transmitted elements are of UInt16 size rather than Int32 size
                        }
                        else
                        {
                            // No action, continue to use the supplied Int32 array because its values fall outside the Uint16 and Int16 number ranges
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"ToByteArray - The camera returned an array of rank: {imageArray.Rank}, which is not supported.");
                }
            }

            switch (metadataVersion)
            {
                case 1:
                    // Create a version 2 metadata structure
                    ArrayMetadataV1 metadataVersion1;
                    if (imageArray.Rank == 2) metadataVersion1 = new ArrayMetadataV1(intendedElementType, transmissionElementType, 2, imageArray.GetLength(0), imageArray.GetLength(1), 0, AlpacaErrors.AlpacaNoError);
                    else metadataVersion1 = new ArrayMetadataV1(intendedElementType, transmissionElementType, 3, imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2), AlpacaErrors.AlpacaNoError);

                    // Turn the metadata structure into a byte array
                    byte[] metadataVersion2Bytes = metadataVersion1.ToByteArray<ArrayMetadataV1>();

                    // Create a return array of size equal to the sum of the metadata and image array lengths
                    byte[] imageArrayBytesV2 = new byte[imageArray.Length * transmissionElementSize + metadataVersion2Bytes.Length]; // Size the image array bytes as the product of the transmission element size and the number of elements

                    // Copy the metadata bytes to the start of the return byte array
                    Array.Copy(metadataVersion2Bytes, imageArrayBytesV2, metadataVersion2Bytes.Length);

                    // Copy the image array bytes after the metadata
                    Buffer.BlockCopy(imageArray, 0, imageArrayBytesV2, metadataVersion2Bytes.Length, imageArray.Length * transmissionElementSize);

                    // Return the byte array
                    return imageArrayBytesV2;

                default:
                    throw new InvalidValueException($"ToByteArray - Unsupported metadata version: {metadataVersion}");
            }

        }

        public static Array ToImageArray(this byte[] imageBytes)
        {
            ImageArrayElementTypes imageElementType;
            ImageArrayElementTypes transmissionElementType;
            int rank;
            int dimension0;
            int dimension1;
            int dimension2;
            int dataStart;

            // Validate the incoming array
            if (imageBytes is null) throw new InvalidValueException("ToImageArray - Supplied array is null.");
            if (imageBytes.Length <= ARRAY_METADATAV1_LENGTH) throw new InvalidValueException($"ToImageArray - Supplied array does not exceed the size of the mandatory metadata. Arrays must contain at least {ARRAY_METADATAV1_LENGTH} bytes. The supplied array has a length of {imageBytes.Length}.");

            int metadataVersion = imageBytes.GetMetadataVersion();

            // Get the metadata version and extract the supplied values
            switch (metadataVersion)
            {
                case 1:
                    ArrayMetadataV1 metadataV1 = imageBytes.GetMetadataV1();
                    // Set the array type, rank and dimensions
                    imageElementType = metadataV1.ImageElementType;
                    transmissionElementType = metadataV1.TransmissionElementType;
                    rank = metadataV1.Rank;
                    dimension0 = metadataV1.Dimension0;
                    dimension1 = metadataV1.Dimension1;
                    dimension2 = metadataV1.Dimension2;
                    dataStart = metadataV1.DataStart;

                    Debug.WriteLine($"ToImageArray - Element type: {imageElementType} Transmission type: {transmissionElementType}");

                    break;

                default:
                    throw new InvalidValueException($"ToImageArray - The supplied array contains an unsupported metadata version number: {metadataVersion}. This component supports metadata version 1.");
            }

            // Validate the metadata
            if (imageElementType == ImageArrayElementTypes.Unknown)
                throw new InvalidValueException("ToImageArray - ImageArrayElementType is 0, meaning ImageArrayElementTypes.Unknown");
            if (imageElementType > Enum.GetValues(typeof(ImageArrayElementTypes)).Cast<ImageArrayElementTypes>().Max())
                throw new InvalidValueException($"ToImageArray - The ImageArrayElementType value {((int)imageElementType)} is outside the valid range 0 to {Enum.GetValues(typeof(ImageArrayElementTypes)).Cast<ImageArrayElementTypes>().Max()}");
            if (transmissionElementType == ImageArrayElementTypes.Unknown)
                throw new InvalidValueException("ToImageArray - The TransmissionElementType is 0, meaning ImageArrayElementTypes.Unknown");

            // Convert the returned byte[] into the form that the client is expecting
            if ((imageElementType == ImageArrayElementTypes.Int32) & (transmissionElementType == ImageArrayElementTypes.Int16)) // Handle the special case where Int32 has been converted to Int16 for transmission
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        Int16[,] short2dArray = new Int16[dimension0, dimension1];
                        Buffer.BlockCopy(imageBytes, dataStart, short2dArray, 0, imageBytes.Length - dataStart);

                        int[,] int2dArray = new int[dimension0, dimension1];
                        Parallel.For(0, short2dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < short2dArray.GetLength(1); j++)
                            {
                                int2dArray[i, j] = short2dArray[i, j];
                            }
                        });
                        return int2dArray;

                    case 3: // Rank 3
                        Int16[,,] short3dArray = new Int16[dimension0, dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, short3dArray, 0, imageBytes.Length - dataStart);

                        int[,,] int3dArray = new int[dimension0, dimension1, dimension2];
                        Parallel.For(0, short3dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < short3dArray.GetLength(1); j++)
                            {
                                for (int k = 0; k < short3dArray.GetLength(2); k++)
                                {
                                    int3dArray[i, j, k] = short3dArray[i, j, k];
                                }
                            }
                        });
                        return int3dArray;

                    default:
                        throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                }
            }
            else if ((imageElementType == ImageArrayElementTypes.Int32) & (transmissionElementType == ImageArrayElementTypes.UInt16)) // Handle the special case where Int32 values has been converted to UInt16 for transmission
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        UInt16[,] uInt16Array2D = new UInt16[dimension0, dimension1];
                        Buffer.BlockCopy(imageBytes, dataStart, uInt16Array2D, 0, imageBytes.Length - dataStart);

                        int[,] int2dArray = new int[dimension0, dimension1];
                        Parallel.For(0, uInt16Array2D.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < uInt16Array2D.GetLength(1); j++)
                            {
                                int2dArray[i, j] = uInt16Array2D[i, j];
                            }
                        });
                        return int2dArray;

                    case 3: // Rank 3
                        UInt16[,,] uInt16Array3D = new UInt16[dimension0, dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, uInt16Array3D, 0, imageBytes.Length - dataStart);

                        int[,,] int3dArray = new int[dimension0, dimension1, dimension2];
                        Parallel.For(0, uInt16Array3D.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < uInt16Array3D.GetLength(1); j++)
                            {
                                for (int k = 0; k < uInt16Array3D.GetLength(2); k++)
                                {
                                    int3dArray[i, j, k] = uInt16Array3D[i, j, k];
                                }
                            }
                        });
                        return int3dArray;

                    default:
                        throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                }
            }
            else // Handle all other cases where the expected array type and the transmitted array type are the same
            {
                if (imageElementType == transmissionElementType) // Required and transmitted array element types are the same
                {
                    switch (imageElementType)
                    {
                        case ImageArrayElementTypes.Byte:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    byte[,] byte2dArray = new byte[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, byte2dArray, 0, imageBytes.Length - dataStart);
                                    return byte2dArray;

                                case 3: // Rank 3
                                    byte[,,] byte3dArray = new byte[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, byte3dArray, 0, imageBytes.Length - dataStart);
                                    return byte3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned byte array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Int16:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    short[,] short2dArray = new short[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, short2dArray, 0, imageBytes.Length - dataStart);
                                    return short2dArray;

                                case 3: // Rank 3
                                    short[,,] short3dArray = new short[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, short3dArray, 0, imageBytes.Length - dataStart);
                                    return short3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int16 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.UInt16:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    UInt16[,] uInt16Array2D = new UInt16[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt16Array2D, 0, imageBytes.Length - dataStart);
                                    return uInt16Array2D;

                                case 3: // Rank 3
                                    UInt16[,,] uInt16Array3D = new UInt16[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt16Array3D, 0, imageBytes.Length - dataStart);
                                    return uInt16Array3D;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned UInt16 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Int32:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    int[,] int2dArray = new int[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, int2dArray, 0, imageBytes.Length - dataStart);
                                    return int2dArray;

                                case 3: // Rank 3
                                    int[,,] int3dArray = new int[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, int3dArray, 0, imageBytes.Length - dataStart);
                                    return int3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int32 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.UInt32:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    UInt32[,] uInt32Array2D = new UInt32[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt32Array2D, 0, imageBytes.Length - dataStart);
                                    return uInt32Array2D;

                                case 3: // Rank 3
                                    UInt32[,,] uInt32Array3D = new UInt32[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt32Array3D, 0, imageBytes.Length - dataStart);
                                    return uInt32Array3D;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned UInt32 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Int64:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Int64[,] int642dArray = new Int64[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, int642dArray, 0, imageBytes.Length - dataStart);
                                    return int642dArray;

                                case 3: // Rank 3
                                    Int64[,,] int643dArray = new Int64[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, int643dArray, 0, imageBytes.Length - dataStart);
                                    return int643dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Single:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Single[,] single2dArray = new Single[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, single2dArray, 0, imageBytes.Length - dataStart);
                                    return single2dArray;

                                case 3: // Rank 3
                                    Single[,,] single3dArray = new Single[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, single3dArray, 0, imageBytes.Length - dataStart);
                                    return single3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Double:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Double[,] double2dArray = new Double[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, double2dArray, 0, imageBytes.Length - dataStart);
                                    return double2dArray;

                                case 3: // Rank 3
                                    Double[,,] double3dArray = new Double[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, double3dArray, 0, imageBytes.Length - dataStart);
                                    return double3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Decimal:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Decimal[,] decimal2dArray = new Decimal[dimension0, dimension1];
                                    Buffer.BlockCopy(imageBytes, dataStart, decimal2dArray, 0, imageBytes.Length - dataStart);
                                    return decimal2dArray;

                                case 3: // Rank 3
                                    Decimal[,,] decimal3dArray = new Decimal[dimension0, dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, decimal3dArray, 0, imageBytes.Length - dataStart);
                                    return decimal3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        default:
                            throw new InvalidValueException($"ToImageArray - The device has returned an unsupported image array element type: {imageElementType}.");
                    }
                }
                else // An unsupported combination of array element types has been returned
                {
                    throw new InvalidValueException($"ToImageArray - The device has returned an unsupported combination of Output type: {imageElementType} and Transmission type: {transmissionElementType}.");
                }
            }
        }

        public static int GetMetadataVersion(this byte[] imageBytes)
        {
            if (imageBytes.Length < ARRAY_METADATAV1_LENGTH) throw new InvalidOperationException($"GetMetadataVersion - Supplied array size: {imageBytes.Length} is smaller than the minimum metadata size ({ARRAY_METADATAV1_LENGTH})");
            return BitConverter.ToInt32(imageBytes, 0);
        }

        public static string GetErrrorMessage(this byte[] errorMessageBytes)
        {
            // Validate error message array
            if (errorMessageBytes.Length < ARRAY_METADATAV1_LENGTH) throw new InvalidOperationException($"GetErrrorMessage - Supplied array size: {errorMessageBytes.Length} is smaller than the minimum metadata size ({ARRAY_METADATAV1_LENGTH})");
            if (errorMessageBytes.Length == ARRAY_METADATAV1_LENGTH) throw new InvalidValueException($"GetErrrorMessage - The byte array length equals the metadata length, the supplied array does not contain any message bytes.");

            // Get the metadata version
            int metadataVersion = errorMessageBytes.GetMetadataVersion();

            // Process according to metadata version
            switch (metadataVersion)
            {
                case 1:
                    ArrayMetadataV1 arrayMetadataV1 = errorMessageBytes.GetMetadataV1();
                    return Encoding.UTF8.GetString(errorMessageBytes, arrayMetadataV1.DataStart, errorMessageBytes.Length - arrayMetadataV1.DataStart);

                default:
                    throw new InvalidValueException($"GetErrrorMessage - The supplied array contains an unsupported metadata version number: {metadataVersion}. This component supports metadata version 1.");
            }
        }

        public static ArrayMetadataV1 GetMetadataV1(this byte[] imageBytes)
        {
            if (imageBytes.Length < ARRAY_METADATAV1_LENGTH) throw new InvalidOperationException($"GetMetadataV1 - Supplied array size: {imageBytes.Length} is smaller than the minimum metadata size ({ARRAY_METADATAV1_LENGTH}");

            // Initialise array to hold the metadata bytes
            byte[] metadataV1Bytes = new byte[ARRAY_METADATAV1_LENGTH];

            // Copy the metadata bytes from the image array to the metadata bytes array
            Array.Copy(imageBytes, 0, metadataV1Bytes, 0, ARRAY_METADATAV1_LENGTH);

            // Create the metadata structure from the metadata bytes and return it to the caller
            ArrayMetadataV1 metadataV1 = metadataV1Bytes.ToStructure<ArrayMetadataV1>();
            return metadataV1;
        }

        #endregion

        #region Private Extensions

        /// <summary>
        /// Convert a structure to a byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structure"></param>
        /// <returns></returns>
        private static byte[] ToByteArray<T>(this T structure) where T : struct
        {
            var bufferSize = Marshal.SizeOf(structure);
            var byteArray = new byte[bufferSize];

            IntPtr handle = Marshal.AllocHGlobal(bufferSize);
            try
            {
                Marshal.StructureToPtr(structure, handle, true);
                Marshal.Copy(handle, byteArray, 0, bufferSize);
            }
            finally
            {
                Marshal.FreeHGlobal(handle);
            }
            return byteArray;
        }

        /// <summary>
        /// Convert a byte array to a structure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private static T ToStructure<T>(this byte[] byteArray) where T : struct
        {
            var structure = new T();
            var bufferSize = Marshal.SizeOf(structure);
            IntPtr handle = Marshal.AllocHGlobal(bufferSize);
            try
            {
                Marshal.Copy(byteArray, 0, handle, bufferSize);
                structure = Marshal.PtrToStructure<T>(handle);

            }
            finally
            {
                Marshal.FreeHGlobal(handle);
            }

            return structure;
        }

        #endregion

        #region Unused code

        // As used by initial alpha versions
        [StructLayout(LayoutKind.Explicit, Size = 28)]
        public struct ArrayMetadataVersion0
        {
            public ArrayMetadataVersion0(ImageArrayElementTypes imageElementType, ImageArrayElementTypes transmissionElementType, int arrayRank, int arrayDimension0, int arrayDimension1, int arrayDimension2)
            {
                MetadataVersion = 0;
                this.ImageElementType = imageElementType;
                this.TransmissionElementType = transmissionElementType;
                this.ArrayRank = arrayRank;
                this.ArrayDimension0 = arrayDimension0;
                this.ArrayDimension1 = arrayDimension1;
                this.ArrayDimension2 = arrayDimension2;
            }

            [FieldOffset(0)] public int MetadataVersion; // Bytes 0..3 - Must always be the first field!
            [FieldOffset(4)] public ImageArrayElementTypes ImageElementType; // Bytes 4..7
            [FieldOffset(8)] public ImageArrayElementTypes TransmissionElementType; // Bytes 8..11
            [FieldOffset(12)] public int ArrayRank; // Bytes 12..15
            [FieldOffset(16)] public int ArrayDimension0; // Bytes 16..19
            [FieldOffset(20)] public int ArrayDimension1; // Bytes 20..23
            [FieldOffset(24)] public int ArrayDimension2; // Bytes 24..27
        }

        #endregion

    }
}
