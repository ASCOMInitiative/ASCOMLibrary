# ASCOMStandard
This repository contains ASCOM Alpaca support components written in .NET Standard. These are intended to assist developers in creating effective ASCOM Alpaca applications and drivers.

See https://www.ascom-standards.org/Developer/Alpaca.htm for further information on ASCOM Alpaca.

# Projects
* ASCOM.Compatibility.Utilities - This project allows the current ASCOM Platform profile and TraceLogger to be used with the new ILogger and IProfile interfaces.
* ASCOM.Standard.COM.DriverAccess - Driver Access for ASCOM COM drivers. This converts the API for devices into the ASCOM Standard API.
* ASCOM.Standard.Types - The types, interfaces and enums for the ASCOM Standard library.
* ASCOM.Standard.Utilities - Cross-platform utility code to help create drivers and clients in the Alpaca world. This includes logging, settings storage, discovery and transforms.
* ASCOMStandard.Tests - A test project.

# ToDo
* Close open notes / issues. There is a readme in each folder with the notes for each project
* Pick final strong name key (use Alpaca key or ASCOM key)
* Decide on Namespace conventions and organizational structure
* Test on Windows, Linux and MacOS (only minimally tested in MacOS)