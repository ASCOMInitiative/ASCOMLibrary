using ASCOM.Alpaca.Errors;

namespace ASCOM.Alpaca.Responses
{
    /// <summary>
    /// Defines the base of an Alpaca response.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Client's transaction ID (0 to 4294967295), as supplied by the client in the command request.
        /// </summary>
        uint ClientTransactionID { get; set; }

        /// <summary>
        /// Server's transaction ID (0 to 4294967295), should be unique for each client transaction so that log messages on the client can be associated with logs on the device.
        /// </summary>
        uint ServerTransactionID { get; set; }

        /// <summary>
        /// Zero for a successful transaction, or a non-zero integer(-2147483648 to 2147483647) if the device encountered an issue.Devices must use ASCOM reserved error
        /// numbers whenever appropriate so that clients can take informed actions. E.g.returning 0x401 (1025) to indicate that an invalid value was received.
        /// </summary>
        /// <seealso cref="ErrorCodes"/>
        int ErrorNumber { get; set; }

        /// <summary>
        /// Empty string for a successful transaction, or a message describing the issue that was encountered. If an error message is returned,
        /// a non zero <see cref="ErrorNumber"/> must also be returned.
        /// </summary>
        string ErrorMessage { get; set; }
    }
}