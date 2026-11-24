using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Class presenting the IAlignerV1.Configuration members
    /// </summary>
    public class AlignerConfigurationV1 : IAlignerConfigurationV1
    {
        /// Backing fields for the optional properties. If the device does not implement a property, the backing field will be null and the property will throw a PropertyNotImplementedException
        private double? xMaxMove = null;
        private double? xMaxPosition = null;
        private double? xMinPosition = null;
        private double? xResolution = null;
        private double? yMaxMove = null;
        private double? yMaxPosition = null;
        private double? yMinPosition = null;
        private double? yResolution = null;
        private double? zMaxMove = null;
        private double? zMaxPosition = null;
        private double? zMinPosition = null;
        private double? zResolution = null;

        private string toStringValue = string.Empty;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AlignerConfigurationV1()
        {
            toStringValue = string.Join(", ", typeof(AlignerConfigurationV1).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(property => $"{property.Name} : {property.GetValue(this) ?? "Not implemented"}"));
        }

        /// <summary>
        /// Constructor that takes an Aligner Configuration DTO and populates this classes fields with its values.
        /// </summary>
        /// <param name="configuration">An AlignerConfigurationV1Dto class that has been created by de-serialising an Aligner device's Configuration response.</param>
        /// <exception cref="ArgumentNullException">If the supplied DTO class is null.</exception>
        internal AlignerConfigurationV1(AlignerConfigurationV1Dto configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Mandatory properties are copied directly
            AlignerType = configuration.AlignerType;
            BacklashCompensation = configuration.BacklashCompensation;
            CanFindHome = configuration.CanFindHome;
            CanMoveX = configuration.CanMoveX;
            CanMoveY = configuration.CanMoveY;
            CanMoveZ = configuration.CanMoveZ;
            CanMoveAbsolute = configuration.CanMoveAbsolute;
            CanSync = configuration.CanSync;
            Units = configuration.Units;

            // Optional properties are copied to the backing fields, which will throw a PropertyNotImplementedException if they are not set
            xMaxMove = configuration.XMaxMove;
            xMaxPosition = configuration.XMaxPosition;
            xMinPosition = configuration.XMinPosition;
            xResolution = configuration.XResolution;
            yMaxMove = configuration.YMaxMove;
            yMaxPosition = configuration.YMaxPosition;
            yMinPosition = configuration.YMinPosition;
            yResolution = configuration.YResolution;
            zMaxMove = configuration.ZMaxMove;
            zMaxPosition = configuration.ZMaxPosition;
            zMinPosition = configuration.ZMinPosition;
            zResolution = configuration.ZResolution;

            toStringValue = string.Join(", ", $"AlignerType: {AlignerType}", 
                $"BacklashCompensation: {BacklashCompensation}",
                $"CanFindHome: {CanFindHome}",
                $"CanMoveX: {CanMoveX}",
                $"CanMoveY: {CanMoveY}",
                $"CanMoveZ: {CanMoveZ}",
                $"CanMoveAbsolute: {CanMoveAbsolute}",
                $"CanSync: {CanSync}",
                $"Units: {Units}",
                $"XMaxMove: {(xMaxMove.HasValue ? xMaxMove.Value.ToString() : "Not implemented")}",
                $"XMaxPosition: {(xMaxPosition.HasValue ? xMaxPosition.Value.ToString() : "Not implemented")}",
                $"XMinPosition: {(xMinPosition.HasValue ? xMinPosition.Value.ToString() : "Not implemented")}",
                $"XResolution: {(xResolution.HasValue ? xResolution.Value.ToString() : "Not implemented")}",
                $"YMaxMove: {(yMaxMove.HasValue ? yMaxMove.Value.ToString() : "Not implemented")}",
                $"YMaxPosition: {(yMaxPosition.HasValue ? yMaxPosition.Value.ToString() : "Not implemented")}",
                $"YMinPosition: {(yMinPosition.HasValue ? yMinPosition.Value.ToString() : "Not implemented")}",
                $"YResolution: {(yResolution.HasValue ? yResolution.Value.ToString() : "Not implemented")}",
                $"ZMaxMove: {(zMaxMove.HasValue ? zMaxMove.Value.ToString() : "Not implemented")}",
                $"ZMaxPosition: {(zMaxPosition.HasValue ? zMaxPosition.Value.ToString() : "Not implemented")}",
                $"ZMinPosition: {(zMinPosition.HasValue ? zMinPosition.Value.ToString() : "Not implemented")}",
                $"ZResolution: {(zResolution.HasValue ? zResolution.Value.ToString() : "Not implemented")}"
                );
        }

        /// <summary>
        /// The intended use of this device
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for information on aligner types.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public AlignerType AlignerType { get; private set; } = AlignerType.Unset;

        /// <summary>
        /// True if the device compensates for gear backlash, otherwise false.
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool BacklashCompensation { get; private set; } = false;

        /// <summary>
        /// True if the device can find home
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanFindHome { get; private set; } = false;

        /// <summary>
        /// True of the device can move the X axis
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveX { get; private set; } = false;

        /// <summary>
        /// True if the device can move its Y axis
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveY { get; private set; } = false;

        /// <summary>
        /// True if the device can move its Z axis
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveZ { get; private set; } = false;

        /// <summary>
        /// True if the device can move to an absolute position
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveAbsolute { get; private set; } = false;

        /// <summary>
        /// True if the device can synchronize its state
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanSync { get; private set; } = false;

        /// <summary>
        /// The device's unit of measurement 1 = Degrees , 2 = Millimeters, 3 = Counts.   
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for information on the units for different aligner types.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public AlignerUnits Units { get; private set; } = AlignerUnits.Unset;

        /// <summary>
        /// Largest relative movement of which the X axis is capable in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the maximum relative X movement.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double XMaxMove
        {
            get
            {
                if (xMaxMove.HasValue)
                {
                    return xMaxMove.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("XMaxMove", false);
                }
            }

            private set
            {
                xMaxMove = value;
            }
        }

        /// <summary>
        /// The maximum reachable position of the X axis in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the maximum possible X position.</exception>
        /// <remarks>
        /// <para>When XMaxPosition is implemented, XMinPosition must also be implemented, Likewise, when XMaxPosition is not implemented XMinPosition must also not be implemented.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double XMaxPosition
        {
            get
            {
                if (xMaxPosition.HasValue)
                {
                    return xMaxPosition.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("XMaxPosition", false);
                }
            }

            private set
            {
                xMaxPosition = value;
            }
        }

        /// <summary>
        /// The minimum reachable position of the X axis in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the minimum possible X position.</exception>
        /// <remarks>
        /// <para>When XMinPosition is implemented, XMaxPosition must also be implemented, Likewise, when XMinPosition is not implemented XMaxPosition must also not be implemented.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double XMinPosition
        {
            get
            {
                if (xMinPosition.HasValue)
                {
                    return xMinPosition.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("XMinPosition", false);
                }
            }

            private set
            {
                xMinPosition = value;
            }
        }

        /// <summary>
        ///  The smallest move the X axis can make in <see cref="AlignerUnits"/>.
        /// </summary>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the smallest X axis movement.</exception>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double XResolution
        {
            get
            {
                if (xResolution.HasValue)
                {
                    return xResolution.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("XResolution", false);
                }
            }

            private set
            {
                xResolution = value;
            }
        }

        /// <summary>
        /// Largest relative movement of which the Y axis is capable in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the maximum relative Y movement.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double YMaxMove
        {
            get
            {
                if (yMaxMove.HasValue)
                {
                    return yMaxMove.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("YMaxMove", false);
                }
            }

            private set
            {
                yMaxMove = value;
            }
        }

        /// <summary>
        /// The maximum reachable position of the Y axis in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the maximum possible Y position.</exception>
        /// <remarks>
        /// <para>When YMaxPosition is implemented, YMinPosition must also be implemented, Likewise, when YMaxPosition is not implemented YMinPosition must also not be implemented.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double YMaxPosition
        {
            get
            {
                if (yMaxPosition.HasValue)
                {
                    return yMaxPosition.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("YMaxPosition", false);
                }
            }

            private set
            {
                yMaxPosition = value;
            }
        }

        /// <summary>
        /// The minimum reachable position of the Y axis in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the minimum possible Y position.</exception>
        /// <remarks>
        /// <para>When YMinPosition is implemented, YMaxPosition must also be implemented, Likewise, when YMinPosition is not implemented YMaxPosition must also not be implemented.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double YMinPosition
        {
            get
            {
                if (yMinPosition.HasValue)
                {
                    return yMinPosition.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("YMinPosition", false);
                }
            }

            private set
            {
                yMinPosition = value;
            }
        }

        /// <summary>
        ///  The smallest move the Y axis can make in <see cref="AlignerUnits"/>.
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the smallest Y axis movement.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double YResolution
        {
            get
            {
                if (yResolution.HasValue)
                {
                    return yResolution.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("YResolution", false);
                }
            }

            private set
            {
                yResolution = value;
            }
        }

        /// <summary>
        /// Largest relative movement of which the Z axis is capable in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the maximum relative Z movement.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double ZMaxMove
        {
            get
            {
                if (zMaxMove.HasValue)
                {
                    return zMaxMove.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("ZMaxMove", false);
                }
            }

            private set
            {
                zMaxMove = value;
            }
        }

        /// <summary>
        /// The maximum reachable position of the Z axis in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the maximum possible Z position.</exception>
        /// <remarks>
        /// <para>When ZMaxPosition is implemented, ZMinPosition must also be implemented, Likewise, when ZMaxPosition is not implemented ZMinPosition must also not be implemented.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double ZMaxPosition
        {
            get
            {
                if (zMaxPosition.HasValue)
                {
                    return zMaxPosition.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("ZMaxPosition", false);
                }
            }

            private set
            {
                zMaxPosition = value;
            }
        }

        /// <summary>
        /// The minimum reachable position of the Z axis in <see cref="AlignerUnits"/>. 
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the minimum possible Z position.</exception>
        /// <remarks>
        /// <para>When ZMinPosition is implemented, ZMaxPosition must also be implemented, Likewise, when ZMinPosition is not implemented ZMaxPosition must also not be implemented.</para>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double ZMinPosition
        {
            get
            {
                if (zMinPosition.HasValue)
                {
                    return zMinPosition.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("ZMinPosition", false);
                }
            }

            private set
            {
                zMinPosition = value;
            }
        }

        /// <summary>
        ///  The smallest move the Z axis can make in <see cref="AlignerUnits"/>.
        /// </summary>
        /// <exception cref="PropertyNotImplementedException">When the device does not know the smallest Z axis movement.</exception>
        /// <remarks>
        /// <para>See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for units and Alpaca / COM protocol specific implementation requirements.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public double ZResolution
        {
            get
            {
                if (zResolution.HasValue)
                {
                    return zResolution.Value;
                }
                else
                {
                    throw new PropertyNotImplementedException("ZResolution", false);
                }
            }

            private set
            {
                zResolution = value;
            }
        }

        /// <summary>
        /// Return the value as a string
        /// </summary>
        /// <returns>String representation of the response value</returns>
        public override string ToString()
        {
            return toStringValue;
        }

    }
}
