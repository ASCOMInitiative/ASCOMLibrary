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
    /// ASCOM Alpaca Telescope client
    /// </summary>
    public class AlpacaTelescope : AlpacaDeviceBaseClass, ITelescopeV3
    {
        #region Variables and Constants

        #endregion

        #region Initialiser

        public AlpacaTelescope()
        {
            Initialise();
        }

        public AlpacaTelescope(ServiceType serviceType,
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

        public AlpacaTelescope(ServiceType serviceType,
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
                clientDeviceType = "Telescope";

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

        #region ITelescope Implementation
        public void AbortSlew()
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "AbortSlew", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "AbortSlew", "Slew aborted OK");
        }

        public AlignmentMode AlignmentMode
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<AlignmentMode>(clientNumber, client, URIBase, strictCasing, TL, "AlignmentMode", MemberTypes.Property);
            }
        }

        public double Altitude
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "Altitude", MemberTypes.Property);
            }
        }

        public double ApertureArea
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "ApertureArea", MemberTypes.Property);
            }
        }

        public double ApertureDiameter
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "ApertureDiameter", MemberTypes.Property);
            }
        }

        public bool AtHome
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "AtHome", MemberTypes.Property);
            }
        }

        public bool AtPark
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "AtPark", MemberTypes.Property);
            }
        }

        public IAxisRates AxisRates(TelescopeAxis Axis)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) }
            };
            return DynamicClientDriver.SendToRemoteDevice<IAxisRates>(clientNumber, client, URIBase, strictCasing, TL, "AxisRates", Parameters, Method.GET, MemberTypes.Method);
        }

        public double Azimuth
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "Azimuth", MemberTypes.Property);
            }
        }

        public bool CanFindHome
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanFindHome", MemberTypes.Property);
            }
        }

        public bool CanMoveAxis(TelescopeAxis Axis)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) }
            };
            return DynamicClientDriver.SendToRemoteDevice<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanMoveAxis", Parameters, Method.GET, MemberTypes.Method);
        }

        public bool CanPark
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanPark", MemberTypes.Property);
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanPulseGuide", MemberTypes.Property);
            }
        }

        public bool CanSetDeclinationRate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetDeclinationRate", MemberTypes.Property);
            }
        }

        public bool CanSetGuideRates
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetGuideRates", MemberTypes.Property);
            }
        }

        public bool CanSetPark
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetPark", MemberTypes.Property);
            }
        }

        public bool CanSetPointingState
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetPointingState", MemberTypes.Property);
            }
        }

        public bool CanSetPierSide
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetPierSide", MemberTypes.Property);
            }
        }

        public bool CanSetRightAscensionRate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetRightAscensionRate", MemberTypes.Property);
            }
        }

        public bool CanSetTracking
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSetTracking", MemberTypes.Property);
            }
        }

        public bool CanSlew
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSlew", MemberTypes.Property);
            }
        }

        public bool CanSlewAltAz
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSlewAltAz", MemberTypes.Property);
            }
        }

        public bool CanSlewAltAzAsync
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSlewAltAzAsync", MemberTypes.Property);
            }
        }

        public bool CanSlewAsync
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSlewAsync", MemberTypes.Property);
            }
        }

        public bool CanSync
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSync", MemberTypes.Property);
            }
        }

        public bool CanSyncAltAz
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanSyncAltAz", MemberTypes.Property);
            }
        }

        public bool CanUnpark
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "CanUnpark", MemberTypes.Property);
            }
        }

        public double Declination
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "Declination", MemberTypes.Property);
            }
        }

        public double DeclinationRate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "DeclinationRate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "DeclinationRate", value, MemberTypes.Property);
            }
        }

        public PointingState DestinationSideOfPier(double RightAscension, double Declination)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            return DynamicClientDriver.SendToRemoteDevice<PointingState>(clientNumber, client, URIBase, strictCasing, TL, "DestinationSideOfPier", Parameters, Method.GET, MemberTypes.Method);
        }

        public bool DoesRefraction
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "DoesRefraction", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetBool(clientNumber, client, URIBase, strictCasing, TL, "DoesRefraction", value, MemberTypes.Property);
            }
        }

        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<EquatorialCoordinateType>(clientNumber, client, URIBase, strictCasing, TL, "EquatorialSystem", MemberTypes.Property);
            }
        }

        public void FindHome()
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "FindHome", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "FindHome", "Home found OK");
        }

        public double FocalLength
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "FocalLength", MemberTypes.Property);
            }
        }

        public double GuideRateDeclination
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "GuideRateDeclination", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "GuideRateDeclination", value, MemberTypes.Property);
            }
        }

        public double GuideRateRightAscension
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "GuideRateRightAscension", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "GuideRateRightAscension", value, MemberTypes.Property);
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "IsPulseGuiding", MemberTypes.Property);
            }
        }

        public void MoveAxis(TelescopeAxis Axis, double Rate)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AXIS_PARAMETER_NAME, ((int)Axis).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.RATE_PARAMETER_NAME, Rate.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "MoveAxis", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void Park()
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "Park", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "Park", "Parked OK");
        }

        public void PulseGuide(GuideDirection Direction, int Duration)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.DIRECTION_PARAMETER_NAME, ((int)Direction).ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DURATION_PARAMETER_NAME, Duration.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "PulseGuide", Parameters, Method.PUT, MemberTypes.Method);
        }

        public double RightAscension
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "RightAscension", MemberTypes.Property);
            }
        }

        public double RightAscensionRate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "RightAscensionRate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "RightAscensionRate", value, MemberTypes.Property);
            }
        }

        public void SetPark()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "SetPark", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "SetPark", "Park set OK");
        }

        public PointingState SideOfPier
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<PointingState>(clientNumber, client, URIBase, strictCasing, TL, "SideOfPier", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
                Dictionary<string, string> Parameters = new Dictionary<string, string>
                {
                    { AlpacaConstants.SIDEOFPIER_PARAMETER_NAME, ((int)value).ToString(CultureInfo.InvariantCulture) }
                };
                DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SideOfPier", Parameters, Method.PUT, MemberTypes.Property);
            }
        }

        public double SiderealTime
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SiderealTime", MemberTypes.Property);
            }
        }

        public double SiteElevation
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SiteElevation", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "SiteElevation", value, MemberTypes.Property);
            }
        }

        public double SiteLatitude
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SiteLatitude", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "SiteLatitude", value, MemberTypes.Property);
            }
        }

        public double SiteLongitude
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "SiteLongitude", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "SiteLongitude", value, MemberTypes.Property);
            }
        }

        public short SlewSettleTime
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<short>(clientNumber, client, URIBase, strictCasing, TL, "SlewSettleTime", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetShort(clientNumber, client, URIBase, strictCasing, TL, "SlewSettleTime", value, MemberTypes.Property);
            }
        }

        public void SlewToAltAz(double Azimuth, double Altitude)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SlewToAltAz", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void SlewToAltAzAsync(double Azimuth, double Altitude)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SlewToAltAzAsync", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void SlewToCoordinates(double RightAscension, double Declination)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SlewToCoordinates", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void SlewToCoordinatesAsync(double RightAscension, double Declination)
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SlewToCoordinatesAsync", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void SlewToTarget()
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "SlewToTarget", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "SlewToTarget", "Slew completed OK");
        }

        public void SlewToTargetAsync()
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "SlewToTargetAsync", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "SlewToTargetAsync", "Slew completed OK");
        }

        public bool Slewing
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "Slewing", MemberTypes.Property);
            }
        }

        public void SyncToAltAz(double Azimuth, double Altitude)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.AZ_PARAMETER_NAME, Azimuth.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.ALT_PARAMETER_NAME, Altitude.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SyncToAltAz", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void SyncToCoordinates(double RightAscension, double Declination)
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            Dictionary<string, string> Parameters = new Dictionary<string, string>
            {
                { AlpacaConstants.RA_PARAMETER_NAME, RightAscension.ToString(CultureInfo.InvariantCulture) },
                { AlpacaConstants.DEC_PARAMETER_NAME, Declination.ToString(CultureInfo.InvariantCulture) }
            };
            DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "SyncToCoordinates", Parameters, Method.PUT, MemberTypes.Method);
        }

        public void SyncToTarget()
        {
            DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "SyncToTarget", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "SyncToTarget", "Slew completed OK");
        }

        public double TargetDeclination
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "TargetDeclination", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "TargetDeclination", value, MemberTypes.Property);
            }
        }

        public double TargetRightAscension
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<double>(clientNumber, client, URIBase, strictCasing, TL, "TargetRightAscension", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetDouble(clientNumber, client, URIBase, strictCasing, TL, "TargetRightAscension", value, MemberTypes.Property);
            }
        }

        public bool Tracking
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<bool>(clientNumber, client, URIBase, strictCasing, TL, "Tracking", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetBool(clientNumber, client, URIBase, strictCasing, TL, "Tracking", value, MemberTypes.Property);
            }
        }

        public DriveRate TrackingRate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<DriveRate>(clientNumber, client, URIBase, strictCasing, TL, "TrackingRate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                DynamicClientDriver.SetInt(clientNumber, client, URIBase, strictCasing, TL, "TrackingRate", (int)value, MemberTypes.Property);
            }
        }

        public ITrackingRates TrackingRates
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<ITrackingRates>(clientNumber, client, URIBase, strictCasing, TL, "TrackingRates", MemberTypes.Property);
            }
        }

        public DateTime UTCDate
        {
            get
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                return DynamicClientDriver.GetValue<DateTime>(clientNumber, client, URIBase, strictCasing, TL, "UTCDate", MemberTypes.Property);
            }
            set
            {
                DynamicClientDriver.SetClientTimeout(client, standardDeviceResponseTimeout);
                Dictionary<string, string> Parameters = new Dictionary<string, string>();
                string utcDateString = value.ToString(AlpacaConstants.ISO8601_DATE_FORMAT_STRING) + "Z";
                Parameters.Add(AlpacaConstants.UTCDATE_PARAMETER_NAME, utcDateString);
                AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "UTCDate", "Sending date string: " + utcDateString);
                DynamicClientDriver.SendToRemoteDevice<NoReturnValue>(clientNumber, client, URIBase, strictCasing, TL, "UTCDate", Parameters, Method.PUT, MemberTypes.Property);
            }
        }

        public void Unpark()
        {
            DynamicClientDriver.SetClientTimeout(client, longDeviceResponseTimeout);
            DynamicClientDriver.CallMethodWithNoParameters(clientNumber, client, URIBase, strictCasing, TL, "UnPark", MemberTypes.Method);
            AlpacaDeviceBaseClass.LogMessage(TL, clientNumber, "UnPark", "Unparked OK");
        }

        #endregion

    }
}
