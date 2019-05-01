namespace ASCOM.Alpaca.Responses
{
    public interface IResponse
    {
        /// <summary>
        /// minimum: 0
        /// maximum: 4294967295
        /// Client's transaction ID (0 to 4294967295), as supplied by the client in the command request.
        /// </summary>
        int ClientTransactionID { get; }

        /// <summary>
        /// minimum: 0
        /// maximum: 4294967295
        /// Server's transaction ID (0 to 4294967295), should be unique for each client transaction so that log messages on the client can be associated with logs on the device.
        /// </summary>
        int ServerTransactionID { get; }

        /// <summary>
        /// minimum: -2147483648
        /// maximum: 2147483647
        /// Zero for a successful transaction, or a non-zero integer(-2147483648 to 2147483647) if the device encountered an issue.Devices must use ASCOM reserved error
        /// numbers whenever appropriate so that clients can take informed actions.E.g.returning 0x401 (1025) to indicate that an invalid value was received (see Alpaca
        /// API definition and developer documentation for further information)
        /// </summary>
        int ErrorNumber { get; }

        /// <summary>
        /// Empty string for a successful transaction, or a message describing the issue that was encountered. If an error message is returned, a non zero error number must also be returned.
        /// </summary>
        string ErrorMessage { get; }
    }
}