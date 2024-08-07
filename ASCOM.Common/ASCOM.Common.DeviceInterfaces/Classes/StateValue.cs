﻿using ASCOM.Common.DeviceStateClasses;
using ASCOM.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ASCOM.Common.DeviceInterfaces
{
    /// <summary>
    /// Class to hold a state name:value pair.
    /// </summary>
    public class StateValue : IStateValue
    {
        /// <summary>
        /// Create a new state value object
        /// </summary>
        public StateValue() { }

        /// <summary>
        /// Create a new state value object with the given name and value
        /// </summary>
        /// <param name="name">State name</param>
        /// <param name="value">State value</param>
        public StateValue(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Create a StateValue object whose Name property is "TimeStamp" and whose Value property is the supplied date-time value.
        /// </summary>
        /// <param name="dateTime">This time-stamp date-time value</param>
        public StateValue(DateTime dateTime)
        {
            Name = "TimeStamp";
            Value = dateTime;
        }

        /// <summary>
        /// State name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// State value
        /// </summary>
        public object Value { get; set; }

    }
}
