# ASCOM.Alpaca

A set of client tools to discover and access Alpaca devices. Written to .Net Standard 2.0 for maximum applicability.

PLease note that you will need to add this PropertyGroup:
```xml
    <PropertyGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">
        <UseNativeHttpHandler>false</UseNativeHttpHandler>
    </PropertyGroup>
```
to .NET MAUI projects that target Android or IOS in order for Alpaca discovery to work as expected.

# Version History

***Release 1.0.111***
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