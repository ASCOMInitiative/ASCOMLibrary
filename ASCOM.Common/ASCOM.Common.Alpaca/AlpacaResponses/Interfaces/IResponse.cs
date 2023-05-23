namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Defines the base of an Alpaca response.
    /// </summary>
    public interface IResponse : IErrorResponse
    {
        /// <summary>
        /// Client's transaction ID (0 to 4294967295), as supplied by the client in the command request.
        /// </summary>
        uint ClientTransactionID { get; set; }

        /// <summary>
        /// Server's transaction ID (0 to 4294967295), should be unique for each client transaction so that log messages on the client can be associated with logs on the device.
        /// </summary>
        uint ServerTransactionID { get; set; }
    }
}