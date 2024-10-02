# ASCOM.AstrometryTools

A set of cross-platform tools to support astrometry requirements. The components include the NOVAS and SOFA native libraries.

This release supports the interface updates introduced in ASCOM Platform 7.

## Utilities
* Transform...
* Sofa is a cross platform release of the ASCOM Platform's SOFA component.
* Novas is a cross platform release of the ASCOM Platform's NOVAS3.1 component.
* NOVASCOM
* Kepler

# Version History
The version history only contains entries when a change is made, if a release version is not listed below, there was no change to this component in that release.

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