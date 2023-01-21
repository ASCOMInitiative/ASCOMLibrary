using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Focuser client
    /// </summary>
    public class AlpacaFocuser : AlpacaDeviceBaseClass, IFocuserV3
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Focuser device with all parameters set to default values
        /// </summary>
        public AlpacaFocuser()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Focuser device specifying all parameters
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
        /// <param name="userAgentProductName">Optional product name to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="userAgentProductVersion">Optional product version to include in the User-Agent HTTP header sent to the Alpaca device</param>
        /// <param name="trustUserGeneratedSslCertificates">Trust user generated SSL certificates</param>
        public AlpacaFocuser(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
                             string ipAddressString = AlpacaClient.CLIENT_IPADDRESS_DEFAULT,
                             int portNumber = AlpacaClient.CLIENT_IPPORT_DEFAULT,
                             int remoteDeviceNumber = AlpacaClient.CLIENT_REMOTEDEVICENUMBER_DEFAULT,
                             int establishConnectionTimeout = AlpacaClient.CLIENT_ESTABLISHCONNECTIONTIMEOUT_DEFAULT,
                             int standardDeviceResponseTimeout = AlpacaClient.CLIENT_STANDARDCONNECTIONTIMEOUT_DEFAULT,
                             int longDeviceResponseTimeout = AlpacaClient.CLIENT_LONGCONNECTIONTIMEOUT_DEFAULT,
                             uint clientNumber = AlpacaClient.CLIENT_CLIENTNUMBER_DEFAULT,
                             string userName = AlpacaClient.CLIENT_USERNAME_DEFAULT,
                             string password = AlpacaClient.CLIENT_PASSWORD_DEFAULT,
                             bool strictCasing = AlpacaClient.CLIENT_STRICTCASING_DEFAULT,
                             ILogger logger = AlpacaClient.CLIENT_LOGGER_DEFAULT,
                             string userAgentProductName = null,
                             string userAgentProductVersion = null,
                             bool trustUserGeneratedSslCertificates=AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT
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
            this.userAgentProductName = userAgentProductName;
            this.userAgentProductVersion = userAgentProductVersion;
            this.trustUserGeneratedSslCertificates = trustUserGeneratedSslCertificates;

            Initialise();
        }

        /// <summary>
        /// Create a client for an Alpaca Focuser device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaFocuser(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.Focuser;

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                // List parameter values
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Service type: {serviceType}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"IP address: {ipAddressString}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"IP port: {portNumber}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Remote device number: {remoteDeviceNumber}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Establish communications timeout: {establishConnectionTimeout}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Standard device response timeout: {standardDeviceResponseTimeout}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Long device response timeout: {longDeviceResponseTimeout}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Client number: {clientNumber}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"User name length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Password length: {password.Length}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Strict casing: {strictCasing}");
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), $"Trust user generated SSL certificates: {trustUserGeneratedSslCertificates}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, standardDeviceResponseTimeout, userName, password, ImageArrayCompression.None,
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region IFocuserV2 members

        /// <summary>
        /// True if the focuser is capable of absolute position; that is, being commanded to a specific step location.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p> </remarks>
        public bool Absolute
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Absolute", MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if the focuser is currently moving to a new position. False if the focuser is stationary.
        /// </summary>
        /// <exception cref="NotConnectedException">If the driver is not connected.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p></remarks>
        public bool IsMoving
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "IsMoving", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Maximum increment size allowed by the focuser; 
        /// i.e. the maximum number of steps allowed in one move operation.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// For most focusers this is the same as the <see cref="MaxStep" /> property. This is normally used to limit the Increment display in the host software.
        /// </remarks>
        public int MaxIncrement
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxIncrement", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Maximum step position permitted.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// The focuser can step between 0 and <see cref="MaxStep" />. If an attempt is made to move the focuser beyond these limits, it will automatically stop at the limit.
        /// </remarks>
        public int MaxStep
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MaxStep", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Current focuser position, in steps.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Can throw a not implemented exception</b></p> Valid only for absolute positioning focusers (see the <see cref="Absolute" /> property).
        /// A <see cref="NotImplementedException">NotImplementedException</see> exception must be thrown if this device is a relative positioning focuser rather than an absolute position focuser.
        /// </remarks>
        public int Position
        {
            get
            {
                return DynamicClientDriver.GetValue<int>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Position", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Step size (microns) for the focuser.
        /// </summary>
        /// <exception cref= "NotImplementedException">If the focuser does not intrinsically know what the step size is.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> Must throw an exception if the focuser does not intrinsically know what the step size is.</remarks>
        public double StepSize
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StepSize", MemberTypes.Property);
            }
        }

        /// <summary>
        /// The state of temperature compensation mode (if available), else always False.
        /// </summary>
        /// <exception cref="NotImplementedException">If <see cref="TempCompAvailable" /> is False and an attempt is made to set <see cref="TempComp" /> to true.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>TempComp Read must be implemented and must not throw a NotImplementedException. </b></p>
        /// <p style="color:red;margin-top:0"><b>TempComp Write can throw a NotImplementedException.</b></p>
        /// If the <see cref="TempCompAvailable" /> property is True, then setting <see cref="TempComp" /> to True puts the focuser into temperature tracking mode; setting it to False will turn off temperature tracking.
        /// <para>If temperature compensation is not available, this property must always return False.</para>
        /// <para> A <see cref="NotImplementedException" /> exception must be thrown if <see cref="TempCompAvailable" /> is False and an attempt is made to set <see cref="TempComp" /> to true.</para>
        /// <para><b>BEHAVIOURAL CHANGE - Platform 6.4</b></para>
        /// <para>Prior to Platform 6.4, the interface specification mandated that drivers must throw an <see cref="InvalidOperationException"/> if a move was attempted when <see cref="TempComp"/> was True, even if the focuser 
        /// was able to execute the move safely without disrupting temperature compensation.</para>
        /// <para>Following discussion on ASCOM-Talk in January 2018, the Focuser interface specification has been revised to IFocuserV3, removing the requirement to throw the InvalidOperationException exception. IFocuserV3 compliant drivers 
        /// are expected to execute Move requests when temperature compensation is active and to hide any specific actions required by the hardware from the client. For example this could be achieved by disabling temperature compensation, moving the focuser and re-enabling 
        /// temperature compensation or simply by moving the focuser with compensation enabled if the hardware supports this.</para>
        /// <para>Conform will continue to pass IFocuserV2 drivers that throw InvalidOperationException exceptions. However, Conform will now fail IFocuserV3 drivers that throw InvalidOperationException exceptions, in line with this revised specification.</para>
        /// </remarks>
        public bool TempComp
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TempComp", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TempComp", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// True if focuser has temperature compensation available.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented</b></p>
        /// Will be True only if the focuser's temperature compensation can be turned on and off via the <see cref="TempComp" /> property. 
        /// </remarks>
        public bool TempCompAvailable
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TempCompAvailable", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Current ambient temperature as measured by the focuser.
        /// </summary>
        /// <exception cref="NotImplementedException">If the property is not available for this device.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p> 
        /// Raises an exception if ambient temperature is not available. Commonly available on focusers with a built-in temperature compensation mode. 
        /// </remarks>
        public double Temperature
        {
            get
            {
                return DynamicClientDriver.GetValue<double>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Temperature", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Immediately stop any focuser motion due to a previous <see cref="Move" /> method call.
        /// </summary>
        /// <exception cref="NotImplementedException">Focuser does not support this method.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Can throw a not implemented exception</b></p>Some focusers may not support this function, in which case an exception will be raised. 
        /// <para><b>Recommendation:</b> Host software should call this method upon initialization and,
        /// if it fails, disable the Halt button in the user interface.</para>
        /// </remarks>
        public void Halt()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Halt", MemberTypes.Method);
            LogMessage(logger, clientNumber, "Halt", "Halted OK");
        }

        /// <summary>
        ///  Moves the focuser by the specified amount or to the specified position depending on the value of the <see cref="Absolute" /> property.
        /// </summary>
        /// <param name="Position">Step distance or absolute position, depending on the value of the <see cref="Absolute" /> property.</param>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Must be implemented</b></p>
        /// <para>If the <see cref="Absolute" /> property is True, then this is an absolute positioning focuser. The <see cref="Move">Move</see> command tells the focuser to move to an exact step position, and the Position parameter 
        /// of the <see cref="Move">Move</see> method is an integer between 0 and <see cref="MaxStep" />.</para>
        /// <para>If the <see cref="Absolute" /> property is False, then this is a relative positioning focuser. The <see cref="Move">Move</see> command tells the focuser to move in a relative direction, and the Position parameter 
        /// of the <see cref="Move">Move</see> method (in this case, step distance) is an integer between minus <see cref="MaxIncrement" /> and plus <see cref="MaxIncrement" />.</para>
        /// <para><b>BEHAVIOURAL CHANGE - Platform 6.4</b></para>
        /// <para>Prior to Platform 6.4, the interface specification mandated that drivers must throw an <see cref="InvalidOperationException"/> if a move was attempted when <see cref="TempComp"/> was True, even if the focuser 
        /// was able to execute the move safely without disrupting temperature compensation.</para>
        /// <para>Following discussion on ASCOM-Talk in January 2018, the Focuser interface specification has been revised to IFocuserV3, removing the requirement to throw the InvalidOperationException exception. IFocuserV3 compliant drivers 
        /// are expected to execute Move requests when temperature compensation is active and to hide any specific actions required by the hardware from the client. For example this could be achieved by disabling temperature compensation, moving the focuser and re-enabling 
        /// temperature compensation or simply by moving the focuser with compensation enabled if the hardware supports this.</para>
        /// <para>Conform will continue to pass IFocuserV2 drivers that throw InvalidOperationException exceptions. However, Conform will now fail IFocuserV3 drivers that throw InvalidOperationException exceptions, in line with this revised specification.</para>
        /// </remarks>
        public void Move(int Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Move", Parameters, HttpMethod.Put, MemberTypes.Method);
        }

        #endregion

    }
}
