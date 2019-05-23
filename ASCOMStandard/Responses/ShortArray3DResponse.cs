namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// 3 dimension image array response
    /// </summary>
    public class ShortArray3DResponse : Response, IArrayResponse<short[,,]>
    {
        /// <summary>
        /// Create a new ShortArray3DResponse with default values
        /// </summary>
        public ShortArray3DResponse()
        {
            Value = new short[0, 0, 0]; // Make sure that Value contains at least an empty array 
        }

        /// <summary>
        /// Create a new ShortArray3DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public ShortArray3DResponse(uint clientTransactionID, uint serverTransactionID, short[,,] value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new ShortArray3DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public ShortArray3DResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        ///  3D image array of short (int16) values
        /// </summary>
        public short[,,] Value { get; set; }

        /// <summary>
        /// Image array type (int16)
        /// </summary>
        public ArrayType Type { get; } = ArrayType.Short;

        /// <summary>
        /// The array's rank, will be 3 (multi plane image (colour)).
        /// </summary>
        public int Rank { get; } = 3;

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "Int16 3D array is null";
            return $"Int16 array ({Value.GetLength(0)} x {Value.GetLength(1)} x {Value.GetLength(2)})";
        }
    }
}