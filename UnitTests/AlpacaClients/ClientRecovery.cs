using ASCOM.Alpaca.Clients;
using ASCOM.Alpaca.Discovery;
using ASCOM.Common;
using ASCOM.Common.Interfaces;
using ASCOM.Tools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AlpacaClients
{
    [Collection("AlpacaClientTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class ClientRecovery
    {
        static int PORT_NUMBER = 11111;

        [Fact]
        public void GoodLocalHostAddressAndPort()
        {
            TraceLogger logger = new TraceLogger("GoodAddressAndPort", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "127.0.0.1";
            configuration.PortNumber = PORT_NUMBER;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.True(device.Connected);

            device.Dispose();
        }

        [Fact]
        public void BadLocalHostAddress()
        {
            TraceLogger logger = new TraceLogger("BadLocalHostAddress", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "127.2.0.1";
            configuration.PortNumber = PORT_NUMBER;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.ThrowsAny<Exception>(() => { var connected = device.Connected; });

            device.Dispose();
        }

        [Fact]
        public void BadLocalHostIpPort()
        {
            TraceLogger logger = new TraceLogger("BadLocalHostIpPort", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "127.0.0.1";
            configuration.PortNumber = PORT_NUMBER + 100;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.ThrowsAny<Exception>(() => { var connected = device.Connected; });

            device.Dispose();
        }
        [Fact]
        public void BadIpAddress()
        {
            TraceLogger logger = new TraceLogger("BadIpAddress", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "192.168.0.225";
            configuration.PortNumber = PORT_NUMBER;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.ThrowsAny<Exception>(() => { var connected = device.Connected; });

            device.Dispose();
        }

        [Fact]
        public void BadIpPort()
        {
            TraceLogger logger = new TraceLogger("BadIpPort", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "192.168.0.222";
            configuration.PortNumber = PORT_NUMBER + 100;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.ThrowsAny<Exception>(() => { var connected = device.Connected; });

            device.Dispose();
        }

        [Fact]
        public async Task SelfHealBadIpPort()
        {
            TraceLogger logger = new TraceLogger("SelfHealBadIpPort", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            string uniqueId = await GetUniqueId(DeviceTypes.Camera);
            logger.LogMessage("SelfHealBadIpPort", $"UniqueId={uniqueId}");

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "192.168.0.241";
            configuration.PortNumber = PORT_NUMBER + 100;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;
            configuration.UniqueId = uniqueId;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.True(device.Connected);

            device.Dispose();
        }

        [Fact]
        public async Task SelfHealBadIpPortNoLogger()
        {
           string uniqueId = await GetUniqueId(DeviceTypes.Camera);

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "192.168.0.241";
            configuration.PortNumber = PORT_NUMBER + 100;
            configuration.Logger = null;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;
            configuration.UniqueId = uniqueId;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.True(device.Connected);

            device.Dispose();
        }

        [Fact]
        public async Task SelfHealBadIpAddress()
        {
            TraceLogger logger = new TraceLogger("SelfHealBadIpAddress", true);
            logger.SetMinimumLoggingLevel(LogLevel.Debug);

            string uniqueId = await GetUniqueId(DeviceTypes.Camera);
            logger.LogMessage("SelfHealBadIpAddress", $"UniqueId={uniqueId}");

            AlpacaConfiguration configuration = new AlpacaConfiguration();
            configuration.IpAddressString = "192.168.0.119";
            configuration.PortNumber = PORT_NUMBER;
            configuration.Logger = logger;
            configuration.EstablishConnectionTimeout = 1;
            configuration.StandardDeviceResponseTimeout = 1;
            configuration.LongDeviceResponseTimeout = 1;
            configuration.NumberOfRetries = 0;
            configuration.UniqueId = uniqueId;

            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);

            Assert.NotNull(device);

            Assert.True(device.Connected);

            device.Dispose();
        }

        private async Task<string> GetUniqueId(DeviceTypes deviceType)
        {

            List<AscomDevice> devices=await AlpacaDiscovery.GetAscomDevicesAsync(deviceType);
            if(devices.Count==0)
                throw new Exception("No devices found");

            return devices[0].UniqueId;
        }
    }
}
