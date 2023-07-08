using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Exception thrown when an attempt is made to read a Kepler value that has not yet been calculated.
    /// </summary>
    /// <remarks>This probably occurs because another variable has not been set or a required method has not been called.</remarks>
    public class ValueNotAvailableException : HelperException
    {

        /// <summary>
        ///         ''' Create a new exception with message 
        ///         ''' </summary>
        ///         ''' <param name="message">Message to be reported by the exception</param>
        ///         ''' <remarks></remarks>
        public ValueNotAvailableException(string message) : base(message)
        {
        }

        /// <summary>
        ///         ''' Create a new exception with message 
        ///         ''' </summary>
        ///         ''' <param name="message">Message to be reported by the exception</param>
        ///         ''' <param name="inner">Exception to be reported as the inner exception</param>
        ///         ''' <remarks></remarks>
        public ValueNotAvailableException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///         ''' Serialise the exception
        ///         ''' </summary>
        ///         ''' <param name="info">Serialisation information</param>
        ///         ''' <param name="context">Serialisation context</param>
        ///         ''' <remarks></remarks>
        public ValueNotAvailableException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }

}
