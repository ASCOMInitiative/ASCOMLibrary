namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// The units of movement for the device's axes. See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for more information.
    /// </summary>
    public enum AlignerUnits
    {
        /// <summary>
        /// Default value allowing clients to confirm that the device has set a valid units value. Clients should report an error if a device returns this value.
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Movement is measured in degrees
        /// </summary>
        Degrees = 1,

        /// <summary>
        /// Movement is measured in millimetres
        /// </summary>
        Millimeters = 2,

        /// <summary>
        /// Movement is measured in arbitrary position counts
        /// </summary>
        /// <remarks>Select this unit if the device is uncalibrated and responds to motor step counts or encoder position values.</remarks>
        Counts = 3,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved4 = 4,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved5 = 5,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved6 = 6,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved7 = 7,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved8 = 8,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved9 = 9,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved10 = 10,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved11 = 11,
        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved12 = 12,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved13 = 13,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved14 = 14,
        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved15 = 15,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved16 = 16,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved17 = 17,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved18 = 18,
        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved19 = 19,

        /// <summary>
        /// Reserved for future expansion
        /// </summary>
        Reserved20 = 20
    }
}
