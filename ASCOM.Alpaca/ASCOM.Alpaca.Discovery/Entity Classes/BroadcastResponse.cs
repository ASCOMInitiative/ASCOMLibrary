using System.Net;

namespace ASCOM.Alpaca.Discovery
{
    /// <summary>
    /// Response to an Alpaca discovery broadcast.
    /// </summary>
    public class BroadcastResponse
    {
        /// <summary>
        /// Create a new instance of the BroadcastResponse class using the provided IP endpoint and response bytes
        /// </summary>
        /// <param name="iPEndpoint"></param>
        /// <param name="response"></param>
        public BroadcastResponse(IPEndPoint iPEndpoint, byte[] response) 
        {
            IPEndpoint = iPEndpoint;
            Response = response;
        }

        /// <summary>
        /// IP address of the responding device
        /// </summary>
        public IPEndPoint IPEndpoint
        {
            get; private set;
        }

        /// <summary>
        /// Response in the returned UDP packet.
        /// </summary>
        public byte[] Response
        {
            get; private set;
        }
    }
}
