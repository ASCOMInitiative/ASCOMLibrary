using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// ImageBytes metadata structure version 1
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = AlpacaTools.ARRAY_METADATAV1_LENGTH)]
    public struct ArrayMetadataV1
    {
        /// <summary>
        /// Initialise the ArrayMetadataV1 structure
        /// </summary>
        /// <param name="errorNumber">Transaction error number.</param>
        /// <param name="clientTransactionID">Client's transaction ID</param>
        /// <param name="serverTransactionID">Device's transaction ID</param>
        /// <param name="imageElementType">Intended element type of the resultant array.</param>
        /// <param name="transmissionElementType">Element type actually sent.</param>
        /// <param name="arrayRank">Rank of the arrayresultant array.</param>
        /// <param name="arrayDimension1">Size of the first dimension of the resultant array (array[Dimension1, Dimension2, Dimension3]).</param>
        /// <param name="arrayDimension2">Size of the second dimension of the resultant array (array[Dimension1, Dimension2, Dimension3]).</param>
        /// <param name="arrayDimension3">Size of the third dimension of the resultant array (array[Dimension1, Dimension2, Dimension3]).</param>
        public ArrayMetadataV1(AlpacaErrors errorNumber,
                               uint clientTransactionID,
                               uint serverTransactionID,
                               ImageArrayElementTypes imageElementType,
                               ImageArrayElementTypes transmissionElementType,
                               int arrayRank,
                               int arrayDimension1,
                               int arrayDimension2,
                               int arrayDimension3)
        {
            MetadataVersion = 1;
            this.ErrorNumber = errorNumber;
            this.ClientTransactionID = clientTransactionID;
            this.ServerTransactionID = serverTransactionID;
            DataStart = AlpacaTools.ARRAY_METADATAV1_LENGTH;
            this.ImageElementType = imageElementType;
            this.TransmissionElementType = transmissionElementType;
            this.Rank = arrayRank;
            this.Dimension1 = arrayDimension1;
            this.Dimension2 = arrayDimension2;
            this.Dimension3 = arrayDimension3;
        }

        /// <summary>
        /// Metadata version
        /// </summary>
        [FieldOffset(0)] public int MetadataVersion; // Bytes 0..3 - Metadata version starting at 1, must always be the first field

        /// <summary>
        /// Alpaca error number, 0 for success
        /// </summary>
        [FieldOffset(4)] public AlpacaErrors ErrorNumber; // Bytes 4..7 - Alpaca error number or zero for success

        /// <summary>
        /// Client's transaction ID
        /// </summary>
        [FieldOffset(8)] public uint ClientTransactionID; // Bytes 8..11 - Client's transaction ID

        /// <summary>
        /// Device's transaction ID
        /// </summary>
        [FieldOffset(12)] public uint ServerTransactionID; // Bytes 12..15 - Device's transaction ID

        /// <summary>
        /// Offset to the start of the returned data bytes or UTF8 encoded error message.
        /// </summary>
        [FieldOffset(16)] public int DataStart; // Bytes 16..19 - Offset of the start of the returned data byte array

        /// <summary>
        /// Type of element in the image array as supplied by the device
        /// </summary>
        [FieldOffset(20)] public ImageArrayElementTypes ImageElementType; // Bytes 20..23 - Element type of the source image array

        /// <summary>
        /// Type of element being transmitted over the network. Can be smaller in byte size than the ImageElementType
        /// </summary>
        [FieldOffset(24)] public ImageArrayElementTypes TransmissionElementType; // Bytes 24..27 - Element type of the array as transmitted over the network

        /// <summary>
        /// Array rank
        /// </summary>
        [FieldOffset(28)] public int Rank; // Bytes 28..31 - image array rank

        /// <summary>
        /// Length of the array's first dimension (array[Dimension1, Dimension2, Dimension3]) 
        /// </summary>
        [FieldOffset(32)] public int Dimension1; // Bytes 32..35 - Length of image array first dimension

        /// <summary>
        /// Length of the array's second dimension (array[Dimension1, Dimension2, Dimension3])
        /// </summary>
        [FieldOffset(36)] public int Dimension2; // Bytes 36..39 - Length of image array second dimension

        /// <summary>
        /// Length of the array's third dimension (array[Dimension1, Dimension2, Dimension3]) - 0 for a 2D array. 
        /// </summary>
        [FieldOffset(40)] public int Dimension3; // Bytes 40..43 - Length of image array third dimension (0 for 2D array)
    }
}
