using System.Text.Json.Serialization;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// 2 dimension image array response
    /// </summary>
    public class DoubleArray2DResponse : Response, IArrayResponse<double[,]>
    {
        /// <summary>
        /// Create a new DoubleArray2DResponse with default values
        /// </summary>
        public DoubleArray2DResponse()
        {
            Value = new double[0, 0]; // Make sure that Value contains at least an empty array 
        }

        /// <summary>
        /// Create a new DoubleArray2DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public DoubleArray2DResponse(uint clientTransactionID, uint serverTransactionID, double[,] value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new DoubleArray2DResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public DoubleArray2DResponse(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Image array type (double)
        /// </summary>
        public ArrayType Type { get; } = ArrayType.Double;

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)).
        /// </summary>
        public int Rank { get; } = 2;

        /// <summary>
        /// 2D image array of double values
        /// </summary>
        [JsonPropertyOrder(1000)]
        public double[,] Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "Double 2D array is null";
            return $"Double array ({Value.GetLength(0)} x {Value.GetLength(1)})";
        }
    }
}