using System.Text.Json.Serialization;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// 3 dimension image array response
    /// </summary>
    public class IntJaggedArray3DResponse : Response, IArrayResponse<int[][][]>
    {
        /// <summary>
        /// Create a new IntArray3DResponse with default values
        /// </summary>
        public IntJaggedArray3DResponse()
        {
            Value = new int[0][][]; // Make sure that Value contains at least an empty array 
        }

        /// <summary>
        /// Create a new IntArray3DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public IntJaggedArray3DResponse(uint clientTransactionID, uint serverTransactionID, int[][][] value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new IntArray3DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public IntJaggedArray3DResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Image array type (int32)
        /// </summary>
        public ArrayType Type { get; } = ArrayType.Int;

        /// <summary>
        /// The array's rank, will be 3 (multi plane image (colour)).
        /// </summary>
        public int Rank { get; } = 3;

        /// <summary>
        /// 3D image array of int32 values
        /// </summary>
        [JsonPropertyOrder(1000)]
        public int[][][] Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "Int32 3D array is null";
            return $"Int32 array ({Value.GetLength(0)} x {Value.GetLength(1)} x {Value.GetLength(2)})";
        }
    }
}