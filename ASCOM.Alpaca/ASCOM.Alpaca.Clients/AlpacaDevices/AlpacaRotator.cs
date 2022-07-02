using ASCOM.Common.Alpaca;
using ASCOM.Common.DeviceInterfaces;
using ASCOM.Common.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// ASCOM Alpaca Rotator client
    /// </summary>
    public class AlpacaRotator : AlpacaDeviceBaseClass, IRotatorV3
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        /// <summary>
        /// Create an Alpaca Rotator device with all values set to default
        /// </summary>
        public AlpacaRotator()
        {
            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Rotator device specifying all parameters
        /// </summary>
        public AlpacaRotator(ServiceType serviceType,
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
                        ILogger TL
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
            this.TL = TL;

            Initialise();
        }

        /// <summary>
        /// Create an Alpaca Rotator device specifying the minimum required parameters, others will have default values
        /// </summary>
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
            TL = logger;
            clientNumber = DynamicClientDriver.GetUniqueClientNumber();
            Initialise();
        }
        private void Initialise()
        {
            try
            {
                // Set the device type
                clientDeviceType = "Rotator";

                URIBase = $"{AlpacaConstants.API_URL_BASE}{AlpacaConstants.API_VERSION_V1}/{clientDeviceType}/{remoteDeviceNumber}/";
                Version version = Assembly.GetEntryAssembly().GetName().Version;

                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Starting initialisation, Version: " + version.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "This instance's unique client number: " + clientNumber);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "This devices's base URI: " + URIBase);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Establish communications timeout: " + establishConnectionTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Standard device response timeout: " + standardDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Long device response timeout: " + longDeviceResponseTimeout.ToString());
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"User name is Null or Empty: {string.IsNullOrEmpty(userName)}, User name is Null or White Space: {string.IsNullOrWhiteSpace(userName)}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"User name length: {password.Length}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"Password is Null or Empty: {string.IsNullOrEmpty(password)}, Password is Null or White Space: {string.IsNullOrWhiteSpace(password)}");
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, $"Password length: {password.Length}");

                DynamicClientDriver.ConnectToRemoteDevice(ref client, serviceType, ipAddressString, portNumber, clientNumber, clientDeviceType, standardDeviceResponseTimeout, userName, password, TL);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, "Completed initialisation");
            }
            catch (Exception ex)
            {
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, clientDeviceType, ex.ToString());
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
        /// <p style="color:red;margin-bottom:0"><b>Must be implemented.</b></p>
        /// </remarks>
        public bool CanReverse
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanReverse", MemberTypes.Property);
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
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "IsMoving", MemberTypes.Property);
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
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<float>(clientNumber, client, URIBase, strictCasing, TL, "Position", MemberTypes.Property);
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
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "Reverse", MemberTypes.Property);
            }

            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetBool(clientNumber, client, URIBase, strictCasing, TL, "Reverse", value, MemberTypes.Property);
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
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<float>(clientNumber, client, URIBase, strictCasing, TL, "StepSize", MemberTypes.Property);
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
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<float>(clientNumber, client, URIBase, strictCasing, TL, "TargetPosition", MemberTypes.Property);
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
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "Halt", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Halt", "Rotator halted OK");
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
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "Move", Parameters, Method.PUT, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Move", $"Rotator moved to relative position {Position} OK");
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
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "MoveAbsolute", Parameters, Method.PUT, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "MoveAbsolute", $"Rotator moved to absolute position {Position} OK");
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
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<float>(clientNumber, client, URIBase, strictCasing, TL, "MechanicalPosition", MemberTypes.Property);
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
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "Sync", Parameters, Method.PUT, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Sync", $"Rotator synced to sky position {Position} OK");
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
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "MoveMechanical", Parameters, Method.PUT, MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "MoveMechanical", $"Rotator moved to mechanical position {Position} OK");
        }

        #endregion

    }
}