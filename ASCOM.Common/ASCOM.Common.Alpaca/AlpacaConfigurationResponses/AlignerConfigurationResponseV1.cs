using ASCOM.Alpaca.Clients;
using ASCOM.Common.DeviceInterfaces;
using System.Linq;
using System.Reflection;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Response that returns a collection of <see cref="AxisRate"/> values.
    /// </summary>
    public class AlignerConfigurationResponseV1 : Response, IValueResponse<AlignerConfigurationV1Dto>
    {
        /// <summary>
        /// Create a new AlignerConfigurationResponse with default values
        /// </summary>
        public AlignerConfigurationResponseV1()
        {
            Value = new AlignerConfigurationV1Dto(); // Make sure that Value contains at least an empty collection 
        }

        /// <summary>
        /// Create a new AlignerConfigurationResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="value">Value to return</param>
        public AlignerConfigurationResponseV1(uint clientTransactionID, uint serverTransactionID, AlignerConfigurationV1Dto value)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            Value = value;
        }

        /// <summary>
        /// Create a new AlignerConfigurationResponse with the supplied parameter values
        /// </summary>
        /// <param name="clientTransactionID">Client transaction ID</param>
        /// <param name="serverTransactionID">Server transaction ID</param>
        /// <param name="errorMessage">Value to return</param>
        /// <param name="errorCode">Server transaction ID</param>
        public AlignerConfigurationResponseV1(uint clientTransactionID, uint serverTransactionID, string errorMessage, AlpacaErrors errorCode)
        {
            base.ServerTransactionID = serverTransactionID;
            base.ClientTransactionID = clientTransactionID;
            base.ErrorMessage = errorMessage;
            base.ErrorNumber = errorCode;
        }

        /// <summary>
        /// Aligner configuration returned by the device
        /// </summary>
        public AlignerConfigurationV1Dto Value { get; set; }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            if (Value == null) return "AlignerConfiguration value is null";

            return string.Join(", ", typeof(AlignerConfigurationV1Dto).GetProperties(BindingFlags.Public).Select(property => $"{property.Name} : {property.GetValue(Value) ?? "Not implemented"}"));
        }
    }
}