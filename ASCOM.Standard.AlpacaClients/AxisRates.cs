using System;
using System.Collections;
using System.Runtime.InteropServices;
using ASCOM.Standard.Interfaces;

namespace ASCOM.Standard.AlpacaClients
{
    //
    // AxisRates is a strongly-typed collection that must be enumerable by
    // both COM and .NET. The IAxisRates and IEnumerable interfaces provide
    // this polymorphism. 
    //
    // The Guid attribute sets the CLSID for ADynamicRemoteClients AxisRates class
    // The ClassInterface/None attribute prevents an empty interface called
    // _AxisRates from being created and used as the [default] interface
    //
    internal class AxisRates : IAxisRates, IEnumerable, IEnumerator, IDisposable
    {
        private TelescopeAxis m_axis;
        private Rate[] m_Rates;
        private int pos;
        ILogger logger;

        //
        // Constructor - Internal prevents public creation
        // of instances. Returned by Telescope.AxisRates.
        //
        internal AxisRates(TelescopeAxis Axis, ILogger TL)
        {
            m_axis = Axis;
            logger = TL;
            AlpacaDeviceBaseClass.LogMessage(TL, 0, "AlpacaClients.AxisRates Init", $"Supplied axis: {Axis}");

            //
            // This collection must hold zero or more Rate objects describing the 
            // rates of motion ranges for the Telescope.MoveAxis() method
            // that are supported by your driver. It is OK to leave this 
            // array empty, indicating that MoveAxis() is not supported.
            //
            // Note that we are constructing a rate array for the axis passed
            // to the constructor. Thus we switch() below, and each case should 
            // initialize the array for the rate for the selected axis.
            //
            switch (m_axis)
            {
                case TelescopeAxis.Primary:
                case TelescopeAxis.Secondary:
                case TelescopeAxis.Tertiary:
                    m_Rates = new Rate[] { };
                    break;
            }
            pos = -1;
            AlpacaDeviceBaseClass.LogMessage(TL, 0, "AlpacaClients.AxisRates Init", $"Number of Rates: {m_Rates.Length}");
        }

        internal void Add(double Minium, double Maximum, ILogger TL)
        {
            AlpacaDeviceBaseClass.LogMessage(TL, 0, "AlpacaClients.AxisRates.Add", "Before m_Rates.Length: " + m_Rates.Length);
            Rate r = new Rate(Minium, Maximum); // Create a new rate to add to the new array
            Rate[] NewRateArray = new Rate[m_Rates.Length + 1]; // Create a new Rate array to replace the current one
            AlpacaDeviceBaseClass.LogMessage(TL, 0, "AlpacaClients.AxisRates.Add", "NewRateArray.Length: " + NewRateArray.Length);
            Array.Copy(m_Rates, NewRateArray, m_Rates.Length); // Copy the current contents of the m_Rated array to the new array
            NewRateArray[m_Rates.Length] = r; // Add the new rate the new Rates array.
            m_Rates = NewRateArray; // Make m_Rates point at the new larger array
            AlpacaDeviceBaseClass.LogMessage(TL, 0, "AlpacaClients.AxisRates.Add", "After m_Rates.Length: " + m_Rates.Length);
        }
        #region IAxisRates Members

        public int Count
        {
            get
            {
                AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.Count", $"Returning Count: {m_Rates.Length}");
                return m_Rates.Length;
            }
        }

        public IEnumerator GetEnumerator()
        {
            AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.GetEnumerator", $"Returning Enumerator");
            pos = -1; //Reset pointer as this is assumed by .NET enumeration
            return this as IEnumerator;
        }

        public IRate this[int index]
        {
            get
            {
                AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.This[index]", $"m_rates is null: {m_Rates is null}");
                if (m_Rates != null) AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.This[index]", $"Returning item at index: {index}");

                if (index < 1 || index > this.Count) throw new InvalidValueException("AxisRates.Index", index.ToString(), $"1 to {this.Count}");
                return m_Rates[index - 1]; 	// 1-based
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.Dispose(bool)", $"SETTING m_Rates to NULL!!!");
                // free managed resources
                m_Rates = null;
            }
        }

        #endregion

        #region IEnumerator implementation

        public bool MoveNext()
        {
            AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.MoveNext", $"Moving to next element");
            if (++pos >= m_Rates.Length) return false;
            return true;
        }

        public void Reset()
        {
            AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.Reset", $"Resetting index");
            pos = -1;
        }

        public object Current
        {
            get
            {
                if (pos < 0 || pos >= m_Rates.Length) throw new System.InvalidOperationException($"ASCOM DynamicRemoteClient AxisRates.Current - Pointer value {pos} is outside expected range 0..{m_Rates.Length}");
                AlpacaDeviceBaseClass.LogMessage(logger, 0, "AlpacaClients.AxisRates.Current", $"Returning current entry");
                return m_Rates[pos];
            }
        }

        #endregion
    }
}
