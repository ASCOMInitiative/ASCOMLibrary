# ASCOMStandard
This repository contains ASCOM Alpaca support components written in .NET Standard. These are intended to assist developers in creating effective ASCOM Alpaca applications and drivers.

See https://www.ascom-standards.org/Developer/Alpaca.htm for further information on ASCOM Alpaca.

# Projects
* ASCOM.Alpaca - ASCOM Alpaca Clients and Client Discovery Library.
* ASCOM.Alpaca.Device - Device / driver side discovery library.
* ASCOM.Com - A .Net Standard (.Net Core / .Net 5+) access library for ASCOM COM drivers.
* ASCOM.Common - The types, interfaces and enums for the ASCOM CrossPlatform library. 
* ASCOM.Tools - A set of CrossPlatform tools for logging, settings and conversions.

# ToDo
* Close open notes / issues. There is a readme in each folder with the notes for each project
* Pick final strong name key (use Alpaca key or ASCOM key)
* Decide on Namespace conventions and organizational structure
* Test on Windows, Linux and MacOS (only minimally tested in MacOS)