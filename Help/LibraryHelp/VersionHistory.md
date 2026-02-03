---
uid: cf197185-7fa2-4b56-bb1a-75aaebd459bb
title: Version History
tocTitle: Version History
# linkText: Optional Text to Use For Links
# keywords: keyword, term 1, term 2, "term, with comma"
# alt-uid: optional-alternate-id
# summary: Optional summary abstract
---

## Version 3.0.0
### Changes for all components in this release
* ADDED - Support for .NET 8.0, 9.0 and 10.0 target frameworks. It will no longer be necessary for applications that use these frameworks to 
rely on the .NET Standard 2.0 component.

### Alpaca Client changes in this release
* POTENTIALLY BREAKING CHANGE - The client's [`100-CONTINUE`](https://dev.to/mrcaidev/everything-you-need-to-know-about-100-continue-3mn5)
behaviour is no longer enabled by default to improve network performance by removing a network round-trip on each `PUT` request.
  * This change is expected to be transparent for clients and devices because Alpaca devices should already be capable of handling requests from other clients 
that do not use the `EXPECT 100-CONTINUE` protocol. Out of an abundance of caution, the change is marked as potentially breaking and the major version number has been increased.
  * This change results in Alpaca clients now sending both the headers and body in one operation.
  * If required, the original `100-CONTINUE` behaviour can be restored by setting the new Alpaca client `request100Continue` parameter to `TRUE`.
  * The previous 100-CONTINUE behaviour caused the client to:
    * add an EXPECT 100-CONTNUE header to the request,
    * send only the request headers to the Alpaca device,
    * wait for the device to return a 100-CONTINUE response,
    * finally send the request body and wait for the device response.
* ADDED - A new telescope client configuration parameter: `throwOnBadDateTimeJSON`, which defaults to FALSE. This is primarily for use by Conform to support 
validation of DateTime values returned by Alpaca devices that do not conform to the Alpaca specification.
* ADDED - A new telescope client configuration parameter: `request100Continue`, which defaults to FALSE. This enables or disables `100-CONTINUE HTTP` behaviour.
* ADDED - Further client creation initialisers for the AlpacaClient and Alpaca device classes that expect a single AlpacaConfiguration class. 
The AlpacaConfiguration class encapsulates all Alpaca client configuration parameters and enables them to be set in a way that makes the configuration 
obvious in the source code.
* BUG-FIX - Fixed bug where `T AlpacaClient.GetDevice<T>` failed when creating a telescope client.

### Astrometry Tools changes in this release
* BUG-FIX - Fixed bug where the NOVAS component gave incorrect answers on 32bit Windows platforms. Other platforms were unaffected.
* ADDED - Over 200 additional SOFA functions, now, all functions in the SOFA library are available.

## Version 2.2.1
**Changes in this release**:

* ASCOM.Com.ChooserSA * Added a static Choose() method that enables a Chooser to be displayed without having to create and dispose of a ChooserSA instance.
* ASCOM.Com.Chooser * All Choose() methods are now marked as obsolete (see note below).

- **Note:** The Choose methods in the ASCOM.Com.Chooser component have been marked as obsolete in favour of using the Library's stand-alone Chooser (ASCOM.Com.ChooserSA) component.
This is because:
  * ASCOM.Com.Chooser is just a thin wrapper around the .NET 3.5 Framework Platform COM Chooser, while the ASCOM.Com.ChooserSA component is a complete re*write in .NET Core with no dependency on the .NET Framework.
  * The Chooser component is not reliable in projects that target .NET 9 and later. See the [](@ef3d33c3-7a7d-4e22-a4bf-7f5b2faf7f10) help topic for more information.
  * This is not a breaking change, the obsolete message appears as a Warning after compilation, but allows compilation to proceed to its normal conclusion.

## Version 2.2.0
**Changes in this release**:
 
* Astrometry Tools * Added the SetObserved and SetAzElObserved options to the Transform component.
* Alpaca Clients BUG*FIX * Fixed bug where the Action method failed when the 'parameters' parameter was over 65,535 characters long.
* ASCOM.COM.PlatformUtilities * Added support for WIndows 25H2 to the OSBuildName functions.
* ASCOM.Common.AlpacaTools * Add a new ToByteArray() method that allows the element type returned to the client to be specified. Previously the array would always have the element type of the array provided by the Alpaca device.

## Version 2.1.0

* Astrometry Tools * Add support for use in 32bit and 64bit Android applications.
* NOVAS * Improved ra cio bin file handling to enable use when the file cannot be located and the internal mechanic is used.
* Transform * Added observed mode to enable unrefracted topocentric coordinates to be converted to refracted topocentric coordinates i.e. observed coordinates.

## Version 2.0.9

* Alpaca Clients * Fixed bug where query strings in HTTP GETs of members that take parameters had multiple leading "?" characters.
* DeviceCapabilities * Added VersionIntroduced function.
* DeviceCapabilities * Added InterfaceHasMember function to report whether a member is present in a specified device type and interface version.

## Version 2.0.8

