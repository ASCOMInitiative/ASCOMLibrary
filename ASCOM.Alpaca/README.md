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

***Release 3.0.0***
* POTENTIALLY BREAKING CHANGE - The client's [`100-CONTINUE`](https://dev.to/mrcaidev/everything-you-need-to-know-about-100-continue-3mn5)
behaviour is no longer enabled by default to improve network performance by removing a network round-trip on each `PUT` request.
  * This change is expected to be transparent for clients and devices because Alpaca devices should already be capable of handling requests from other clients 
that do not use the `EXPECT 100-CONTINUE` protocol. Out of an abundance of caution, the change is marked as potentially breaking and the major version number has been increased.
  * This change results in Alpaca clients now sending the both headers and body in one operation.
  * If required, the original `100-CONTINUE` behaviour can be restored by setting the new Alpaca client `request100Continue` parameter to `TRUE`.
  * The previous 100-CONTINUE behaviour caused the client to:
    * add an EXPECT 100-CONTNUE header to the request,
    * send only the request headers to the Alpaca device,
    * wait for the device to return a 100-CONTINUE response,
    * finally send the request body and wait for the device response.
* ADDED - A new telescope client configuration parameter: `throwOnBadDateTimeJSON`, which defaults to FALSE. This is primarily for use by Conform to support 
validation of DateTime values returned by Alpaca devices that do not conform to the Alpaca specification.
* ADDED - A new telescope client configuration parameter: `request100Continue`, which defaults to FALSE. This enables or disables `100-CONTINUE HTTP` behaviour.
* ADDED - Further client creation initialisers for the AlpacaClient and Alpaca device classes that expect a single AlpacaConfiguration class. 
The AlpacaConfiguration class encapsulates all Alpaca client configuration parameters and enables them to be set in a way that makes the configuration 
obvious in the source code.
* ADDED - Support for .NET 8.0, 9.0 and 10.0 target frameworks. It will no longer be necessary for applications that use these frameworks to 
rely on the .NET Standard 2.0 component.
* BUG-FIX - Fixed bug where `T AlpacaClient.GetDevice<T>` failed when creating a telescope client.

***Release 2.2.0***
* BUG-FIX - Fixed bug where the Alpaca client Action method failed when the 'parameters' parameter was over 65,535 characters long.

***Release 2.0.9***
* BUG-FIX - Fixed bug where query strings in HTTP GETs of members that take parameters, e.g. Telescope.CanSetAxisrate(Axis), had multiple leading "?" characters 
instead of a single leading "?" character.

***Release 2.0.8***
* Fix incorrect exception message when the client times out.
* Remove duplicate keep-alive element from the Connection header.

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