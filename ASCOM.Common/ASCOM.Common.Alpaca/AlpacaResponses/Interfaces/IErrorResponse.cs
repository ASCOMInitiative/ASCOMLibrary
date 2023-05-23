using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Defines the base of an Alpaca response.
    /// </summary>
    public interface IErrorResponse
    {
        /// <summary>
        /// Zero for a successful transaction, or a non-zero integer(-2147483648 to 2147483647) if the device encountered an issue.Devices must use ASCOM reserved error
        /// numbers whenever appropriate so that clients can take informed actions. E.g.returning 0x401 (1025) to indicate that an invalid value was received.
        /// </summary>
        /// <seealso cref="AlpacaErrors"/>
        AlpacaErrors ErrorNumber { get; set; }

        /// <summary>
        /// Empty string for a successful transaction, or a message describing the issue that was encountered. If an error message is returned,
        /// a non zero <see cref="ErrorNumber"/> must also be returned.
        /// </summary>
        string ErrorMessage { get; set; }
    }
}
