using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ASCOM.Alpaca.Tests
{
    internal static class Extensions
    {
        #region Private Extensions

        /// <summary>
        /// Convert a structure to a byte array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="structure"></param>
        /// <returns></returns>
        internal static byte[] ToByteArray<T>(this T structure) where T : struct
        {
            var bufferSize = Marshal.SizeOf(structure);
            var byteArray = new byte[bufferSize];

            IntPtr handle = Marshal.AllocHGlobal(bufferSize);
            try
            {
                Marshal.StructureToPtr(structure, handle, true);
                Marshal.Copy(handle, byteArray, 0, bufferSize);
            }
            finally
            {
                Marshal.FreeHGlobal(handle);
            }
            return byteArray;
        }

        /// <summary>
        /// Convert a byte array to a structure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        internal static T ToStructure<T>(this byte[] byteArray) where T : struct
        {
            var structure = new T();
            var bufferSize = Marshal.SizeOf(structure);
            IntPtr handle = Marshal.AllocHGlobal(bufferSize);
            try
            {
                Marshal.Copy(byteArray, 0, handle, bufferSize);
                structure = Marshal.PtrToStructure<T>(handle);

            }
            finally
            {
                Marshal.FreeHGlobal(handle);
            }

            return structure;
        }

        #endregion


    }
}
