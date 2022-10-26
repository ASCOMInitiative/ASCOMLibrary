using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ASCOM.Common;
using ASCOM.Alpaca.Clients;
using System.Threading.Tasks;
using ASCOM.Tools;

namespace ASCOM.Alpaca.Tests.Clients
{
    public static class ExtensionTests
    {
        [Fact]
        public static async Task  AsyncAltAz()
        {
            TraceLogger TL = new TraceLogger("AsyncAltAz",true);
            AlpacaTelescope client = new AlpacaTelescope();
            client.Connected = true;
            await client.AsyncSlewToAltAz(0, 0,logger:TL);
            client.Connected = false;
            Assert.NotNull(client);
            client.Dispose();
        }
    }
}
