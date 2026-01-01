# ASCOM.COM

A set of tools to access ASCOM COM drivers. Written to .Net Standard 2.0 for maximum applicability.

This release supports the interface updates introduced in ASCOM Platform 7.

# Version History
The version history only contains entries when a change is made, if a release version is not listed below, there was no change to this component in that release.

***Release 3.0.0***
*  ADDED - Support for .NET 8.0, 9.0 and 10.0 target frameworks. It will no longer be necessary for applications that use these frameworks to 
rely on the .NET Standard 2.0 component.

***Release 2.2.1***
* Updated the "Obsolete" messages on both ASCOM.Com.Chooser Choose() methods with more a comprehensive rationale.
* Added a static member to ChooserSA enabling a device to be chosen without having to create and dispose of a ChooserSA instance

***Release 2.2.0***
* Added support for WIndows 25H2 to the OSBuildName functions.
* Added reason description for ASCOM.Com.Chooser being marked as obsolete.

***Release 2.0.4***
* Added the PlatformUtiltiies.OSBuildName() and OSBuildName(int buildNumber) functions that will return a descriptive name for a Windows operating system based on its build number.

***Release 2.0.0***
* No change.

***Release 2.0.0-rc.28***
* No change.

***Release 2.0.0-rc.23***
* Add support for new Platform 7 interface members.
* Add device state convenience members that package device state in an easy to use form.
* Fix - Four MethodNotImplementedException replaced with NotImplementedExceptions to match the Library standard.
* Fix - Include the driver's original exception as an inner exception when throwing exceptions from Com.DriverAccess to make behaviour consistent with Platform behaviour.
* Fix - Prevent two unintended NullReferenceExceptions in Com.DriverAccess when handling exceptions returned by drivers.
* Fix - Accept an array of integers for Gains to ensure that drivers remain usable when clients use the ASCOM Library.
* Client async methods now respect timeouts if the target driver/device locks up and does not return from the initiator or polling variable.

***Release 1.0.112***
* No change.

***Release 1.0.111***
* No change.

***Release 1.0.110***
* Re-release of version 1.0.109 without additional features and changes that are intended for a future release.

***Release 1.0.109***
* Added PlatformUtilities.IsPlatformInstalled() function.
* Profile.GetDrivers() now returns an empty list instead of an exception if no drivers are found.

***Release 1.0.108***
* Fixed an issue where interface version 1 Focusers would have Connected called instead of Link

***Release 1.0.107***
* No change.

***Release 1.0.106***
* No change