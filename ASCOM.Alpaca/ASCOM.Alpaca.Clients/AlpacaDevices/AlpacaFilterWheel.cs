using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca FilterWheel client
    /// </summary>
    public class AlpacaFilterWheel : AlpacaDeviceBaseClass, IFilterWheelV2
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca FilterWheel device with all parameters set to default values
        /// </summary>
        public AlpacaFilterWheel()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca FilterWheel device specifying all parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="establishConnectionTimeout">Timeout (seconds) to initially connect to the Alpaca device</param>
        /// <param name="standardDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to complete quickly e.g. retrieving CanXXX properties</param>
        /// <param name="longDeviceResponseTimeout">Timeout (seconds) for transactions that are expected to take a long time to complete e.g. Camera.ImageArray</param>
        /// <param name="clientNumber">Arbitrary integer that represents this client. (Should be the same for all transactions from this client)</param>
        /// <param name="userName">Basic authentication user name for the Alpaca device</param>
        /// <param name="password">basic authentication password for the Alpaca device</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaFilterWheel(ServiceType serviceType,
                          string ipAddressString,
                          int portNumber,
                          int remoteDeviceNumber,
                          int establishConnectionTimeout,
                          int standardDeviceResponseTimeout,
                          int longDeviceResponseTimeout,
                          uint clientNumber,
                          string userName,
                          string password,
                          bool strictCasing,
                          ILogger logger
            )
        {
            this.serviceType = serviceType;
            this.ipAddressString = ipAddressString;
            this.portNumber = portNumber;
            this.remoteDeviceNumber = remoteDeviceNumber;
            this.establishConnectionTimeout = establishConnectionTimeout;
            this.standardDeviceResponseTimeout = standardDeviceResponseTimeout;
            this.longDeviceResponseTimeout = longDeviceResponseTimeout;
            this.clientNumber = clientNumber;
            this.userName = userName;
            this.password = password;
            this.strictCasing = strictCasing;
            this.logger = logger;

            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca FilterWheel device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be used to record operational information during execution</param>
        public AlpacaFilterWheel(ServiceType serviceType,
                         string ipAddressString,
                         int portNumber,
                         int remoteDeviceNumber,
                         bool strictCasing,
                         ILogger logger)
        {
            this.serviceType = serviceType;
            this.ipAddressString = ipAddressString;
            this.portNumber = portNumber;
            this.remoteDeviceNumber = remoteDeviceNumber;
            this.strictCasing = strictCasing;
            base.logger = logger;
            clientNumber = DynamicClientDriver.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = DeviceTypes.FilterWheel;

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Starting initialisation, Version: " + version.ToString());
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "This instance's unique client number: " + clientNumber);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "This devices's base URI: " + URIBase);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Establish communications timeout: " + establishConnectionTimeout.ToString());
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Standard device response timeout: " + standardDeviceResponseTimeout.ToString());
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Long device response timeout: " + longDeviceResponseTimeout.ToString());
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password length: {password.Length}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, standardDeviceResponseTimeout, userName, password, ImageArrayCompression.None, logger);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region IFilterWheel implementation

        /// <summary>
        /// Focus offset of each filter in the wheel
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// For each valid slot number (from 0 to N-1), reports the focus offset for the given filter position.  These values are focuser and filter dependent, and  would usually be set up by the user via 
        /// the SetupDialog. The number of slots N can be determined from the length of the array. If focuser offsets are not available, then it should report back 0 for all array values.
        /// </remarks>
        public int[] FocusOffsets
        {
            get
            {
                return DynamicClientDriver.GetValue<int[]>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "FocusOffsets", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Name of each filter in the wheel
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// For each valid slot number (from 0 to N-1), reports the name given to the filter position.  These names would usually be set up by the user via the
        /// SetupDialog.  The number of slots N can be determined from the length of the array.  If filter names are not available, then it should report back "Filter 1", "Filter 2", etc.
        /// </remarks>
        public string[] Names
        {
            get
            {
                return DynamicClientDriver.GetValue<string[]>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Names", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets or returns the current filter wheel position
        /// </summary>
        /// <exception cref="InvalidValueException">Must throw an InvalidValueException if an invalid position is set</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented, must not throw a NotImplementedException.</b></p>
        /// Write a position number between 0 and N-1, where N is the number of filter slots (see <see cref="Names"/>). Starts filter wheel rotation immediately when written. Reading
        /// the property gives current slot number (if wheel stationary) or -1 if wheel is moving. 
        /// <para>Returning a position of -1 is <b>mandatory</b> while the filter wheel is in motion; valid slot numbers must not be reported back while the filter wheel is rotating past filter positions.</para>
        /// <para><b>Note</b></para>
        /// <para>Some filter wheels are built into the camera (one driver, two interfaces).  Some cameras may not actually rotate the wheel until the exposure is triggered.  In this case, the written value is available
        /// immediately as the read value, and -1 is never produced.</para>
        /// </remarks>
        public short Position
        {
            get
            {
                return DynamicClientDriver.GetValue<short>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Position", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Position", value, MemberTypes.Property);
            }
        }

        #endregion
    }
}