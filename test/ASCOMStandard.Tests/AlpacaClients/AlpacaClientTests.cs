using ASCOM.Alpaca.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AlpacaClients
{
    [Collection("AlpacaClientTests")]
#if NET8_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public class AlpacaClientTests
    {
        static  AlpacaConfiguration configuration= new();

        [Fact]
        public void GetDeviceCamera()
        {
            AlpacaCamera device = AlpacaClient.GetDevice<AlpacaCamera>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetCoverCalibrator()
        {
            AlpacaCoverCalibrator device = AlpacaClient.GetDevice<AlpacaCoverCalibrator>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceDome()
        {
            AlpacaDome device = AlpacaClient.GetDevice<AlpacaDome>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceFilterWheel()
        {
            AlpacaFilterWheel device = AlpacaClient.GetDevice<AlpacaFilterWheel>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceFocuser()
        {
            AlpacaFocuser device = AlpacaClient.GetDevice<AlpacaFocuser>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceObservingConditions()
        {
            AlpacaObservingConditions device = AlpacaClient.GetDevice<AlpacaObservingConditions>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceRotator()
        {
            AlpacaRotator device = AlpacaClient.GetDevice<AlpacaRotator>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceSafetyMonitor()
        {
            AlpacaSafetyMonitor device = AlpacaClient.GetDevice<AlpacaSafetyMonitor>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceSwitch()
        {
            AlpacaSwitch device = AlpacaClient.GetDevice<AlpacaSwitch>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void GetDeviceTelescope()
        {
            AlpacaTelescope device = AlpacaClient.GetDevice<AlpacaTelescope>(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaCamera()
        {
            AlpacaCamera device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaCoverCalibrator()
        {
            AlpacaCoverCalibrator device = new (configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaDome()
        {
            AlpacaDome device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaFilterWheel()
        {
            AlpacaFilterWheel device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaFocuser()
        {
            AlpacaFocuser device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaObservingConditions()
        {
            AlpacaObservingConditions device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaRotator()
        {
            AlpacaRotator device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaSafetyMonitor()
        {
            AlpacaSafetyMonitor device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaSwitch()
        {
            AlpacaSwitch device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }

        [Fact]
        public void AlpacaTelescope()
        {
            AlpacaTelescope device = new(configuration);
            Assert.NotNull(device);
            device.Dispose();
        }
    }
}
