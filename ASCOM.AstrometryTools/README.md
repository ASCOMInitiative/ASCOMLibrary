# ASCOM.AstrometryTools

A set of cross-platform tools to support astrometry requirements. The components include the NOVAS and SOFA native libraries.

This release supports the interface updates introduced in ASCOM Platform 7.

## Utilities
* Transform.
* Sofa is a cross platform release of the ASCOM Platform's SOFA component.
* Novas is a cross platform release of the ASCOM Platform's NOVAS3.1 component.
* NOVASCOM
* Kepler

# Version History
The version history only contains entries when a change is made, if a release version is not listed below, there was no change to this component in that release.

***Release 3.0.0***
* BUG-FIX - Fixed bug where the NOVAS component gave incorrect answers on 32bit Windows platforms. Other platforms were unaffected.
*  ADDED - Support for .NET 8.0, 9.0 and 10.0 target frameworks. It will no longer be necessary for applications that use these frameworks to 
rely on the .NET Standard 2.0 component.

***Release 2.2.0***
* Add SetObserved() and SetAzimuthElevationObserved() methods to the Transform component.

***Release 2.1.0***
* Improved NOVAS ra cio bin file handling to enable use when the file cannot be located and the internal mechanic is used.
* Transform - Add observed mode to enable unrefracted topocentric coordinates to be converted to refracted topocentric coordinates i.e. observed coordinates.
* Astrometry Tools can now be used in applications that run on Android 32bit and 64bit.

***Release 2.1.0-rc.4***
* Improve NOVAS ra cio bin file handling.
* Transform - Add observed mode to enable unrefracted topocentric coordinates to be converted to refracted topocentric coordinates i.e. observed coordinates.

***Release 2.1.0-rc.3***
* Replace Android libraries for NOVAS and SOFA with differently compiled versions. Improve log messages.

***Release 2.1.0-rc.2***
* NOVAS - Add additional search paths for the JPLEPH file to make it work on Android

***Release 2.1.0-rc.1***
* Add experimental SOFA and NOVAS support for Android 32bit and 64bit.

***Release 2.0.0***
* No change.

***Release 2.0.0-rc.28***
* Native library calling convention for SOFA and NOVAS changed to CDecl in order to remove an "imbalanced stack" condition when calls return. The issue seems 
* to have been handled transparently in Release but was reported in Debug configurations.

***Release 2.0.0-rc.23***
* Initial release after migration of features from the ASCOM.Tools package.
* Significantly reduced package size by restricting NOVAS Planet ephemeris data to start at the year 2020 rather than 1900
* Extended the range of NOVAS Planet ephemeris data to the year 2050 from 2035.
* SOFA updated to Release 19 dated 11th October 2023.
* Fix - AstroUtilities.JulianDateFromDateTime extended to handle dates more than 5 years in the future.
* Fix - Removed very small error (6th decimal place) in AstroUtilities.JulianDate function