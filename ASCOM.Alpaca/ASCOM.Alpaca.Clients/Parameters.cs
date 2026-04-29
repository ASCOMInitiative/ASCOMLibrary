using ASCOM.Common.Interfaces;

using System.Net.Http;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Holds the common connection and context parameters shared by all DynamicClientDriver remote access methods.
    /// </summary>
    internal class Parameters
    {
        internal uint ClientNumber { get; }
        internal HttpClient Client { get; }
        internal int Timeout { get; }
        internal string URIBase { get; }
        internal bool StrictCasing { get; }
        internal ILogger Logger { get; }
        internal string Method { get; }
        internal MemberTypes MemberType { get; }

        internal Parameters(uint clientNumber, HttpClient client, int timeout, string uriBase, bool strictCasing, ILogger logger, string method, MemberTypes memberType)
        {
            ClientNumber = clientNumber;
            Client = client;
            Timeout = timeout;
            URIBase = uriBase;
            StrictCasing = strictCasing;
            Logger = logger;
            Method = method;
            MemberType = memberType;
        }
    }
}
