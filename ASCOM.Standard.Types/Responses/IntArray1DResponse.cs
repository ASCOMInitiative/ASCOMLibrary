namespace ASCOM.Alpaca.Responses
{
    public class IntArray1DResponse : Response, IArrayResponse<int[]>
    {
        /// <summary>
        /// Create a new IntArray1DResponse with default values
        /// </summary>
        public IntArray1DResponse()
        {
            Value = new int[0];
        }

        /// <summary>
        /// Create a new IntArray1DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public IntArray1DResponse(uint clientTransactionID, uint serverTransactionID, int[] value)
        {
            ServerTransactionID = serverTransactionID;
            ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new IntArray1DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public IntArray1DResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, ErrorCodes errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        public int[] Value { get; set; }

        /// <summary>
        /// Image array type (int32)
        /// </summary>
        public ArrayType Type { get; } = ArrayType.Int;

        /// <summary>
        /// The array's rank is 1
        /// </summary>
        public int Rank { get; } = 1;

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "Int32 1D array is null";
            return $"Int32 array ({Value.GetLength(0)})";
        }

    }
}