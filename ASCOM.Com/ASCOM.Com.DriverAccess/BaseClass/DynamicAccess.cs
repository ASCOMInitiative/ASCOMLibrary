using System;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ASCOM.Com.DriverAccess
{
    /// <summary>
    /// Dynamically manipulate a COM object's members
    /// </summary>
    public class DynamicAccess : DynamicObject
    {
        private readonly object device;

        internal object Device
        {
            get => device;
        }

        internal bool IsComObject
        {
            get;
            private set;
        }

        /// <summary>
        /// Initialise the class, binding it to the specified ProgID.
        /// </summary>
        /// <param name="ProgID">ProgiId of the driver</param>
        /// <exception cref="Exception">If unable to load the specified driver</exception>
        public DynamicAccess(string ProgID)
        {
            Type type = Type.GetTypeFromProgID(ProgID);
            if (type == null)
            {
                throw new Exception($"Failed to load ASCOM Driver with ProgID {ProgID}.");
            }
            device = Activator.CreateInstance(type);

            IsComObject = type.IsCOMObject;
        }

        /// <summary>
        /// Call a COM object method.
        /// </summary>
        /// <param name="binder">COM object member reference</param>
        /// <param name="args">Method arguments</param>
        /// <param name="result">Method result.</param>
        /// <returns>True if the call is successful, otherwise false</returns>
        /// <exception cref="NotImplementedException">If the specified member is not implemented by the driver</exception>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                result = Unwrap().GetType().InvokeMember(
                    binder.Name,
                    BindingFlags.InvokeMethod,
                    Type.DefaultBinder,
                    Unwrap(),
                    args
                );
                return true;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException?.HResult == ErrorCodes.NotImplemented)
                {
                    FakeLogger.LogMessageCrLf(binder.Name, "  Throwing NotImplementedException: '" + binder.Name + "'");
                    throw new NotImplementedException(binder.Name, ex.InnerException);
                }

                CheckDotNetExceptions(binder.Name, ex);

                throw;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Set a COM object property value
        /// </summary>
        /// <param name="binder">COM object member reference</param>
        /// <param name="value">Method arguments</param>
        /// <returns>True if the call is successful, otherwise false</returns>
        /// <exception cref="NotImplementedException">If the specified member is not implemented by the driver</exception>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                Unwrap().GetType().InvokeMember(
                    binder.Name,
                    BindingFlags.SetProperty,
                    Type.DefaultBinder,
                    Unwrap(),
                    new object[] { value }
                 );
                return true;
            }
            catch (TargetInvocationException ex)
            {
                CheckDotNetExceptions(binder.Name, ex);

                if (ex.InnerException?.HResult == ErrorCodes.NotImplemented)
                {
                    FakeLogger.LogMessageCrLf(binder.Name, "  Throwing NotImplementedException: '" + binder.Name + "'");
                    throw new NotImplementedException(binder.Name, ex.InnerException);
                }

                throw;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get a COM object property value.
        /// </summary>
        /// <param name="binder">COM object member reference</param>
        /// <param name="result">Response from the member</param>
        /// <returns>True if the call is successful, otherwise false</returns>
        /// <exception cref="NotImplementedException">If the specified member is not implemented by the driver</exception>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = Unwrap().GetType().InvokeMember(
                    binder.Name,
                    BindingFlags.GetProperty,
                    Type.DefaultBinder,
                    Unwrap(),
                    new object[] { },
                    CultureInfo.InvariantCulture
                );
                return true;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException?.HResult == ErrorCodes.NotImplemented)
                {
                    FakeLogger.LogMessageCrLf(binder.Name, "  Throwing NotImplementedException: '" + binder.Name + "'");
                    throw new NotImplementedException(binder.Name, ex.InnerException);
                }

                CheckDotNetExceptions(binder.Name, ex);

                throw;
            }
            catch
            {
                throw;
            }
        }

        private object Unwrap() => Device ?? throw new ObjectDisposedException(nameof(Device));

        /// <summary>
        /// Checks for ASCOM exceptions returned as inner exceptions of TargetInvocationException. When new ASCOM exceptions are created
        /// they must be added to this method. They will then be used in all three cases of Property Get, Property Set and Method call.
        /// </summary>
        /// <param name="memberName">The name of the invoked member</param>
        /// <param name="e">The thrown TargetInvocationException including the inner exception</param>
        private void CheckDotNetExceptions(string memberName, Exception e)
        {
            int HResult = e.InnerException?.HResult ?? 0;

            string message;
            // Deal with the possibility that DriverAccess is being used in both driver and client so remove the outer
            // DriverAccessCOMException exception if present
            if (e.InnerException is DriverAccessCOMException)
            {
                message = e.InnerException.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  *** Found DriverAccessCOMException so stripping this off and reprocessing through CheckDotNetExceptions: '" + message + "'");
                FakeLogger.LogMessageCrLf(memberName, "  *** Inner exception is: " + e.InnerException.InnerException.GetType().Name);
                try // Try and print out the Inner.Inner exception
                {
                    FakeLogger.LogMessageCrLf(memberName, "  *** InnerException.InnerException is: " + e.InnerException.InnerException.InnerException.GetType().Name);
                }
                catch (Exception ex)
                {
                    // Report but ignore this error, catch it later in CheckDotNetExceptions
                    FakeLogger.LogMessageCrLf(memberName, "  *** Exception arose when accessing InnerException.InnerException: " + ex.ToString());
                }

                CheckDotNetExceptions(memberName + " inner exception", e.InnerException.InnerException);
            }

            //Throw the appropriate exception based on the inner exception of the TargetInvocationException
            if (HResult == ErrorCodes.ActionNotImplementedException)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing ActionNotImplementedException: '" + message + "'");
                throw new ActionNotImplementedException(message,e.InnerException);
            }

            if (HResult == ErrorCodes.InvalidOperationException)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing InvalidOperationException: '" + message + "'");
                throw new InvalidOperationException(message,e.InnerException);
            }

            if (HResult == ErrorCodes.InvalidValue)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing InvalidValueException: '" + message + "'");
                throw new InvalidValueException(message, e.InnerException);
            }

            if (HResult == ErrorCodes.NotConnected)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing NotConnectedException: '" + message + "'");
                throw new NotConnectedException(message, e.InnerException);
            }

            if (HResult == ErrorCodes.NotImplemented)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing NotImplementedException: '" + message + "'");
                throw new NotImplementedException(message, e.InnerException);
            }

            if (HResult == ErrorCodes.InvalidWhileParked)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing ParkedException: '" + message + "'");
                throw new ParkedException(message, e.InnerException);
            }

            if (HResult == ErrorCodes.InvalidWhileSlaved)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing SlavedException: '" + message + "'");
                throw new SlavedException(message, e.InnerException);
            }

            if (HResult == ErrorCodes.OperationCancelled)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing OperationCancelledException: '" + message + "'");
                throw new OperationCanceledException(message, e.InnerException);
            }

            if (HResult == ErrorCodes.ValueNotSet)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing ValueNotSetException: '" + message + "'");
                throw new ValueNotSetException(message, e.InnerException);
            }

            if (HResult >= ErrorCodes.DriverBase && HResult <= ErrorCodes.DriverMax)
            {
                message = e.InnerException.Message;

                FakeLogger.LogMessageCrLf(memberName, "  Throwing DriverException: '" + message + "'");
                throw new DriverException(message, e.InnerException);
            }

            if (e.InnerException is COMException)
            {
                message = e.InnerException.Message;
                int number = (int)e.InnerException.GetType().InvokeMember("ErrorCode", BindingFlags.Default | BindingFlags.GetProperty, null, e.InnerException, new object[] { }, CultureInfo.InvariantCulture);

                FakeLogger.LogMessageCrLf(memberName, "  Throwing DriverAccessCOMException: '" + message + "' '" + number + "'");
                throw new DriverAccessCOMException(message, number, e.InnerException);
            }

            // Default behavior if its not one of the exceptions above
            message = e.InnerException.Message;

            FakeLogger.LogMessageCrLf(memberName, "  Throwing Default DriverException: '" + message + "'");
            throw new DriverException(message, e.InnerException);
        }
    }

    internal class FakeLogger
    {
        internal static void LogMessageCrLf(string memberName, string v)
        {
            if (memberName == v) return; // Arbitrary statement just to use the supplied parameters and thereby avoid two compiler warnings about unused variables.
        }
    }
}