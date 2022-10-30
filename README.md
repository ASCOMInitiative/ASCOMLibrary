# ASCOM Cross-Platform libraries
This repository contains cross platform ASCOM Alpaca and Microsoft COM support components that target .NET Standard 2.0 to provide the widest applicability. These are intended to assist developers in creating effective ASCOM Alpaca / COM applications and Alpaca devices / COM drivers.

See https://www.ascom-standards.org/Developer/Alpaca.htm for further information on ASCOM Alpaca.

# Library Capabilities
* Alpaca clients that provide straightforward access to Alpaca devices
* Simple tools to discover available Alpaca devices.
* COM clients that provide straightforward access to Windows COM drivers
* Ability for applications to treat Alpaca and COM devices interchangeably
* Asynchronous async/await support for long-running processes such as Telescope.SlewToCoordinates()
* ILogger framework with ConsoleLogger and TraceLogger components
* SOFA and Transform astrometric calculation support components
* Utilities to support development
* Chooser and Profile components that provide similar functionality to the ASCOM Platform's components (Windows only)
* Whole profile load and save component (Windows only)
* A range of lower level definitions, data structures and interfaces to support development.

# Packages
The library is distributed via NuGet and consists of five packages:
* ASCOM.Alpaca.Components - ASCOM Alpaca Clients and Client Discovery Library.
* ASCOM.Alpaca.Device - Device / driver side discovery library.
* ASCOM.Com.Components - A .Net Standard (.Net Core / .Net 5+) access library for ASCOM COM drivers.
* ASCOM.Tools - A set of CrossPlatform tools for logging, settings and conversions.
* ASCOM.Common.Components - The types, interfaces and enums for the ASCOM CrossPlatform library.