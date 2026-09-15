## ASCOM.Alpaca.Device

Library of ASCOM Alpaca device specific components.

## Version History
The version history only contains entries when a change is made.

***Release 4.1.0***
* BUGFIX - The responder now listens for discovery packets on the IPv6 localhost address ::1 when configured to do so. Previously it only listened on the IPv6 link local addresses.
* BUGFIX - Now ensures that exactly one UDP awaiter is running at a time when waiting for the next discovery packet. Previously the number of awaiters incremented by one every time a discovery packet was received, which could eventually result in 
resource starvation.

***Release 3.0.0***
*  ADDED - Support for .NET 8.0, 9.0 and 10.0 target frameworks. It will no longer be necessary for applications that use these frameworks to 
rely on the .NET Standard 2.0 component.

***Release 1.0.110***
* Re-release of version 1.0.109 without additional features and changes that are intended for a future release.