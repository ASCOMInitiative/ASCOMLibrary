namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// 2 dimension image array response
    /// </summary>
    public class ShortArray2DResponse : Response, IArrayResponse<short[,]>
    {
        /// <summary>
        /// Create a new ShortArray2DResponse with default values
        /// </summary>
        public ShortArray2DResponse()
        {
            Value = new short[0, 0]; // Make sure that Value contains at least an empty array 
        }

        /// <summary>
        /// Create a new ShortArray2DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public ShortArray2DResponse(uint clientTransactionID, uint serverTransactionID, short[,] value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new ShortArray2DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public ShortArray2DResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Image array type (int16)
        /// </summary>
        public ArrayType Type { get; } = ArrayType.Short;

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)).
        /// </summary>
        public int Rank { get; } = 2;

        /// <summary>
        /// Short 2d array returned by the device
        /// </summary>
        public short[,] Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "Int16 2D array is null";
            return $"Int16 array ({Value.GetLength(0)} x {Value.GetLength(1)})";
        }
    }
}