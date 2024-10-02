# ASCOM.Alpaca

A set of client tools to discover and access Alpaca devices. Written to .Net Standard 2.0 for maximum applicability.

This release supports the interface updates introduced in ASCOM Platform 7.

Please note that you will need to add this PropertyGroup to .NET projects that target Android in order for Alpaca discovery to work as expected:
```xml
    <PropertyGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">
        <UseNativeHttpHandler>false</UseNativeHttpHandler>
    </PropertyGroup>
```

This may also be the case for projects that target IOS, but has not yet been confirmed.

# Version History
The version history only contains entries when a change is made, if a release version is not listed below, there was no change to this component in that release.

***Release 2.0.3***
* Some applications call InterfaceVersion before connecting, hence changing the timeout used for InterfaceVersion-GET from the standard timeout to the establish communications timeout, which is 
usually shorter and facilitates quicker return to the client when the Alpaca device cannot be reached.
* Reduced the delay between communications error retries from 1.0 second to 0.1 seconds.

***Release 2.0.2***
* Change the timeout used for Connected-GET, Connecting, Connect() and Disconnect() from the standard timeout to the establish communications timeout, which is 
usually shorter and facilitates quicker return to the client when the Alpaca device cannot be reached.

* ***Release 2.0.0***
* Fix bug where the remote device number was not set correctly when changed through the ClientConfiguration class.

***Release 2.0.0-rc.28***
* No change.

***Release 2.0.0-rc.23***
* Add support for new Platform 7 interface members.
* Added client async task extensions ConnectAsync() and DisconnectAsync().
* New device state convenience members that package device state in an easy to use form.
* New ClientRefresh() method to effect configuration changes made through the client configuration instance returned by ClientConfiguration
* Fix - Four MethodNotImplementedException replaced with NotImplementedExceptions to match the Library standard.
* Fix - Client async methods now respect timeouts if the target driver/device locks up and does not return from the initiator or polling variable.

***Release 1.0.112***
* Fix issue where Alpaca clients did not query the device to determine its Connected state.

* ***Release 1.0.111***
* Fix issue that caused Alpaca client initialisation to fail on Android and similar AOT compile platforms.

***Release 1.0.110***
* Re-release of version 1.0.109 without additional features and changes that are intended for a future release.

***Release 1.0.109***
* No change.

***Release 1.0.108***
* No change.

***Release 1.0.107***
* No change.

***Release 1.0.106***
* Improved Alpaca Client debug logging
* Improved Alpaca Client resilience to unexpected content in JSON error responses.

***Release 1.0.96***
* Added support for trusting user generated certificates.
* Improved handling of Alpaca management information during discovery on Linux, Arm and MacOS operating systems.