* DeviceCapabilities * Added IsSupportedInterface and DeviceCapabilities.IsValidInterface methods.
* Alpaca clients * Removed duplicate keep*alive element from the Connection header.
* Alpaca Clients * Fixed incorrect exception message when the client times out.

## Version 2.0.6

* Alpaca Clients * Change the timeout used for Connected*GET, Connecting, Connect() and Disconnect() from the standard timeout to the establish communications timeout,
which is usually shorter and facilitates quicker return to the client when the Alpaca device cannot be reached.
* Alpaca Clients * Make InterfaceVersion use the establish communications retry interval because some applications call this before trying to connect.
Change the delay between communications retries from 1.0 second to 0.1 seconds and reduce the number of socket error re*trys to 1 to speed up the process when the device cannot be reached.
* Alpaca Clients * Fix cosmetic issue in exception messages when reporting not implemented exceptions. Previously the exception added extraneous "is not implemented" text to the supplied message.
* Asynchronous method extensions * Fixed an issue where ConnectAsync and DisconnectAsync caused "member not found" exceptions when used with Platform 6 devices.
These methods now require the device type and device interface version to determine whether to use the Platform 6 or Platform 7 connection mechanic.
* Platform Utilities * Added the OSBuildName function to return the descriptive name of the operating system e.g. Windows 11 (24H2).

## Version 2.0.1 - Supports Platform 7
* ASCOM.Tools Package Breaking Change

  * All astrometry related functions including Transform, SOFA and NOVAS, have been moved into a
new package: **ASCOM.AstrometryTools** in order to reduce the size and complexity of the ASCOM.Tools package and increase convenience for
developers who don't require astrometry features.
 
  * Class and object names (including namespaces) have been retained as far as possible, the major naming change is that astrometry features in the ASCOM.Tools.Utilities component
are now in a new ASCOM.Tools.AstroUtilities component in the new ASCOM.AstroUtilities package.

  * The principle developer changes required are to:
    * Install the new ASCOM.AstrometryTools package from NuGet
    * Add the package to the project
    * Add a "using ASCOM.AstrometryTools;" reference to relevant classes
    * In the codebase, change astrometry function references from ASCOM.Tools.Utilities.XXX() to ASCOM.Tools.AstroUtilities.XXX()

* Added Support for Platform 7 interfaces including Connect(), Disconnect(), DeviceState, SwitchAsync() and SwitchValueAsync().
* Added Client Toolkit awaitable Task extensions for the Connect(), Disconnect(), SwitchAsync() and SwitchValueAsync() methods.
* Added Windows ARM64 support for NOVAS and SOFA components.
* Added DeviceCapabilities.IsPlatform7OrLater function and improved help text for other DeviceCapabiities methods.
* Updated SOFA to release 19 * 11th October 2023.
* Fix - Include the driver's original exception as an inner exception when throwing exceptions from Com.DriverAccess to make behaviour consistent with Platform behaviour.
* Fix - Prevent two unintended NullReferenceExceptions in Com.DriverAccess when handling exceptions returned by drivers.
* Fix - Accept an array of integers for Gains to ensure that drivers remain usable when clients use the ASCOM Library.
* Fix - The default TraceLogger log path on non*Windows systems is once again "Documents/ascom"; it unintentionally became "Documents" in the 1.0.111 release from October 2023.
* Fix - Remove a small error (6th decimal place) in AstroUtililties JulianDate function.
* Fix - Eliminate a stack imbalance when returning from x86 native library calls by specifying the CDECL calling convention.
* Fix - Alpaca Clients did not set the remote device number correctly when changed through the ClientConfiguration class.

## Version 1.0.111
* Alpaca Clients * Fix issue that caused Alpaca client initialisation to fail on Android and similar AOT compile platforms.

## Version 1.0.110
* Re*release of version 1.0.109 without additional features and changes that are intended for a future release.

## Version 1.0.109
* ASCOM.COM * Added PlatformUtilities.IsPlatformInstalled() function.
* ASCOM.COM * Profile.GetDrivers() now returns an empty list instead of an exception if no drivers are found.

## Version 1.0.108
* ASCOM.COM * Fixed an issue where interface version 1 Focusers would have Connected called instead of Link
* ASCOM.Tools * Added a NOVAS3.1 component to Utilities.
* ASCOM.Tools * Added MoonPhase, MoonIllumination and EventTimes astrometry functions to Utilities.
* Added an Almanac generator to Utilities. This writes a whole year almanac for a particular event to an ILogger instance. Supported events are:
* Rise and set times for the planets, sun and moon
* Start and end times for Civil, Nautical and Astronomical twilight

## Version 1.0.107
* ASCOM.Tools * Added missing linux*arm32 native SOFA library.

## Version 1.0.106
* Alpaca Clients * Refactored base class to enable error number and message to be extracted more easily.
* Alpaca Clients * Added support for trusting user generated certificates.
* Alpaca Clients * Improved handling of Alpaca management information during discovery on Linux, Arm and MacOS operating systems.

## Version 1.0.52
* First production release.