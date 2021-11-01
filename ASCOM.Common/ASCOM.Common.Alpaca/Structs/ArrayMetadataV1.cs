using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ASCOM.Common.Alpaca
{
    [StructLayout(LayoutKind.Explicit, Size = AlpacaTools.ARRAY_METADATAV1_LENGTH)]
    public struct ArrayMetadataV1
    {
        public ArrayMetadataV1(ImageArrayElementTypes imageElementType,
                                     ImageArrayElementTypes transmissionElementType,
                                     int arrayRank,
                                     int arrayDimension0,
                                     int arrayDimension1,
                                     int arrayDimension2,
                                     AlpacaErrors errorNumber)
        {
            MetadataVersion = 1;
            this.ImageElementType = imageElementType;
            this.TransmissionElementType = transmissionElementType;
            this.Rank = arrayRank;
            this.Dimension0 = arrayDimension0;
            this.Dimension1 = arrayDimension1;
            this.Dimension2 = arrayDimension2;
            this.ErrorNumber = errorNumber;
            DataStart = AlpacaTools.ARRAY_METADATAV1_LENGTH;
        }

        [FieldOffset(0)] public int MetadataVersion; // Bytes 0..3 - Metadata version starting at 1, must always be the first field
        [FieldOffset(4)] public AlpacaErrors ErrorNumber; // Bytes 4..7 - Alpaca error number or zero for success
        [FieldOffset(8)] public int DataStart; // Bytes 8..11 - Offset of the start of the returned data byte array
        [FieldOffset(12)] public ImageArrayElementTypes ImageElementType; // Bytes 12..15 - Element type of the source image array
        [FieldOffset(16)] public ImageArrayElementTypes TransmissionElementType; // Bytes 16..19 - Element type of the array as transmitted over the network
        [FieldOffset(20)] public int Rank; // Bytes 20..23 - image array rank
        [FieldOffset(24)] public int Dimension0; // Bytes 24..27 - Length of image array first dimension
        [FieldOffset(28)] public int Dimension1; // Bytes 28..31 - Length of image array second dimension
        [FieldOffset(32)] public int Dimension2; // Bytes 32..35 - Length of image array third dimension (0 for 2D array)
    }
}
