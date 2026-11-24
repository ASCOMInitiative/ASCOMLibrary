using ASCOM.Common.DeviceInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Alpaca.Clients
{
    /// <summary>
    /// Data transfer object class to hold the de-serialised Aligner.Configuration JSON object.
    /// </summary>
    /// <remarks>
    /// This class mirrors the properties of the <see cref="IAlignerConfigurationV1"/> interface except that all optional properties are implemented as nullable types.
    /// This enables us to detect when values are not supplied, because the property values will be null after de-serialisation. Later these values are copied to a class that 
    /// implements <see cref="IAlignerConfigurationV1"/> and properties with missing values will throw <see cref="PropertyNotImplementedException"/>s.
    /// </remarks>
    public class AlignerConfigurationV1Dto
    {
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
        public AlignerType AlignerType { get; set; } = AlignerType.Unset;

        /// <summary>
        /// True if the device compensates for gear backlash, otherwise false.
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool BacklashCompensation { get; set; } = false;

        /// <summary>
        /// True if the device can find home
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanFindHome { get; set; } = false;

        /// <summary>
        /// True of the device can move the X axis
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveX { get; set; } = false;

        /// <summary>
        /// True if the device can move its Y axis
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveY { get; set; } = false;

        /// <summary>
        /// True if the device can move its Z axis
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveZ { get; set; } = false;

        /// <summary>
        /// True if the device can move to an absolute position
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanMoveAbsolute { get; set; } = false;

        /// <summary>
        /// True if the device can synchronize its state
        /// </summary>
        /// <remarks>
        /// <para><b style="color:red">Must be implemented</b> Cannot throw a <see cref="PropertyNotImplementedException"/>.</para>
        /// </remarks>
        /// <revisionHistory visible="true">
        /// <revision visible="true" date="IAlignerV1" version="Platform 7.4">Member added.</revision>
        /// </revisionHistory>
        public bool CanSync { get; set; } = false;

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
        public AlignerUnits Units { get; set; } = AlignerUnits.Unset;

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
        public double? XMaxMove { get; set; } = null;

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
        public double? XMaxPosition { get; set; } = null;

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
        public double? XMinPosition { get; set; } = null;

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
        public double? XResolution { get; set; } = null;

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
        public double? YMaxMove { get; set; } = null;

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
        public double? YMaxPosition { get; set; } = null;

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
        public double? YMinPosition { get; set; } = null;

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
        public double? YResolution { get; set; } = null;

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
        public double? ZMaxMove { get; set; } = null;

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
        public double? ZMaxPosition { get; set; } = null;

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
        public double? ZMinPosition { get; set; } = null;

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
        public double? ZResolution { get; set; } = null;
    }
}
