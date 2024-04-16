using ASCOM.Com.DriverAccess;
using System;
using Xunit.Abstractions;
using Xunit;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace DriverAccess
{
    [Collection("SwitchTests")]
    public class SwitchAsyncTests(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper output = output;

        [Fact]
        public void SetSwitch()
        {
            // Create a Switch simulator device
            Type switchType = Type.GetTypeFromProgID("ASCOM.Simulator.Switch");
            dynamic switchSim = Activator.CreateInstance(switchType);

            // Connect
            switchSim.Connected = true;
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is false
            Assert.False(switchSim.GetSwitch(10));

            // Set the value true synchronously (should take 3 seconds to complete)
            switchSim.SetSwitch(10, true);

            // Confirm the expected outcomes
            Assert.True(switchSim.StateChangeComplete(10));
            Assert.True(switchSim.GetSwitch(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public async void SetAsync()
        {
            // Create a Switch simulator device
            Type switchType = Type.GetTypeFromProgID("ASCOM.Simulator.Switch");
            dynamic switchSim = Activator.CreateInstance(switchType);

            // Connect
            switchSim.Connected = true;
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is false
            Assert.False(switchSim.GetSwitch(10));

            // Set the value true asynchronously (should take 3 seconds to complete)
            switchSim.SetAsync(10, true);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            // Wait for most of 3 seconds and make sure the operation is still running
            await Task.Delay(2800);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            // Wait a short while longer to make sure that the new value is in effect
            await Task.Delay(350);
            Assert.True(switchSim.StateChangeComplete(10));
            Assert.True(switchSim.GetSwitch(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public void SetSwitchValue()
        {
            // Create a Switch simulator device
            Type switchType = Type.GetTypeFromProgID("ASCOM.Simulator.Switch");
            dynamic switchSim = Activator.CreateInstance(switchType);

            // Connect
            switchSim.Connected = true;
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is false
            Assert.False(switchSim.GetSwitch(10));

            // Set the value true synchronously (should take 3 seconds to complete)
            switchSim.SetSwitchValue(10, 5.0);

            // Confirm the expected outcomes
            Assert.True(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(5.0, switchSim.GetSwitchValue(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public async void SetAsyncValue()
        {
            // Create a Switch simulator device
            Type switchType = Type.GetTypeFromProgID("ASCOM.Simulator.Switch");
            dynamic switchSim = Activator.CreateInstance(switchType);

            // Connect
            switchSim.Connected = true;
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is 0.0
            Assert.Equal<double>(0.0, switchSim.GetSwitchValue(10));

            // Set the value true asynchronously (should take 3 seconds to complete)
            switchSim.SetAsyncValue(10, 5.0);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(0.0, switchSim.GetSwitchValue(10));

            // Wait for most of 3 seconds and make sure the operation is still running
            await Task.Delay(2950);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(0.0, switchSim.GetSwitchValue(10));

            // Wait a short while longer to make sure that the new value is in effect
            await Task.Delay(150);
            Assert.True(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(5.0, switchSim.GetSwitchValue(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public async void CancelAsync()
        {
            // Create a Switch simulator device
            Type switchType = Type.GetTypeFromProgID("ASCOM.Simulator.Switch");
            dynamic switchSim = Activator.CreateInstance(switchType);

            // Connect
            switchSim.Connected = true;
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is false
            Assert.False(switchSim.GetSwitch(10));

            // Set the value true asynchronously (should take 3 seconds to complete)
            switchSim.SetAsync(10, true);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            // Wait for 2 seconds and then cancel the operation
            await Task.Delay(2000);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));
            switchSim.CancelAsync(10);

            // Wait a short while before checking that the operation is cancelled
            await Task.Delay(100);
            Assert.Throws<COMException>(() => switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            // Wait until after 3 seconds and make sure that the outcome is still the same
            await Task.Delay(1000);
            Assert.Throws<COMException>(() => switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public async void SetAsyncDriverAccess()
        {
            // Create a Switch simulator device
            using (Switch switchSim = new("ASCOM.Simulator.Switch"))
            {

                // Connect
                switchSim.Connected = true;
                Assert.True(switchSim.Connected);

                // Confirm that some devices are configured
                Assert.Equal<short>(11, switchSim.MaxSwitch);

                // Confirm that device 10 is async
                Assert.False(switchSim.CanAsync(9));
                Assert.True(switchSim.CanAsync(10));

                // Confirm that the current value is false
                Assert.False(switchSim.GetSwitch(10));

                // Set the value true asynchronously (should take 3 seconds to complete)
                switchSim.SetAsync(10, true);
                Assert.False(switchSim.StateChangeComplete(10));
                Assert.False(switchSim.GetSwitch(10));

                // Wait for most of 3 seconds and make sure the operation is still running
                await Task.Delay(2950);
                Assert.False(switchSim.StateChangeComplete(10));
                Assert.False(switchSim.GetSwitch(10));

                // Wait a short while longer to make sure that the new value is in effect
                await Task.Delay(150);
                Assert.True(switchSim.StateChangeComplete(10));
                Assert.True(switchSim.GetSwitch(10));

                switchSim.Connected = false;
            }

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public async void SetAsyncValueDriverAccess()
        {
            // Create a Switch simulator device
            Switch switchSim = new("ASCOM.Simulator.Switch")
            {
                // Connect
                Connected = true
            };
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is 0.0
            Assert.Equal<double>(0.0, switchSim.GetSwitchValue(10));

            // Set the value true asynchronously (should take 3 seconds to complete)
            switchSim.SetAsyncValue(10, 5.0);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(0.0, switchSim.GetSwitchValue(10));

            // Wait for most of 3 seconds and make sure the operation is still running
            await Task.Delay(2950);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(0.0, switchSim.GetSwitchValue(10));

            // Wait a short while longer to make sure that the new value is in effect
            await Task.Delay(150);
            Assert.True(switchSim.StateChangeComplete(10));
            Assert.Equal<double>(5.0, switchSim.GetSwitchValue(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        [Fact]
        public async void CancelAsyncDriverAccess()
        {
            // Create a Switch simulator device
            Switch switchSim = new("ASCOM.Simulator.Switch")
            {
                // Connect
                Connected = true
            };
            Assert.True(switchSim.Connected);

            // Confirm that some devices are configured
            Assert.Equal<short>(11, switchSim.MaxSwitch);

            // Confirm that device 10 is async
            Assert.False(switchSim.CanAsync(9));
            Assert.True(switchSim.CanAsync(10));

            // Confirm that the current value is false
            Assert.False(switchSim.GetSwitch(10));

            // Set the value true asynchronously (should take 3 seconds to complete)
            switchSim.SetAsync(10, true);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            // Wait for 2 seconds and then cancel the operation
            await Task.Delay(2000);
            Assert.False(switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));
            switchSim.CancelAsync(10);

            // Wait a short while before checking that the operation is cancelled and an OperationCancelledException is thrown
            await Task.Delay(100);
            Assert.Throws<OperationCanceledException>(() => switchSim.StateChangeComplete(10));

            Assert.False(switchSim.GetSwitch(10));

            // Wait until after 3 seconds and make sure that the outcome is still the same
            await Task.Delay(1000);
            Assert.Throws<OperationCanceledException>(() => switchSim.StateChangeComplete(10));
            Assert.False(switchSim.GetSwitch(10));

            switchSim.Connected = false;

            // Wait to make sure test is complete
            SwitchDelay();
        }

        private static void SwitchDelay()
        {
            Thread.Sleep(3000);
        }
    }
}
