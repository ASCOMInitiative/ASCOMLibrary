﻿using System;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ASCOM.Standard.COM.DriverAccess
{
    public class DynamicAccess : DynamicObject
    {
        private object device;

        internal object Device
        {
            get => device;
        }

        internal bool IsComObject
        {
            get;
            private set;
        }

        public DynamicAccess(string ProgID)
        {
            Type type = Type.GetTypeFromProgID(ProgID);
            if(type == null)
            {
                throw new Exception($"Failed to load ASCOM Driver with ProgID {ProgID}.");
            }
            device = Activator.CreateInstance(type);

            IsComObject = type.IsCOMObject;
        }

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
                    FakeLogger.LogMessageCrLf(binder.Name, "  Throwing MethodNotImplementedException: '" + binder.Name + "'");
                    throw new MethodNotImplementedException(binder.Name, ex.InnerException);
                }

                CheckDotNetExceptions(binder.Name, ex);

                throw;
            }
            catch
            {
                throw;
            }
        }

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
                    FakeLogger.LogMessageCrLf(binder.Name, "  Throwing PropertyNotImplementedException: '" + binder.Name + "'");
                    throw new PropertyNotImplementedException(binder.Name, false, ex.InnerException);
                }

                throw;
            }
            catch
            {
                throw;
            }
        }

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
                    FakeLogger.LogMessageCrLf(binder.Name, "  Throwing PropertyNotImplementedException: '" + binder.Name + "'");
                    throw new PropertyNotImplementedException(binder.Name, false, ex.InnerException);
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
            string message = string.Empty;

            int HResult = e.InnerException?.HResult ?? 0;

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
            if (HResult == ErrorCodes.InvalidOperationException)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing InvalidOperationException: '" + message + "'");
                throw new InvalidOperationException(message);
            }

            if (HResult == ErrorCodes.InvalidValue)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing InvalidValueException: '" + message + "'");
                throw new InvalidValueException(message);
            }

            if (HResult == ErrorCodes.NotConnected)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing NotConnectedException: '" + message + "'");
                throw new NotConnectedException(message);
            }

            if (HResult == ErrorCodes.NotImplemented)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing NotImplementedException: '" + message + "'");
                throw new NotImplementedException(message);
            }

            if (HResult == ErrorCodes.InvalidWhileParked)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing ParkedException: '" + message + "'");
                throw new ParkedException(message);
            }

            if (HResult == ErrorCodes.InvalidWhileSlaved)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing SlavedException: '" + message + "'");
                throw new SlavedException(message);
            }

            if (HResult == ErrorCodes.ValueNotSet)
            {
                message = e.InnerException.Message;
                FakeLogger.LogMessageCrLf(memberName, "  Throwing ValueNotSetException: '" + message + "'");
                throw new ValueNotSetException(message);
            }

            if (HResult >= ErrorCodes.DriverBase && HResult <= ErrorCodes.DriverMax)
            {
                message = e.InnerException.Message;

                FakeLogger.LogMessageCrLf(memberName, "  Throwing DriverException: '" + message + "'");
                throw new DriverException(message);
            }

            if (e.InnerException is COMException)
            {
                message = e.InnerException.Message;
                int number = (int)e.InnerException.GetType().InvokeMember("ErrorCode", BindingFlags.Default | BindingFlags.GetProperty, null, e.InnerException, new object[] { }, CultureInfo.InvariantCulture);

                FakeLogger.LogMessageCrLf(memberName, "  Throwing DriverAccessCOMException: '" + message + "' '" + number + "'");
                throw new DriverAccessCOMException(message, number, e.InnerException);
            }

            // Default behavior if its not one of the exceptions above
            string defaultmessage = "CheckDotNetExceptions " + "_strProgId" + " " + memberName + " " + e.InnerException.ToString() + " (See Inner Exception for details)";

            FakeLogger.LogMessageCrLf(memberName, "  Throwing Default DriverException: '" + defaultmessage + "'");
            throw new DriverException(defaultmessage, e.InnerException);
        }
    }

    internal class FakeLogger
    {
        internal static void LogMessageCrLf(string memberName, string v)
        {
        }
    }
}