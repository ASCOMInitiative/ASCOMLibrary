# ASCOM.Standard.COM.DriverAccess

An experimental library to access ASCOM COM drivers from .Net Standard / Core / 5.0. This then translates the interfaces to use the ASCOM Standard version of the interface to behave the same as Alpaca devices.

Currently this reads the registry directly looking for device ProgIDs. It does not check the bitness of the registered drivers. Because .Net Core is not supposed to load from the GAC this uses reflection for all access and does not use any ASCOM platform interfaces.

This should display the UI if the loading platform supports UI. Simply call SetupDialog like normal. Out of process (local server) drivers may have better UI support.

Each device type offers a static list of "ASCOMRegistrations". This is all registered drivers of that type that the library could find. Each ASCOM Registration contains the ProgID needed to activate the driver and the more understandable Name of the driver.

A chooser style UI will be provided in a separate project as an extension method. This will allow .Net Core projects to use this library without requiring a dependency on Windows UI.