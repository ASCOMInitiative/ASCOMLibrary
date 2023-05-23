using ASCOM.Common.Com;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Defines the properties that are common to all Alpaca responses.
    /// </summary>
    /// <remarks>
    /// If a command does not return a value, use <see cref="CommandCompleteResponse"/> instead of this class.
    /// </remarks>
    public class Response : ErrorResponse, IResponse
    {
        /// <summary>
        /// Client's transaction ID (0 to 4294967295), as supplied by the client in the command request.
        /// </summary>
        [JsonPropertyOrder(10)]
        public uint ClientTransactionID { get; set; }

        /// <summary>
        /// Server's transaction ID (0 to 4294967295), should be unique for each client transaction so that log messages on the client can be associated with logs on the device.
        /// </summary>
        [JsonPropertyOrder(20)]
        public uint ServerTransactionID { get; set; }
    }
}