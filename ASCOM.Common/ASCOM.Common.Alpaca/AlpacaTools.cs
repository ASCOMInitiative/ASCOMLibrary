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
    /// <summary>
    /// Repository of tools and constants specific to Alpaca implementation
    /// </summary>
    public static class AlpacaTools
    {
        #region Constants

        // ImageBytes constants

        /// <summary>
        /// Length of array metadata version 1
        /// </summary>
        public const int ARRAY_METADATAV1_LENGTH = 44; // Length of the array metadata version 1 structure

        /// <summary>
        /// Latest verison number of the ImageBytes metadata array
        /// </summary>
        public const int ARRAY_METADATA_VERSION = 1;

        #endregion

        #region Public Functions

        /// <summary>
        /// Convert an Alpaca error number and message to a byte array for transfer to a client.
        /// </summary>
        /// <param name="metadataVersion">Required metadata version - Currently 1</param>
        /// <param name="alpacaErrorNumber">Alpaca error number</param>
        /// <param name="errorMessage">Error message to encode.</param>
        /// <returns></returns>
        /// <exception cref="InvalidValueException"></exception>
        public static byte[] ErrorMessageToByteArray(int metadataVersion, uint clientTransactionID, uint serverTransactionID, AlpacaErrors alpacaErrorNumber, string errorMessage)
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
                    ArrayMetadataV1 arrayMetadataV1 = new ArrayMetadataV1(alpacaErrorNumber, clientTransactionID, serverTransactionID, ImageArrayElementTypes.Unknown, ImageArrayElementTypes.Unknown, 0, 0, 0, 0);

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

        /// <summary>
        /// Alpaca Extension - Convert the array or error message to a byte array for transmission to a client
        /// </summary>
        /// <param name="imageArray">The 2D or 3D source image array. (Ignored when returning an error.)</param>
        /// <param name="metadataVersion">Metadata version to use (Currently 1).</param>
        /// <param name="clientTransactionID">Client's transaction ID.</param>
        /// <param name="serverTransactionID">Device's transaction ID.</param>
        /// <param name="errorNumber">Error number. 0 for success, non-zero for an error.</param>
        /// <param name="errorMessage">Error message. Empty string for success, error message for an error.</param>
        /// <returns>Byte array prefixed with array metadata.</returns>
        /// <exception cref="InvalidValueException">If only one of the error number and error message indicates an error.</exception>
        /// <exception cref="InvalidValueException">Image array is null for a successful transaction or the array rank is <2 or >3 or the array is of type object</exception>
        /// <exception cref="InvalidValueException">The array element type is not supported.</exception>
        /// <remarks>
        /// Int32 source arrays where all elements have Int16, UInt16 or Byte values will automatically be converted to the smaller 
        /// data size for transmission in order to improve performance and reduce network trafic. 
        /// Int16 and UInt16 source arrays that only have Byte values will similarly be converted to the smaller 
        /// data size for transmission. 
        /// All other element types are transmited as supplied.
        /// </remarks>
        public static byte[] ToByteArray(this Array imageArray, int metadataVersion, uint clientTransactionID, uint serverTransactionID, AlpacaErrors errorNumber, string errorMessage)
        {
            int transmissionElementSize; // Managed size of transmitted elements
            bool arrayIsByte; // Flag indicating whether the supplied array conforms to the Byte value range 0 to +255.

            // Handle error conditions
            if ((errorNumber != 0) | (!string.IsNullOrWhiteSpace(errorMessage)))
            {
                // Validate error parameters
                if ((errorNumber == 0) & (!string.IsNullOrWhiteSpace(errorMessage))) throw new InvalidValueException($"ToByteArray - Error number is {errorNumber} but an error message has been supplied: '{errorMessage}'");
                if ((errorNumber != 0) & (string.IsNullOrWhiteSpace(errorMessage))) throw new InvalidValueException($"ToByteArray - Error number is {errorNumber} but no error message has been supplied: '{errorMessage}'");

                return ErrorMessageToByteArray(metadataVersion, clientTransactionID, serverTransactionID, errorNumber, errorMessage);
            }

            // At this point we have a successful transaction so validate the incoming array
            if (imageArray is null) throw new InvalidValueException("ToByteArray - Supplied array is null.");
            if ((imageArray.Rank < 2) | (imageArray.Rank > 3)) throw new InvalidValueException($"ToByteArray - Only arrays of rank 2 and 3 are supported. The supplied array has a rank of {imageArray.Rank}.");

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

                    // Special handling for Int16 arrays - see if we can convert them to a Byte array

                    arrayIsByte = true; // Flag indicating whether the supplied array conforms to the Byte value range 0 to +255. Start by assuming success

                    // Handle 2D and 3D arrays
                    switch (imageArray.Rank)
                    {
                        case 2:
                            byte[,] byteArray = new byte[imageArray.GetLength(0), imageArray.GetLength(1)]; // Array to hold the 8bit transmission array

                            // Get the device's Int16 image array
                            Int16[,] int2dArray = (Int16[,])imageArray;

                            // Parellelise the array copy to improve performance
                            Parallel.For(0, imageArray.GetLength(0), (i, state) => // Iterate over the slowest changing dimension
                            {
                                byte byteElementValue; // Local variable to hold the Byte element being tested (saves calculating an array offset later in the process)
                                Int16 int16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)

                                bool arrayIsByteInternal = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.

                                // Iterate over the fastest changing dimension
                                for (int j = 0; j < imageArray.GetLength(1); j++)
                                {
                                    // Get the current array element value
                                    int16ElementValue = int2dArray[i, j];

                                    // Truncate the supplied 2-byte Int16 value to create a 1-byte Byte value
                                    byteElementValue = (byte)int16ElementValue;

                                    // Store the Byte value in the array.
                                    byteArray[i, j] = byteElementValue;

                                    // Compare the Byte and Int16 values, indicating whether they match. 
                                    if (byteElementValue != int16ElementValue) arrayIsByteInternal = false;

                                }

                                // Update the master arrayIsByte variable.
                                arrayIsByte &= arrayIsByteInternal;

                                // Terminate the parallel for loop early if the image data is determined to be 16bit
                                if (!arrayIsByte) state.Break();

                            });

                            // Return the Byte array values if these were provided by the device
                            if (arrayIsByte) // Supplied array has Byte values so return the shorter array in place of the supplied Int16 array
                            {
                                imageArray = byteArray; // Assign the Byte array to the imageArray variable in place of the Int16 array
                                transmissionElementType = ImageArrayElementTypes.Byte; // Flag that the array elements are Byte
                                transmissionElementSize = sizeof(byte); // Indicate that the transmitted elements are of Byte size rather than Int16 size
                            }
                            else
                            {
                                // No action, continue to use the supplied Int16 array because its values fall outside the Byte number range
                            }
                            break;

                        case 3:
                            byte[,,] byte3dArray = new byte[imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2)]; // Array to hold the 8bit transmission array

                            // Get the device's Int16 image array
                            Int16[,,] int3dArray = (Int16[,,])imageArray;

                            // Parellelise the array copy to improve performance
                            Parallel.For(0, imageArray.GetLength(0), (i, state) => // Iterate over the slowest changing dimension
                            {
                                bool arrayIsByteInternal1 = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.

                                // Iterate over the mid changing dimension
                                for (int j = 0; j < imageArray.GetLength(1); j++)
                                {
                                    byte byteElementValue; // Local variable to hold the Byte element being tested (saves calculating an array offset later in the process)
                                    Int16 int16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)
                                    bool arrayIsByteInternal2 = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.

                                    // Iterate over the fastest changing dimension
                                    for (int k = 0; k < imageArray.GetLength(2); k++)
                                    {
                                        // Get the current array element value
                                        int16ElementValue = int3dArray[i, j, k];

                                        // Truncate the supplied 2-byte Int16 value to create a 1-byte Byte value
                                        byteElementValue = (byte)int16ElementValue;

                                        // Store the Byte value in the array.
                                        byte3dArray[i, j, k] = byteElementValue;

                                        // Compare the Byte and Int16 values, indicating whether they match. 
                                        if (byteElementValue != int16ElementValue) arrayIsByteInternal2 = false;

                                    }

                                    // Update the arrayIsByteInternal1variable.
                                    arrayIsByteInternal1 &= arrayIsByteInternal2;

                                }

                                // Update the master arrayIsByte variable as the logical AND of the mater and update values.
                                arrayIsByte &= arrayIsByteInternal1;

                                // Terminate the parallel for loop early if the image data is determined to be 16bit
                                if (!arrayIsByte) state.Break();
                            });

                            // Return the appropriate array if either Byte array values were provided by the device
                            if (arrayIsByte) // Supplied array has Byte values so return the shorter array in place of the supplied Int16 array
                            {
                                imageArray = byte3dArray; // Assign the Byte array to the imageArray variable in place of the Int16 array
                                transmissionElementType = ImageArrayElementTypes.Byte; // Flag that the array elements are Byte
                                transmissionElementSize = sizeof(byte); // Indicate that the transmitted elements are of Byte size rather than Int16 size
                            }
                            else
                            {
                                // No action, continue to use the supplied Int16 array because its values fall outside the Byte number range.
                            }
                            break;

                        default:
                            throw new InvalidValueException($"ToByteArray - The camera returned an array of rank: {imageArray.Rank}, which is not supported.");
                    }

                    break;

                case TypeCode.UInt16:
                    intendedElementType = ImageArrayElementTypes.UInt16;
                    transmissionElementType = ImageArrayElementTypes.UInt16;
                    transmissionElementSize = 2;

                    // Special handling for UInt16 arrays - see if we can convert them to a Byte array

                    arrayIsByte = true; // Flag indicating whether the supplied array conforms to the Byte value range 0 to +255. Start by assuming success

                    // Handle 2D and 3D arrays
                    switch (imageArray.Rank)
                    {
                        case 2:
                            byte[,] byteArray = new byte[imageArray.GetLength(0), imageArray.GetLength(1)]; // Array to hold the 8bit transmission array

                            // Get the device's Int16 image array
                            UInt16[,] uint2dArray = (UInt16[,])imageArray;

                            // Parellelise the array copy to improve performance
                            Parallel.For(0, imageArray.GetLength(0), (i, state) => // Iterate over the slowest changing dimension
                            {
                                byte byteElementValue; // Local variable to hold the Byte element being tested (saves calculating an array offset later in the process)
                                UInt16 uint16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)

                                bool arrayIsByteInternal = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.

                                // Iterate over the fastest changing dimension
                                for (int j = 0; j < imageArray.GetLength(1); j++)
                                {
                                    // Get the current array element value
                                    uint16ElementValue = uint2dArray[i, j];

                                    // Truncate the supplied 2-byte UInt16 value to create a 1-byte Byte value
                                    byteElementValue = (byte)uint16ElementValue;

                                    // Store the Byte value in the array.
                                    byteArray[i, j] = byteElementValue;

                                    // Compare the Byte and UInt16 values, indicating whether they match. 
                                    if (byteElementValue != uint16ElementValue) arrayIsByteInternal = false;

                                }

                                // Update the master arrayIsByte variable.
                                arrayIsByte &= arrayIsByteInternal;

                                // Terminate the parallel for loop early if the image data is determined to be 16bit
                                if (!arrayIsByte) state.Break();

                            });

                            // Return the Byte array values if these were provided by the device
                            if (arrayIsByte) // Supplied array has Byte values so return the shorter array in place of the supplied UInt16 array
                            {
                                imageArray = byteArray; // Assign the Byte array to the imageArray variable in place of the UInt16 array
                                transmissionElementType = ImageArrayElementTypes.Byte; // Flag that the array elements are Byte
                                transmissionElementSize = sizeof(byte); // Indicate that the transmitted elements are of Byte size rather than UInt16 size
                            }
                            else
                            {
                                // No action, continue to use the supplied Int16 array because its values fall outside the Byte number range
                            }
                            break;

                        case 3:
                            byte[,,] byte3dArray = new byte[imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2)]; // Array to hold the 8bit transmission array

                            // Get the device's Int16 image array
                            UInt16[,,] uint3dArray = (UInt16[,,])imageArray;

                            // Parellelise the array copy to improve performance
                            Parallel.For(0, imageArray.GetLength(0), (i, state) => // Iterate over the slowest changing dimension
                            {
                                bool arrayIsByteInternal1 = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.

                                // Iterate over the mid changing dimension
                                for (int j = 0; j < imageArray.GetLength(1); j++)
                                {
                                    byte byteElementValue; // Local variable to hold the Byte element being tested (saves calculating an array offset later in the process)
                                    UInt16 uint16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)
                                    bool arrayIsByteInternal2 = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.

                                    // Iterate over the fastest changing dimension
                                    for (int k = 0; k < imageArray.GetLength(2); k++)
                                    {
                                        // Get the current array element value
                                        uint16ElementValue = uint3dArray[i, j, k];

                                        // Truncate the supplied 2-byte UInt16 value to create a 1-byte Byte value
                                        byteElementValue = (byte)uint16ElementValue;

                                        // Store the Byte value in the array.
                                        byte3dArray[i, j, k] = byteElementValue;

                                        // Compare the Byte and UInt16 values, indicating whether they match. 
                                        if (byteElementValue != uint16ElementValue) arrayIsByteInternal2 = false;

                                    }

                                    // Update the arrayIsByteInternal1variable.
                                    arrayIsByteInternal1 &= arrayIsByteInternal2;

                                }

                                // Update the master arrayIsByte variable as the logical AND of the mater and update values.
                                arrayIsByte &= arrayIsByteInternal1;

                                // Terminate the parallel for loop early if the image data is determined to be 16bit
                                if (!arrayIsByte) state.Break();
                            });

                            // Return the appropriate array if either Byte array values were provided by the device
                            if (arrayIsByte) // Supplied array has Byte values so return the shorter array in place of the supplied UInt16 array
                            {
                                imageArray = byte3dArray; // Assign the byte array to the imageArray variable in place of the UInt16 array
                                transmissionElementType = ImageArrayElementTypes.Byte; // Flag that the array elements are Byte
                                transmissionElementSize = sizeof(byte); // Indicate that the transmitted elements are of Byte size rather than UInt16 size
                            }
                            else
                            {
                                // No action, continue to use the supplied Int16 array because its values fall outside the Byte number range.
                            }
                            break;

                        default:
                            throw new InvalidValueException($"ToByteArray - The camera returned an array of rank: {imageArray.Rank}, which is not supported.");
                    }


                    break;

                case TypeCode.Int32:
                    intendedElementType = ImageArrayElementTypes.Int32;
                    transmissionElementType = ImageArrayElementTypes.Int32;
                    transmissionElementSize = 4;

                    // Special handling for Int32 arrays - see if we can convert them to Int16 or UInt16 arrays

                    // NOTE
                    // NOTE - This algorithm uses a UInt16 array to transmit an array with Int16 values because we are only interested in the byte values for this purpose,
                    // NOTE - not the arithmetic interpretation of those bytes.
                    // NOTE

                    arrayIsByte = true; // Flag indicating whether the supplied array conforms to the Byte value range 0 to +255. Start by assuming success
                    bool arrayIsInt16 = true; // Flag indicating whether the supplied array conforms to the Int16 value range -32768 to +32767. Start by assuming success
                    bool arrayIsUint16 = true; // Flag indicating whether the supplied array conforms to the UInt16 value range 0 to +65535. Start by assuming success

                    // Handle 2D and 3D arrays
                    switch (imageArray.Rank)
                    {
                        case 2:
                            byte[,] byteArray = new byte[imageArray.GetLength(0), imageArray.GetLength(1)]; // Array to hold the 8bit transmission array
                            UInt16[,] uInt16Array = new UInt16[imageArray.GetLength(0), imageArray.GetLength(1)]; // Array to hold the 16bit transmission array (either Int16 or UInt16 values)

                            // Get the device's Int32 image array
                            int[,] int2dArray = (int[,])imageArray;

                            // Parellelise the array copy to improve performance
                            Parallel.For(0, imageArray.GetLength(0), (i, state) => // Iterate over the slowest changing dimension
                            {
                                byte byteElementValue; // Local variable to hold the Byte element being tested (saves calculating an array offset later in the process)
                                Int32 int32ElementValue; // Local variable to hold the Int32 element being tested (saves calculating an array offset later in the process)
                                Int16 int16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)
                                UInt16 uInt16ElementValue; // Local variable to hold the Unt16 element value (saves calculating an array offset later in the process)

                                bool arrayIsByteInternal = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.
                                bool arrayIsInt16Internal = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsInt16 variable.
                                bool arrayIsUint16Internal = true; // Local variable to hold the UInt16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsInt16 variable.

                                // Iterate over the fastest changing dimension
                                for (int j = 0; j < imageArray.GetLength(1); j++)
                                {
                                    // Get the current array element value
                                    int32ElementValue = int2dArray[i, j];

                                    // Truncate the supplied 4-byte Int32 value to create a 1-byte Byte value
                                    byteElementValue = (byte)int32ElementValue;

                                    // Truncate the supplied 4-byte Int32 value to create a 2-byte UInt16 value
                                    uInt16ElementValue = (UInt16)int32ElementValue;

                                    // Truncate the supplied 4-byte Int32 value to create a 2-byte Int16 value
                                    int16ElementValue = (Int16)int32ElementValue;

                                    // Store the Byte value in the array.
                                    byteArray[i, j] = byteElementValue;

                                    // Store the UInt16 value in the array.
                                    uInt16Array[i, j] = uInt16ElementValue;

                                    // Compare the Byte and Int32 values, indicating whether they match. 
                                    if (byteElementValue != int32ElementValue) arrayIsByteInternal = false;

                                    // Compare the Int16 and Int32 values, indicating whether they match. 
                                    if (int16ElementValue != int32ElementValue) arrayIsInt16Internal = false;

                                    // Compare the UInt16 and Int32 values, indicating whether they match. 
                                    if (uInt16ElementValue != int32ElementValue) arrayIsUint16Internal = false;
                                }

                                // Update the master arrayIsInt16 and arrayIsUint16 variables as the logical AND of the mater and update values.
                                arrayIsByte &= arrayIsByteInternal;
                                arrayIsInt16 &= arrayIsInt16Internal;
                                arrayIsUint16 &= arrayIsUint16Internal;

                                // Terminate the parallel for loop early if the image data is determined to be 32bit
                                if (!arrayIsInt16 & !arrayIsUint16) state.Break();

                            });

                            // Return the appropriate array if either a Byte, UInt16 or Int16 array was provided by the device
                            if (arrayIsByte) // Supplied array has UInt16 values so return the shorter array in place of the supplied Int32 array
                            {
                                imageArray = byteArray; // Assign the Int16 array to the imageArray variable in place of the Int32 array
                                transmissionElementType = ImageArrayElementTypes.Byte; // Flag that the array elements are UInt16
                                transmissionElementSize = sizeof(byte); // Indicate that the transmitted elements are of UInt16 size rather than Int32 size
                            }
                            else if (arrayIsUint16) // Supplied array has UInt16 values so return the shorter array in place of the supplied Int32 array
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
                            byte[,,] byte3dArray = new byte[imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2)]; // Array to hold the 8bit transmission array
                            UInt16[,,] uInt163dArray = new UInt16[imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2)];  // Array to hold the 16bit transmission array (either Int16 or UInt16 values)

                            // Get the device's Int32 image array
                            Int32[,,] int3dArray = (Int32[,,])imageArray;

                            // Parellelise the array copy to improve performance
                            Parallel.For(0, imageArray.GetLength(0), (i, state) => // Iterate over the slowest changing dimension
                            {
                                bool arrayIsByteInternal1 = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.
                                bool arrayIsInt16Internal1 = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayisInt16 variable.
                                bool arrayIsUint16Internal1 = true; // Local variable to hold the UInt16 status within this particular thread. Used to reduce thread conflict when updating the arrayisInt16 variable.

                                // Iterate over the mid changing dimension
                                for (int j = 0; j < imageArray.GetLength(1); j++)
                                {
                                    byte byteElementValue; // Local variable to hold the Byte element being tested (saves calculating an array offset later in the process)
                                    Int32 int32ElementValue; // Local variable to hold the Int32 element being tested (saves calculating an array offset later in the process)
                                    Int16 int16ElementValue; // Local variable to hold the Int16 element value (saves calculating an array offset later in the process)
                                    UInt16 uInt16ElementValue; // Local variable to hold the UInt16 element value (saves calculating an array offset later in the process)
                                    bool arrayIsByteInternal2 = true; // Local variable to hold the Byte status within this particular thread. Used to reduce thread conflict when updating the arrayIsByte variable.
                                    bool arrayIsInt16Internal2 = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsInt16 variable.
                                    bool arrayIsUInt16Internal2 = true; // Local variable to hold the Int16 status within this particular thread. Used to reduce thread conflict when updating the arrayIsUInt16 variable.

                                    // Iterate over the fastest changing dimension
                                    for (int k = 0; k < imageArray.GetLength(2); k++)
                                    {
                                        // Get the current array element value
                                        int32ElementValue = int3dArray[i, j, k];

                                        // Truncate the supplied 4-byte Int32 value to create a 1-byte Byte value
                                        byteElementValue = (byte)int32ElementValue;

                                        // Truncate the supplied 4-byte Int32 value to create a 2-byte UInt16 value
                                        uInt16ElementValue = (UInt16)int32ElementValue;

                                        // Truncate the supplied 4-byte Int32 value to create a 2-byte Int16 value
                                        int16ElementValue = (Int16)int32ElementValue;

                                        // Store the Byte value in the array.
                                        byte3dArray[i, j, k] = byteElementValue;

                                        // Store the UInt16 value to the corresponding Int16 array element. 
                                        uInt163dArray[i, j, k] = uInt16ElementValue;

                                        // Compare the Byte and Int32 values, indicating whether they match. 
                                        if (byteElementValue != int32ElementValue) arrayIsByteInternal2 = false;

                                        // Compare the Int16 and Int32 values. 
                                        if (int16ElementValue != int32ElementValue) arrayIsInt16Internal2 = false; // If they are not the same the Int32 value was outside the range of Int16 and arrayIsInt16Internal will be set false

                                        // Compare the UInt16 and Int32 values. 
                                        if (uInt16ElementValue != int32ElementValue) arrayIsUInt16Internal2 = false;
                                    }

                                    // Update the arrayIsInt16Internal1 and arrayIsUint16Internal1 variables as the logical AND of the mater and update values.
                                    arrayIsByteInternal1 &= arrayIsByteInternal2;
                                    arrayIsInt16Internal1 &= arrayIsInt16Internal2;
                                    arrayIsUint16Internal1 &= arrayIsUInt16Internal2;

                                }

                                // Update the master arrayIsInt16 and arrayIsUint16 variables as the logical AND of the mater and update values.
                                arrayIsByte &= arrayIsByteInternal1;
                                arrayIsInt16 &= arrayIsInt16Internal1;
                                arrayIsUint16 &= arrayIsUint16Internal1;

                                // Terminate the parallel for loop early if the image data is determined to be 32bit
                                if (!arrayIsInt16 & !arrayIsUint16) state.Break();
                            });

                            // Return the appropriate array if either a Byte, UInt16 or Int16 array was provided by the device
                            if (arrayIsByte) // Supplied array has UInt16 values so return the shorter array in place of the supplied Int32 array
                            {
                                imageArray = byte3dArray; // Assign the Int16 array to the imageArray variable in place of the Int32 array
                                transmissionElementType = ImageArrayElementTypes.Byte; // Flag that the array elements are UInt16
                                transmissionElementSize = sizeof(byte); // Indicate that the transmitted elements are of UInt16 size rather than Int32 size
                            }
                            else if (arrayIsUint16) // Supplied array has UInt16 values so return the shorter array in place of the supplied Int32 array
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
                            throw new InvalidValueException($"ToByteArray - The camera returned an array of rank: {imageArray.Rank}, which is not supported.");
                    }

                    break;

                case TypeCode.UInt32:
                    intendedElementType = ImageArrayElementTypes.UInt32;
                    transmissionElementType = ImageArrayElementTypes.UInt32;
                    transmissionElementSize = 4;
                    break;

                case TypeCode.Int64:
                    intendedElementType = ImageArrayElementTypes.Int64;
                    transmissionElementType = ImageArrayElementTypes.Int64;
                    transmissionElementSize = 8;
                    break;

                case TypeCode.UInt64:
                    intendedElementType = ImageArrayElementTypes.UInt64;
                    transmissionElementType = ImageArrayElementTypes.UInt64;
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

                case TypeCode.Object:
                    intendedElementType = ImageArrayElementTypes.Object;

                    // Get the type name of the elements within the objectt array
                    string elementTypeName = "";

                    switch (imageArray.Rank)
                    {
                        case 2:
                            elementTypeName = imageArray.GetValue(0, 0).GetType().Name;
                            break;

                        case 3:
                            elementTypeName = imageArray.GetValue(0, 0, 0).GetType().Name;
                            break;
                    }

                    switch (elementTypeName)
                    {
                        case "Byte":
                            transmissionElementType = ImageArrayElementTypes.Byte;
                            transmissionElementSize = 1;
                            break;

                        case "Int16":
                            transmissionElementType = ImageArrayElementTypes.Int16;
                            transmissionElementSize = 2;
                            break;

                        case "UInt16":
                            transmissionElementType = ImageArrayElementTypes.UInt16;
                            transmissionElementSize = 2;
                            break;

                        case "Int32":
                            transmissionElementType = ImageArrayElementTypes.Int32;
                            transmissionElementSize = 4;
                            break;

                        case "UInt32":
                            transmissionElementType = ImageArrayElementTypes.UInt32;
                            transmissionElementSize = 4;
                            break;

                        case "Int64":
                            transmissionElementType = ImageArrayElementTypes.Int64;
                            transmissionElementSize = 8;
                            break;

                        case "UInt64":
                            transmissionElementType = ImageArrayElementTypes.UInt64;
                            transmissionElementSize = 8;
                            break;

                        case "Single":
                            transmissionElementType = ImageArrayElementTypes.Single;
                            transmissionElementSize = 4;
                            break;

                        case "Double":
                            transmissionElementType = ImageArrayElementTypes.Double;
                            transmissionElementSize = 8;
                            break;

                        default:
                            throw new InvalidValueException($"ToByteArray - Received an unsupported object array element type: {elementTypeName}");

                    }

                    break;

                default:
                    throw new InvalidValueException($"ToByteArray - Received an unsupported return array type: {imageArray.GetType().Name}, with elements of type: {imageArray.GetType().GetElementType().Name} with TypeCode: {arrayElementTypeCode}");
            }


            switch (metadataVersion)
            {
                case 1:
                    // Create a version 1 metadata structure
                    ArrayMetadataV1 metadataVersion1;
                    if (imageArray.Rank == 2) metadataVersion1 = new ArrayMetadataV1(AlpacaErrors.AlpacaNoError, clientTransactionID, serverTransactionID, intendedElementType, transmissionElementType, 2, imageArray.GetLength(0), imageArray.GetLength(1), 0);
                    else metadataVersion1 = new ArrayMetadataV1(AlpacaErrors.AlpacaNoError, clientTransactionID, serverTransactionID, intendedElementType, transmissionElementType, 3, imageArray.GetLength(0), imageArray.GetLength(1), imageArray.GetLength(2));

                    // Turn the metadata structure into a byte array
                    byte[] metadataBytes = metadataVersion1.ToByteArray<ArrayMetadataV1>();

                    // Create a return array of size equal to the sum of the metadata and image array lengths
                    byte[] imageArrayBytes = new byte[imageArray.Length * transmissionElementSize + metadataBytes.Length]; // Size the image array bytes as the product of the transmission element size and the number of elements

                    // Copy the metadata bytes to the start of the return byte array
                    Array.Copy(metadataBytes, imageArrayBytes, metadataBytes.Length);

                    // Copy the image array bytes after the metadata
                    if (arrayElementTypeCode != TypeCode.Object) // For all arrays, except object arrays, copy the image array directly to the byte array
                    {
                        Buffer.BlockCopy(imageArray, 0, imageArrayBytes, metadataBytes.Length, imageArray.Length * transmissionElementSize);
                    }
                    else // Special handling for object arrays
                    {
                        int startOfNextElement = ARRAY_METADATAV1_LENGTH;
                        switch (imageArray.Rank)
                        {
                            case 2:
                                switch (transmissionElementType)
                                {
                                    case ImageArrayElementTypes.Byte:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((Byte)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Int16:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((Int16)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.UInt16:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((UInt16)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Int32:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((Int32)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.UInt32:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((UInt32)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Int64:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((Int64)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.UInt64:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((UInt64)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Single:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((Single)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Double:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                Array.Copy(BitConverter.GetBytes((Double)imageArray.GetValue(i, j)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                startOfNextElement += transmissionElementSize;
                                            }
                                        }
                                        break;

                                    default:
                                        throw new InvalidValueException($"Unsupported object array element type: {imageArray.GetValue(0, 0, 0).GetType().Name}");
                                }
                                break;

                            case 3:
                                switch (transmissionElementType)
                                {
                                    case ImageArrayElementTypes.Byte:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((Byte)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Int16:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((Int16)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.UInt16:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((UInt16)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Int32:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((Int32)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.UInt32:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((UInt32)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Int64:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((Int64)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.UInt64:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((UInt64)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Single:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((Single)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    case ImageArrayElementTypes.Double:
                                        for (int i = 0; i < imageArray.GetLength(0); i++)
                                        {
                                            for (int j = 0; j < imageArray.GetLength(1); j++)
                                            {
                                                for (int k = 0; k < imageArray.GetLength(2); k++)
                                                {
                                                    Array.Copy(BitConverter.GetBytes((Double)imageArray.GetValue(i, j, k)), 0, imageArrayBytes, startOfNextElement, transmissionElementSize);
                                                    startOfNextElement += transmissionElementSize;
                                                }
                                            }
                                        }
                                        break;

                                    default:
                                        throw new InvalidValueException($"Unsupported object array element type: {imageArray.GetValue(0, 0, 0).GetType().Name}");
                                }

                                break;
                        }

                    }


                    // Return the byte array
                    return imageArrayBytes;

                default:
                    throw new InvalidValueException($"ToByteArray - Unsupported metadata version: {metadataVersion}");
            }

        }

        /// <summary>
        /// Alpaca Extension - Convert a byte array to a 2D or 3D mage array based on the array metadata.
        /// </summary>
        /// <param name="imageBytes">byte array to convert</param>
        /// <returns>2D or 3D array as specified in the array metadata.</returns>
        /// <exception cref="InvalidValueException">The byte array is null.</exception>
        public static Array ToImageArray(this byte[] imageBytes)
        {
            ImageArrayElementTypes imageElementType;
            ImageArrayElementTypes transmissionElementType;
            int rank;
            int dimension1;
            int dimension2;
            int dimension3;
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
                    dimension1 = metadataV1.Dimension1;
                    dimension2 = metadataV1.Dimension2;
                    dimension3 = metadataV1.Dimension3;
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

            if ((imageElementType == ImageArrayElementTypes.Int16) & (transmissionElementType == ImageArrayElementTypes.Byte)) // Handle the special case where Int32 has been converted to Byte for transmission
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        byte[,] byte2dArray = new byte[dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, byte2dArray, 0, imageBytes.Length - dataStart);

                        Int16[,] int2dArray = new Int16[dimension1, dimension2];
                        Parallel.For(0, byte2dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < byte2dArray.GetLength(1); j++)
                            {
                                int2dArray[i, j] = byte2dArray[i, j];
                            }
                        });
                        return int2dArray;

                    case 3: // Rank 3
                        byte[,,] byte3dArray = new byte[dimension1, dimension2, dimension3];
                        Buffer.BlockCopy(imageBytes, dataStart, byte3dArray, 0, imageBytes.Length - dataStart);

                        Int16[,,] int3dArray = new Int16[dimension1, dimension2, dimension3];
                        Parallel.For(0, byte3dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < byte3dArray.GetLength(1); j++)
                            {
                                for (int k = 0; k < byte3dArray.GetLength(2); k++)
                                {
                                    int3dArray[i, j, k] = byte3dArray[i, j, k];
                                }
                            }
                        });
                        return int3dArray;

                    default:
                        throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                }
            }
            else if ((imageElementType == ImageArrayElementTypes.UInt16) & (transmissionElementType == ImageArrayElementTypes.Byte)) // Handle the special case where Int32 has been converted to Byte for transmission
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        byte[,] byte2dArray = new byte[dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, byte2dArray, 0, imageBytes.Length - dataStart);

                        UInt16[,] uint2dArray = new UInt16[dimension1, dimension2];
                        Parallel.For(0, byte2dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < byte2dArray.GetLength(1); j++)
                            {
                                uint2dArray[i, j] = byte2dArray[i, j];
                            }
                        });
                        return uint2dArray;

                    case 3: // Rank 3
                        byte[,,] byte3dArray = new byte[dimension1, dimension2, dimension3];
                        Buffer.BlockCopy(imageBytes, dataStart, byte3dArray, 0, imageBytes.Length - dataStart);

                        UInt16[,,] uint3dArray = new UInt16[dimension1, dimension2, dimension3];
                        Parallel.For(0, byte3dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < byte3dArray.GetLength(1); j++)
                            {
                                for (int k = 0; k < byte3dArray.GetLength(2); k++)
                                {
                                    uint3dArray[i, j, k] = byte3dArray[i, j, k];
                                }
                            }
                        });
                        return uint3dArray;

                    default:
                        throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                }
            }
            else if ((imageElementType == ImageArrayElementTypes.Int32) & (transmissionElementType == ImageArrayElementTypes.Byte)) // Handle the special case where Int32 has been converted to Byte for transmission
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        byte[,] byte2dArray = new byte[dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, byte2dArray, 0, imageBytes.Length - dataStart);

                        Int32[,] int2dArray = new Int32[dimension1, dimension2];
                        Parallel.For(0, byte2dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < byte2dArray.GetLength(1); j++)
                            {
                                int2dArray[i, j] = byte2dArray[i, j];
                            }
                        });
                        return int2dArray;

                    case 3: // Rank 3
                        byte[,,] byte3dArray = new byte[dimension1, dimension2, dimension3];
                        Buffer.BlockCopy(imageBytes, dataStart, byte3dArray, 0, imageBytes.Length - dataStart);

                        Int32[,,] int3dArray = new Int32[dimension1, dimension2, dimension3];
                        Parallel.For(0, byte3dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < byte3dArray.GetLength(1); j++)
                            {
                                for (int k = 0; k < byte3dArray.GetLength(2); k++)
                                {
                                    int3dArray[i, j, k] = byte3dArray[i, j, k];
                                }
                            }
                        });
                        return int3dArray;

                    default:
                        throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                }
            }
            else if ((imageElementType == ImageArrayElementTypes.Int32) & (transmissionElementType == ImageArrayElementTypes.Int16)) // Handle the special case where Int32 has been converted to Int16 for transmission
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        Int16[,] short2dArray = new Int16[dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, short2dArray, 0, imageBytes.Length - dataStart);

                        int[,] int2dArray = new int[dimension1, dimension2];
                        Parallel.For(0, short2dArray.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < short2dArray.GetLength(1); j++)
                            {
                                int2dArray[i, j] = short2dArray[i, j];
                            }
                        });
                        return int2dArray;

                    case 3: // Rank 3
                        Int16[,,] short3dArray = new Int16[dimension1, dimension2, dimension3];
                        Buffer.BlockCopy(imageBytes, dataStart, short3dArray, 0, imageBytes.Length - dataStart);

                        int[,,] int3dArray = new int[dimension1, dimension2, dimension3];
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
                        UInt16[,] uInt16Array2D = new UInt16[dimension1, dimension2];
                        Buffer.BlockCopy(imageBytes, dataStart, uInt16Array2D, 0, imageBytes.Length - dataStart);

                        int[,] int2dArray = new int[dimension1, dimension2];
                        Parallel.For(0, uInt16Array2D.GetLength(0), (i) =>
                        {
                            for (int j = 0; j < uInt16Array2D.GetLength(1); j++)
                            {
                                int2dArray[i, j] = uInt16Array2D[i, j];
                            }
                        });
                        return int2dArray;

                    case 3: // Rank 3
                        UInt16[,,] uInt16Array3D = new UInt16[dimension1, dimension2, dimension3];
                        Buffer.BlockCopy(imageBytes, dataStart, uInt16Array3D, 0, imageBytes.Length - dataStart);

                        int[,,] int3dArray = new int[dimension1, dimension2, dimension3];
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
            else if (imageElementType == ImageArrayElementTypes.Object)
            {
                switch (rank)
                {
                    case 2: // Rank 2
                        switch (transmissionElementType)
                        {
                            case ImageArrayElementTypes.Byte:
                                Object[,] byteArray2D = new Object[dimension1, dimension2];
                                int nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        byteArray2D[i, j] = imageBytes[nextArrayElement];
                                        nextArrayElement += 1;
                                    }
                                }
                                return byteArray2D;

                            case ImageArrayElementTypes.Int16:
                                Object[,] int16Array2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        int16Array2D[i, j] = BitConverter.ToInt16(imageBytes, nextArrayElement);
                                        nextArrayElement += 2;
                                    }
                                }
                                return int16Array2D;

                            case ImageArrayElementTypes.UInt16:
                                Object[,] uint16Array2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        uint16Array2D[i, j] = BitConverter.ToUInt16(imageBytes, nextArrayElement);
                                        nextArrayElement += 2;
                                    }
                                }
                                return uint16Array2D;

                            case ImageArrayElementTypes.Int32:
                                Object[,] int32Array2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        int32Array2D[i, j] = BitConverter.ToInt32(imageBytes, nextArrayElement);
                                        nextArrayElement += 4;
                                    }
                                }
                                return int32Array2D;

                            case ImageArrayElementTypes.UInt32:
                                Object[,] uint32Array2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        uint32Array2D[i, j] = BitConverter.ToUInt32(imageBytes, nextArrayElement);
                                        nextArrayElement += 4;
                                    }
                                }
                                return uint32Array2D;

                            case ImageArrayElementTypes.Int64:
                                Object[,] int64Array2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        int64Array2D[i, j] = BitConverter.ToInt64(imageBytes, nextArrayElement);
                                        nextArrayElement += 8;
                                    }
                                }
                                return int64Array2D;

                            case ImageArrayElementTypes.UInt64:
                                Object[,] uint64Array2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        uint64Array2D[i, j] = BitConverter.ToUInt64(imageBytes, nextArrayElement);
                                        nextArrayElement += 8;
                                    }
                                }
                                return uint64Array2D;

                            case ImageArrayElementTypes.Single:
                                Object[,] singleArray2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        singleArray2D[i, j] = BitConverter.ToSingle(imageBytes, nextArrayElement);
                                        nextArrayElement += 4;
                                    }
                                }
                                return singleArray2D;

                            case ImageArrayElementTypes.Double:
                                Object[,] doubleArray2D = new Object[dimension1, dimension2];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        doubleArray2D[i, j] = BitConverter.ToDouble(imageBytes, nextArrayElement);
                                        nextArrayElement += 8;
                                    }
                                }
                                return doubleArray2D;

                            default:
                                throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                        }

                    case 3: // Rank 3
                        switch (transmissionElementType)
                        {
                            case ImageArrayElementTypes.Byte:
                                Object[,,] byteArray3D = new Object[dimension1, dimension2, dimension3];
                                int nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            byteArray3D[i, j, k] = (Byte)imageBytes[nextArrayElement];
                                            nextArrayElement += 1;
                                        }
                                    }
                                }
                                return byteArray3D;

                            case ImageArrayElementTypes.Int16:
                                Object[,,] int16Array3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            int16Array3D[i, j, k] = BitConverter.ToInt16(imageBytes, nextArrayElement);
                                            nextArrayElement += 2;
                                        }
                                    }
                                }
                                return int16Array3D;

                            case ImageArrayElementTypes.UInt16:
                                Object[,,] uint16Array3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            uint16Array3D[i, j, k] = BitConverter.ToUInt16(imageBytes, nextArrayElement);
                                            nextArrayElement += 2;
                                        }
                                    }
                                }
                                return uint16Array3D;

                            case ImageArrayElementTypes.Int32:
                                Object[,,] int32Array3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            int32Array3D[i, j, k] = BitConverter.ToInt32(imageBytes, nextArrayElement);
                                            nextArrayElement += 4;
                                        }
                                    }
                                }
                                return int32Array3D;

                            case ImageArrayElementTypes.UInt32:
                                Object[,,] uint32Array3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            uint32Array3D[i, j, k] = BitConverter.ToUInt32(imageBytes, nextArrayElement);
                                            nextArrayElement += 4;
                                        }
                                    }
                                }
                                return uint32Array3D;

                            case ImageArrayElementTypes.Int64:
                                Object[,,] int64Array3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            int64Array3D[i, j, k] = BitConverter.ToInt64(imageBytes, nextArrayElement);
                                            nextArrayElement += 8;
                                        }
                                    }
                                }
                                return int64Array3D;

                            case ImageArrayElementTypes.UInt64:
                                Object[,,] uint64Array3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            uint64Array3D[i, j, k] = BitConverter.ToUInt64(imageBytes, nextArrayElement);
                                            nextArrayElement += 8;
                                        }
                                    }
                                }
                                return uint64Array3D;

                            case ImageArrayElementTypes.Single:
                                Object[,,] singleArray3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            singleArray3D[i, j, k] = BitConverter.ToSingle(imageBytes, nextArrayElement);
                                            nextArrayElement += 4;
                                        }
                                    }
                                }
                                return singleArray3D;

                            case ImageArrayElementTypes.Double:
                                Object[,,] doubleArray3D = new Object[dimension1, dimension2, dimension3];
                                nextArrayElement = ARRAY_METADATAV1_LENGTH;
                                for (int i = 0; i < dimension1; i++)
                                {
                                    for (int j = 0; j < dimension2; j++)
                                    {
                                        for (int k = 0; k < dimension3; k++)
                                        {
                                            doubleArray3D[i, j, k] = BitConverter.ToDouble(imageBytes, nextArrayElement);
                                            nextArrayElement += 8;
                                        }
                                    }
                                }
                                return doubleArray3D;

                            default:
                                throw new InvalidValueException($"ToImageArray - Returned array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                        }

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
                                    byte[,] byte2dArray = new byte[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, byte2dArray, 0, imageBytes.Length - dataStart);
                                    return byte2dArray;

                                case 3: // Rank 3
                                    byte[,,] byte3dArray = new byte[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, byte3dArray, 0, imageBytes.Length - dataStart);
                                    return byte3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned byte array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Int16:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    short[,] short2dArray = new short[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, short2dArray, 0, imageBytes.Length - dataStart);
                                    return short2dArray;

                                case 3: // Rank 3
                                    short[,,] short3dArray = new short[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, short3dArray, 0, imageBytes.Length - dataStart);
                                    return short3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int16 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.UInt16:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    UInt16[,] uInt16Array2D = new UInt16[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt16Array2D, 0, imageBytes.Length - dataStart);
                                    return uInt16Array2D;

                                case 3: // Rank 3
                                    UInt16[,,] uInt16Array3D = new UInt16[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt16Array3D, 0, imageBytes.Length - dataStart);
                                    return uInt16Array3D;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned UInt16 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Int32:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    int[,] int2dArray = new int[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, int2dArray, 0, imageBytes.Length - dataStart);
                                    return int2dArray;

                                case 3: // Rank 3
                                    int[,,] int3dArray = new int[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, int3dArray, 0, imageBytes.Length - dataStart);
                                    return int3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int32 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.UInt32:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    UInt32[,] uInt32Array2D = new UInt32[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt32Array2D, 0, imageBytes.Length - dataStart);
                                    return uInt32Array2D;

                                case 3: // Rank 3
                                    UInt32[,,] uInt32Array3D = new UInt32[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, uInt32Array3D, 0, imageBytes.Length - dataStart);
                                    return uInt32Array3D;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned UInt32 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Int64:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Int64[,] int642dArray = new Int64[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, int642dArray, 0, imageBytes.Length - dataStart);
                                    return int642dArray;

                                case 3: // Rank 3
                                    Int64[,,] int643dArray = new Int64[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, int643dArray, 0, imageBytes.Length - dataStart);
                                    return int643dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Int64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.UInt64:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    UInt64[,] uint64Array2D = new UInt64[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, uint64Array2D, 0, imageBytes.Length - dataStart);
                                    return uint64Array2D;

                                case 3: // Rank 3
                                    UInt64[,,] uint64Array3D = new UInt64[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, uint64Array3D, 0, imageBytes.Length - dataStart);
                                    return uint64Array3D;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned UInt64 array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Single:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Single[,] single2dArray = new Single[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, single2dArray, 0, imageBytes.Length - dataStart);
                                    return single2dArray;

                                case 3: // Rank 3
                                    Single[,,] single3dArray = new Single[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, single3dArray, 0, imageBytes.Length - dataStart);
                                    return single3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Single array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Double:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Double[,] double2dArray = new Double[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, double2dArray, 0, imageBytes.Length - dataStart);
                                    return double2dArray;

                                case 3: // Rank 3
                                    Double[,,] double3dArray = new Double[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, double3dArray, 0, imageBytes.Length - dataStart);
                                    return double3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Double array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
                            }

                        case ImageArrayElementTypes.Object:
                            switch (rank)
                            {
                                case 2: // Rank 2
                                    Object[,] object2dArray = new Object[dimension1, dimension2];
                                    Buffer.BlockCopy(imageBytes, dataStart, object2dArray, 0, imageBytes.Length - dataStart);
                                    return object2dArray;

                                case 3: // Rank 3
                                    Object[,,] object3dArray = new Object[dimension1, dimension2, dimension3];
                                    Buffer.BlockCopy(imageBytes, dataStart, object3dArray, 0, imageBytes.Length - dataStart);
                                    return object3dArray;

                                default:
                                    throw new InvalidValueException($"ToImageArray - Returned Object array cannot be handled because it does not have a rank of 2 or 3. Returned array rank:{rank}.");
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

        /// <summary>
        /// Alpaca Extension - Returns the metadata version in use within the byte array presentation of an image array.
        /// </summary>
        /// <param name="imageBytes">Source byte array.</param>
        /// <returns>Integer metadata version number.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static int GetMetadataVersion(this byte[] imageBytes)
        {
            if (imageBytes.Length < ARRAY_METADATAV1_LENGTH) throw new InvalidOperationException($"GetMetadataVersion - Supplied array size: {imageBytes.Length} is smaller than the minimum metadata size ({ARRAY_METADATAV1_LENGTH})");
            return BitConverter.ToInt32(imageBytes, 0);
        }

        /// <summary>
        /// Alpaca Extension - Extracts the error message from a byte array returned by an Alpaca device.
        /// </summary>
        /// <param name="errorMessageBytes">The byte array from which to extract the error message.</param>
        /// <returns>The error message as a string.</returns>
        /// <exception cref="InvalidOperationException">The byte array is smaller than the smallest metadata size.</exception>
        /// <exception cref="InvalidOperationException">The byte array equals the smallest metadata length and thus does not contain an error message.</exception>
        public static string GetErrrorMessage(this byte[] errorMessageBytes)
        {
            // Validate error message array
            if (errorMessageBytes.Length < ARRAY_METADATAV1_LENGTH) throw new InvalidOperationException($"GetErrrorMessage - Supplied array size: {errorMessageBytes.Length} is smaller than the minimum metadata size ({ARRAY_METADATAV1_LENGTH})");
            if (errorMessageBytes.Length == ARRAY_METADATAV1_LENGTH) throw new InvalidOperationException($"GetErrrorMessage - The byte array length equals the metadata length, the supplied array does not contain any message bytes.");

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

        /// <summary>
        /// Alpaca Extension - Extracts the array metadata from in version 1 form from a byte array returned by an Alpaca device.
        /// </summary>
        /// <param name="imageBytes">The byte array from which to extract the metadata.</param>
        /// <returns>The metadata as a version 1 structure.</returns>
        /// <exception cref="InvalidOperationException">The byte array is smaller than the version 1 metadata size.</exception>
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

        /// <summary>
        /// Alpaca Extension - Returns the image data from an ImageBytes byte array.
        /// </summary>
        /// <param name="imageBytes">ImageVBytes array containing the image data.</param>
        /// <returns>Byte array continaing the image data</returns>
        /// <exception cref="InvalidValueException">The ImageArray data contains an unsupported metadata version.</exception>
        public static byte[] GetImageData(this byte[] imageBytes)
        {
            int metadataVersion = imageBytes.GetMetadataVersion();
            switch (metadataVersion)
            {
                case 1:
                    ArrayMetadataV1 metadata = imageBytes.GetMetadataV1();
                    byte[] imageData = new byte[imageBytes.Length - ARRAY_METADATAV1_LENGTH];
                    Array.Copy(imageBytes, metadata.DataStart, imageData, 0, imageData.Length);
                    return imageData;

                default:
                    throw new InvalidValueException($"GetImageBytes - The supplied array contains an unsupported metadata version number: {metadataVersion}. This component supports metadata version 1.");
            }
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

    }
}
