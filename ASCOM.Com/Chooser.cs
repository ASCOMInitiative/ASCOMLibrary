using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ASCOM.Com
{
    /// <summary>
    /// The Chooser provides a way for your application to let your user select which device to use.
    /// </summary>
    /// <remarks>
    /// This component is a light wrapper for the Platform Chooser COM component. In time it will be re-written as a native .NET Core component
    /// </remarks>
    public class Chooser : IDisposable
    {
        // COM ProgID of the Platform Chooser component
        private const string CHOOSER_PROGID = "ASCOM.Utilities.Chooser";

        private readonly dynamic chooser; // Holds the Chooser COM object reference
        private bool disposedValue; // Indicates whether the object has been Disposed

        #region New and Dispose

        /// <summary>
        /// Creates a new Chooser object
        /// </summary>
        public Chooser()
        {
            // Get the Chooser's Type from its ProgID
            Type chooserType = Type.GetTypeFromProgID(CHOOSER_PROGID);

            // Create a Chooser COM object and save the reference to the chooser variable
            chooser = Activator.CreateInstance(chooserType);
        }

        /// <summary>
        /// Chooser destructor, called by the runtime during garbage collection
        /// </summary>
        /// <remarks>This method ensures that Dispose is called during garbage collection even if it has not been called it from the application.</remarks>
        ~Chooser()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Dispose of this components Chooser object.
        /// </summary>
        /// <param name="disposing">True if called by the application, false if called by the garbage collector.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No objects to dispose here.
                }

                // Dispose of the Chooser COM object, ensuring that no exception is thrown
                try { Marshal.ReleaseComObject(chooser); } catch { }

                // Flag that Dispose has been called and the resources have been released
                disposedValue = true;
            }
        }

        /// <summary>
        /// Release the Chooser component's Chooser COM object
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put clean-up code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The type of device from which the Chooser will select a driver. (default = "Telescope") 
        /// </summary>
        public string DeviceType
        {
            get
            {
                CheckDisposed("DeviceType Get");
                return chooser.DeviceType;
            }
            set
            {
                CheckDisposed("DeviceType Set");
                chooser.DeviceType = value;
            }
        }

        /// <summary>
        /// Select the ASCOM driver to use without pre-selecting one in the drop-down list 
        /// </summary>
        /// <returns>The ProgID of the selected device or an empty string if no device was chosen</returns>
        public string Choose()
        {
            CheckDisposed("Choose(\"\")");
            return chooser.Choose("Telescope");
        }

        /// <summary>
        /// Display the Chooser dialogue enabling the user to select a driver
        /// </summary>
        /// <param name="progId">The driver ProgId to pre-select in the Chooser drop-down list</param>
        /// <returns>The ProgID of the selected device or an empty string if no device was chosen</returns>
        public string Choose(string progId)
        {
            CheckDisposed($"Choose(\"{progId})\"");
            return chooser.Choose(progId);
        }

        #endregion

        #region Support code
        /// <summary>
        /// Validate that this object has not been disposed. If this component has been disposed, throw an InvalidOperationException.
        /// </summary>
        /// <param name="method">Name of the called method</param>
        /// <exception cref="InvalidOperationException">When the Chooser has already been disposed.</exception>
        private void CheckDisposed(string method)
        {
            if(disposedValue)
            {
                throw new InvalidOperationException($"Cannot call Chooser.{method} because it has been disposed.");
            }
        }

        #endregion

    }
}
