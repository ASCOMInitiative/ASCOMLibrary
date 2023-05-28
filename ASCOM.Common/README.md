# ASCOM.Common
This is a set of data classes and utility functions to support writing ALpaca devices and COM clients. Written to .Net Standard 2.0 for maximum applicability.

# Components
* **ASCOM.Common.Alpaca** - Common types for accessing and working with Alpaca devices.
* **ASCOM.Common.Com** - Common types for accessing and working with ASCOM COM drivers.
* **ASCOM.Common.DeviceInterfaces** - Device interfaces for Alpaca and COM drivers. Also includes types and enums.
* **ASCOM.Common.Helpers** - Sets of extension methods to help use ASCOM and Alpaca drivers.
* **ASCOM.Common.Interfaces** - Interfaces for library functions.

# Version History

***Release 1.0.107***
* No change.

***Release 1.0.106***
* A new base class has been introduced in the Alpaca response classes to enable just error keys to be extracted
from JSON responses. This is expected to be a non-breaking backward compatibile change.
