namespace ASCOM.Com
{
    /// <summary>
    /// A class that represents the registration data for a driver that is stored in the ASCOM Register
    /// </summary>
    public class ASCOMRegistration
    {
        internal ASCOMRegistration(string progID, string name)
        {
            this.ProgID = progID;
            this.Name = name;
        }

        /// <summary>
        /// The ProgID of the driver used to create an instance of the COM Object
        /// </summary>
        public string ProgID
        {
            get;
            private set;
        }

        /// <summary>
        /// The Name of the driver. Often used to implement a chooser. This is a human readable value.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}
