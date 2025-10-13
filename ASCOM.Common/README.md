# ASCOM.Common
This is a set of data classes and utility functions to support writing ALpaca devices and COM clients. Written to .Net Standard 2.0 for maximum applicability.

This release supports the interface updates introduced in ASCOM Platform 7.

# Components
* **ASCOM.Common.Alpaca** - Common types for accessing and working with Alpaca devices.
* **ASCOM.Common.Com** - Common types for accessing and working with ASCOM COM drivers.
* **ASCOM.Common.DeviceInterfaces** - Device interfaces for Alpaca and COM drivers. Also includes types and enums.
* **ASCOM.Common.Helpers** - Sets of extension methods to help use ASCOM and Alpaca drivers.
* **ASCOM.Common.Interfaces** - Interfaces for library functions.

# Version History
The version history only contains entries when a change is made, if a release version is not listed below, there was no change to this component in that release.

***Release 2.2.0***
* AlpacaTools - Add a new ToByteArray() method that allows the element type returned to the client to be specified. Previously the array would always have the element type of the array provided by the Alpaca device.

***Release 2.0.9***
* Add new InterfaceHasMember and DeviceCapabilities.VersionIntroduced methods to help clients and drivers support multiple interface versions.

***Release 2.0.8***
* Refactor DeviceCapabilities so that there is only one source of truth for when interface changes occurred.
* Add IsSupportedInterface and IsValidInterface methods to DeviceCapabilities.

***Release 2.0.7***
* Added further logging to the ConnectAsync extension to aid debugging.

***Release 2.0.6***
* Fix - The ConnectAsync and DisconnectAsync extension methods now work correctly with both Platform 6 and 7 devices. Both methods have additional mandatory parameters
to specify the device type and interface version implemented by the device. These are required to enable the method to differentiate between Platform 6 and 
Platform 7 interfaces. For Platform 7 interface devices the Connect() and Disconnect() methods are used while the Connected property is used for Platform 6 devices.

***Release 2.0.0***
* No change.

***Release 2.0.0-rc.28***
* No change.

***Release 2.0.0-rc.23***
* Add support for Platform 7 interface changes.
* Added JSON response classes for new interface members.
* Added further discovery members to DeviceCapabilities such as IsPlatform7Orlater(), HasConnectAndDeviceState() and HasCoverMoving().
* Added client async task extensions ConnectAsync() and DisconnectAsync().
* Fix - A MethodNotImplementedException was replaced by a NotImplementedException in line with Library policy.
* Fix - Client async methods now respect timeouts if the target driver/device locks up and does not return from the initiator or polling variable.

* ***Release 1.0.112***
* No change.

***Release 1.0.111***
* No change.

***Release 1.0.110***
* Re-release of version 1.0.109 without additional features and changes that are intended for a future release.

***Release 1.0.109***
* No change.

***Release 1.0.108***
* No change.

***Release 1.0.107***
* No change.

***Release 1.0.106***
* A new base class has been introduced in the Alpaca response classes to enable just error keys to be extracted
from JSON responses. This is expected to be a non-breaking backward compatible change.
