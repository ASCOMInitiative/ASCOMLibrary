namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// The device's purpose. See the <see href="AlignerFAQ.htm">Aligner FAQ</see> for more information.
    /// </summary>
    public enum AlignerType
    {
        /// <summary>
        /// Default value allowing clients to confirm that the device has set a valid aligner type value. Clients should report an error if a device returns this value.
        /// </summary>
        Unset = 0,

        /// <summary>
        /// The device is a polar alignment aid
        /// </summary>
        PolarAligner = 1,

        /// <summary>
        /// The device is for aligning a telescope to the required optical axis
        /// </summary>
        OpticalAxisAligner = 2,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved3 = 3,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved4 = 4,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved5 = 5,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved6 = 6,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved7 = 7,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved8 = 8,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved9 = 9,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved10 = 10,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved11 = 11,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved12 = 12,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved13 = 13,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved14 = 14,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved15 = 15,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved16 = 16,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved17 = 17,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved18 = 18,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved19 = 19,

        /// <summary>
        /// Reserved for future use
        /// </summary>
        Reserved20 = 20
    }
}
