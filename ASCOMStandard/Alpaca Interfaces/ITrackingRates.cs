using System;
using ASCOM.Alpaca.Interfaces;

/// <summary>
/// ''' Returns a collection of supported DriveRate values that describe the permissible values of the TrackingRate property for this telescope type.
/// ''' </summary>
public interface ITrackingRates 
{
    /// <summary>
    ///     ''' Returns a specified item from the collection
    ///     ''' </summary>
    ///     ''' <param name="index">Number of the item to return</param>
    ///     ''' <value>A collection of supported DriveRate values that describe the permissible values of the TrackingRate property for this telescope type.</value>
    ///     ''' <returns>Returns a collection of supported DriveRate values</returns>
    ///     ''' <remarks>This is only used by telescope interface versions 2 and 3</remarks>
    DriveRate this[int index] { get; }
    /// <summary>
    ///     ''' Number of DriveRates supported by the Telescope
    ///     ''' </summary>
    ///     ''' <value>Number of DriveRates supported by the Telescope</value>
    ///     ''' <returns>Integer count</returns>
    ///     ''' <remarks></remarks>
    int Count { get; }

    /// <summary>
    ///     ''' Returns an enumerator for the collection
    ///     ''' </summary>
    ///     ''' <returns>An enumerator</returns>
    ///     ''' <remarks></remarks>
    System.Collections.IEnumerator GetEnumerator();

    /// <summary>
    ///     ''' Disposes of the TrackingRates object
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    void Dispose();
}
