# ASCOM.Tools

A set of cross platform utilities, logging, and basic settings providers that implement the 
ASCOM.Common interfaces. Written to .Net Standard 2.0 for maximum applicability.

## Logging
Given the change to non shared libraries should TraceLogger require a mutex?

## Utilities
* ConsoleLogger is an ILogger implementation that logs out to the console, using color coding messages
* Logger is a static class which contains an ILogger. ASCOM Standard logs out here, but a developer can 
switch the logging provider. This allows the platform to use the same logger as the developed software. 
All methods are safe, they should never be able to throw.
* TraceLogger is a cross platform implementation of the ASCOM Platform's TraceLogger that exposes ILogger. 
This is the default logger for ASCOM Standard
* XMLProfile is a IProfile implementation that stores settings in an xml file. By default this is 
stored in $HOME/.config/stuff on Linux and User/.ASCOM/stuff on Windows. You can see the full paths and 
how they are created in the class. The reason Windows uses .ASCOM instead of .config is that .config is 
created by third party software on Windows and could be removed by that software. The locations need to be 
approved and this needs to be tested on MacOS (coming soon).
* Sofa is a cross platform release of the ASCOM Platform's SOFA component.
* Utilities contains a variety of astronomy oriented helper functions to complement the general purpose
routines that are available in standard .NET namespaces.
* Novas is a cross platform release of the ASCOM Platform's NOVAS3.1 component.

# Version History

***Release 1.0.112***
* No change.

***Release 1.0.111***
* No change.

***Release 1.0.110***
* Re-release of version 1.0.109 without additional features and changes that are intended for a future release.

***Release 1.0.109***
* No change.

***Release 1.0.108***
* Added a NOVAS3.1 component to Utilities.
* Added MoonPhase, MoonIllumination and EventTimes astrometry functions to Utilities.
* Added an Almanac generator to Utilities. This writes a whole year almanac for a particular event to an ILogger instance. Supported events are:
  * Rise and set times for the planets, sun and moon
  * Start and end times for Civil, Nautical and Astronomical twilight

***Release 1.0.107***
* Added missing linux-arm32 native SOFA library.

***Release 1.0.106***
* No change