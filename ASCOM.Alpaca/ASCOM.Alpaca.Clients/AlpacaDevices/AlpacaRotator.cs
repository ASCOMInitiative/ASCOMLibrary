using ASCOM.Common;
using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Rotator client
    /// </summary>
    public class AlpacaRotator : AlpacaDeviceBaseClass, IRotatorV4
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create a client for an Alpaca Rotator device with all parameters set to default values
        /// </summary>
        public AlpacaRotator()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Rotator device specifying all parameters
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
        public AlpacaRotator(ServiceType serviceType = AlpacaClient.CLIENT_SERVICETYPE_DEFAULT,
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
                             bool trustUserGeneratedSslCertificates = AlpacaClient.TRUST_USER_GENERATED_SSL_CERTIFICATES_DEFAULT
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
        /// Create a client for an Alpaca Rotator device specifying the minimum number of parameters
        /// </summary>
        /// <param name="serviceType">HTTP or HTTPS</param>
        /// <param name="ipAddressString">Alpaca device's IP Address</param>
        /// <param name="portNumber">Alpaca device's IP Port number</param>
        /// <param name="remoteDeviceNumber">Alpaca device's device number e.g. Telescope/0</param>
        /// <param name="strictCasing">Tolerate or throw exceptions  if the Alpaca device does not use strictly correct casing for JSON object element names.</param>
        /// <param name="logger">Optional ILogger instance that can be sued to record operational information during execution</param>
        public AlpacaRotator(ServiceType serviceType,
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
                clientDeviceType = DeviceTypes.Rotator;

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

                DynamicClientDriver.CreateHttpClient(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, userName, password, ImageArrayCompression.None,
                    logger, userAgentProductName, userAgentProductVersion, trustUserGeneratedSslCertificates);
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), "Completed initialisation");
            }
            catch (Exception ex)
            {
                LogMessage(logger, clientNumber, Devices.DeviceTypeToString(clientDeviceType), ex.ToString());
            }
        }

        #endregion

        #region IRotator Implementation

        /// <summary>
        /// Indicates whether the Rotator supports the <see cref="Reverse" /> method.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <returns>
        /// True if the Rotator supports the <see cref="Reverse" /> method.
        /// </returns>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented and must always return True for the IRotatorV3 interface or later.</b></p>
        /// </remarks>
        public bool CanReverse
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "CanReverse", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Indicates whether the rotator is currently moving
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <returns>True if the Rotator is moving to a new position. False if the Rotator is stationary.</returns>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// <para>During rotation, <see cref="IsMoving" /> must be True, otherwise it must be False.</para>
        /// <para><b>NOTE</b></para>
        /// <para>IRotatorV3, released in Platform 6.5, requires this method to be implemented, in previous interface versions implementation was optional.</para>
        /// </remarks>
        public bool IsMoving
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "IsMoving", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Current instantaneous Rotator position, allowing for any sync offset, in degrees.
        /// </summary>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// <p style="color:red"><b>SPECIFICATION REVISION - IRotatorV3 - Platform 6.5</b></p>
        /// <para>
        /// Position reports the synced position rather than the mechanical position. The synced position is defined 
        /// as the mechanical position plus an offset. The offset is determined when the <see cref="Sync(float)"/> method is called and must be persisted across driver starts and device reboots.
        /// </para>
        /// <para>
        /// The position is expressed as an angle from 0 up to but not including 360 degrees, counter-clockwise against the
        /// sky. This is the standard definition of Position Angle. However, the rotator does not need to (and in general will not)
        /// report the true Equatorial Position Angle, as the attached imager may not be precisely aligned with the rotator's indexing.
        /// It is up to the client to determine any offset between mechanical rotator position angle and the true Equatorial Position
        /// Angle of the imager, and compensate for any difference.
        /// </para>
        /// <para>
        /// The <see cref="Reverse" /> property is provided in order to manage rotators being used on optics with odd or
        /// even number of reflections. With the Reverse switch in the correct position for the optics, the reported position angle must
        /// be counter-clockwise against the sky.
        /// </para>
        /// <para><b>NOTE</b></para>
        /// <para>IRotatorV3, released in Platform 6.5, requires this method to be implemented, in previous interface versions implementation was optional.</para>
        /// </remarks>
        public float Position
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Position", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Sets or Returns the rotator’s Reverse state.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <value>True if the rotation and angular direction must be reversed for the optics</value>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// <para>See the definition of <see cref="Position" />.</para>
        /// <para><b>NOTE</b></para>
        /// <para>IRotatorV3, released in Platform 6.5, requires this method to be implemented, in previous interface versions implementation was optional.</para>
        /// </remarks>
        public bool Reverse
        {
            get
            {
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Reverse", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetBool(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "Reverse", value, MemberTypes.Property);
            }
        }

        /// <summary>
        /// The minimum StepSize, in degrees.
        /// </summary>
        /// <exception cref="NotImplementedException">Throw a NotImplementedException if the rotator does not know its step size.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Optional - can throw a not implemented exception</b></p>
        /// <para>Raises an exception if the rotator does not intrinsically know what the step size is.</para>
        /// </remarks>
        public float StepSize
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "StepSize", MemberTypes.Property);
            }
        }

        /// <summary>
        /// The destination position angle for Move() and MoveAbsolute().
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <value>The destination position angle for<see cref="Move">Move</see> and <see cref="MoveAbsolute">MoveAbsolute</see>.</value>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// <para>Upon calling <see cref="Move">Move</see> or <see cref="MoveAbsolute">MoveAbsolute</see>, this property immediately changes to the position angle to which the rotator is moving. 
        /// The value is retained until a subsequent call to <see cref="Move">Move</see> or <see cref="MoveAbsolute">MoveAbsolute</see>.</para>
        /// <para><b>NOTE</b></para>
        /// <para>IRotatorV3, released in Platform 6.5, requires this method to be implemented, in previous interface versions implementation was optional.</para>
        /// </remarks>
        public float TargetPosition
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "TargetPosition", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Immediately stop any Rotator motion due to a previous <see cref="Move">Move</see> or <see cref="MoveAbsolute">MoveAbsolute</see> method call.
        /// </summary>
        /// <exception cref="NotImplementedException">Throw a NotImplementedException if the rotator cannot halt.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks><p style="color:red"><b>Optional - can throw a not implemented exception</b></p> </remarks>
        public void Halt()
        {
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Halt", MemberTypes.Method);
            LogMessage(logger, clientNumber, "Halt", "Rotator halted OK");
        }

        /// <summary>
        /// Causes the rotator to move Position degrees relative to the current <see cref="Position" /> value.
        /// </summary>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <param name="Position">Relative position to move in degrees from current <see cref="Position" />.</param>
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// <para>Calling <see cref="Move">Move</see> causes the <see cref="TargetPosition" /> property to change to the sum of the current angular position
        /// and the value of the <see cref="Position" /> parameter (modulo 360 degrees), then starts rotation to <see cref="TargetPosition" />.</para>
        /// <para><b>NOTE</b></para>
        /// <para>IRotatorV3, released in Platform 6.5, requires this method to be implemented, in previous interface versions implementation was optional.</para>
        /// </remarks>
        public void Move(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Move", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "Move", $"Rotator moved to relative position {Position} OK");
        }

        /// <summary>
        /// Causes the rotator to move the absolute position of <see cref="Position" /> degrees.
        /// </summary>
        /// <param name="Position">Absolute position in degrees.</param>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// <p style="color:red"><b>SPECIFICATION REVISION - IRotatorV3 - Platform 6.5</b></p>
        /// <para>
        /// Calling <see cref="MoveAbsolute"/> causes the <see cref="TargetPosition" /> property to change to the value of the
        /// <see cref="Position" /> parameter, then starts rotation to <see cref="TargetPosition" />. 
        /// </para>
        /// <para><b>NOTE</b></para>
        /// <para>IRotatorV3, released in Platform 6.5, requires this method to be implemented, in previous interface versions implementation was optional.</para>
        /// </remarks>
        public void MoveAbsolute(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "MoveAbsolute", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "MoveAbsolute", $"Rotator moved to absolute position {Position} OK");
        }

        #endregion

        #region Convenience members

        /// <summary>
        /// Rotator device state
        /// </summary>
        public RotatorState RotatorState
        {
            get
            {
                // Create a state object to return.
                RotatorState rotatorState = new RotatorState(DeviceState, logger);
                logger.LogMessage(LogLevel.Debug, nameof(RotatorState), $"Returning: " +
                    $"Cloud cover: '{rotatorState.IsMoving}', " +
                    $"Dew point: '{rotatorState.MechanicalPosition}', " +
                    $"Humidity: '{rotatorState.Position}', " +
                    $"Time stamp: '{rotatorState.TimeStamp}'");

                // Return the device specific state class
                return rotatorState;
            }
        }

        #endregion

        #region IRotatorV3 Properties and Methods

        /// <summary>
        /// This returns the raw mechanical position of the rotator in degrees.
        /// </summary>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented.</b></p>
        /// <p style="color:red"><b>Introduced in IRotatorV3.</b></p>
        /// Returns the mechanical position of the rotator, which is equivalent to the IRotatorV2 <see cref="Position"/> property. Other clients (beyond the one that performed the sync) 
        /// can calculate the current offset using this and the <see cref="Position"/> value.
        /// </remarks>
        public float MechanicalPosition
        {
            get
            {
                return DynamicClientDriver.GetValue<float>(clientNumber, client, standardDeviceResponseTimeout, URIBase, strictCasing, logger, "MechanicalPosition", MemberTypes.Property);
            }
        }

        /// <summary>
        /// Syncs the rotator to the specified position angle without moving it. 
        /// </summary>
        /// <param name="Position">Synchronised rotator position angle.</param>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented.</b></p>
        /// <p style="color:red"><b>Introduced in IRotatorV3.</b></p>
        /// Once this method has been called and the sync offset determined, both the <see cref="MoveAbsolute(float)"/> method and the <see cref="Position"/> property must function in synced coordinates 
        /// rather than mechanical coordinates. The sync offset must persist across driver starts and device reboots.
        /// </remarks>
        public void Sync(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "Sync", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "Sync", $"Rotator synced to sky position {Position} OK");
        }

        /// <summary>
        /// Moves the rotator to the specified mechanical angle. 
        /// </summary>
        /// <param name="Position">Mechanical rotator position angle.</param>
        /// <exception cref="InvalidValueException">If Position is invalid.</exception>
        /// <exception cref="NotConnectedException">When <see cref="IAscomDevice.Connected"/> is False.</exception>
        /// <exception cref="DriverException">An error occurred that is not described by one of the more specific ASCOM exceptions. The device did not successfully complete the request.</exception> 
        /// <remarks>
        /// <p style="color:red"><b>Must be implemented.</b></p>
        /// <p style="color:red"><b>Introduced in IRotatorV3.</b></p>
        /// <para>Moves the rotator to the requested mechanical angle, independent of any sync offset that may have been set. This method is to address requirements that need a physical rotation
        /// angle such as taking sky flats.</para>
        /// <para>Client applications should use the <see cref="MoveAbsolute(float)"/> method in preference to this method when imaging.</para>
        /// </remarks>
        public void MoveMechanical(float Position)
        {
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.POSITION_PARAMETER_NAME, Position.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, longDeviceResponseTimeout, URIBase, strictCasing, logger, "MoveMechanical", Parameters, HttpMethod.Put, MemberTypes.Method);
            LogMessage(logger, clientNumber, "MoveMechanical", $"Rotator moved to mechanical position {Position} OK");
        }

        #endregion

        #region IRotatorV4 implementation

        // No new members

        #endregion

    }
}