# Notes / Open issues / questions

## Interfaces
These should be pretty much the same as the upstream ASCOM.Standard project. I did update them to the latest ASCOM Platform versions.
I did not change types or names so these may diverge in the same way as upstream.
To decide: naming, namespaces, any cleanup.

## Platform Interfaces
These are all newly added to the fork.
### Enums
Any enums used by platform interfaces. Currently this is only LogLevels, which could be merged into ILogger if needed.
### IAlpacaDevice
IAlpacaDevice is an Alpaca specific interface that is used so the Alpaca server can read this information from the device library in a generic way. It is not part of the interface contract, it is used internally to pass information within a driver.
Currently this only includes AlpacaConfiguredDevice, allowing the device code project to report this information to the Alpaca server
### ILogger
Standard Extensions are provided in LoggerExtensions.cs.

Possible Additional methods / api changes
* Flush - A method that blocks until the log has finished writing out. This could be called before shutdown to ensure that all logs are created
* Allow each logging level to be turned on / off. This would require a bool LoggingLevelActive(LogLevel level) and a SetLoggingLevel(LogLevel level, bool active)
* Should we have a formatted log IE Log(LogLevel level, string message, object[] objects) here or as an extension?
* Are the LogLevels sufficient or should there be more or less?
* Should any of the Extensions be part of the Interface
### IProfile
This should be pretty clean. Open questions are naming and any methods that should be removed or added.
### ITraceLogger
A copy of the ASCOM Platform TraceLogger interface. Originally I planned to offer this but with the addition of ILogger it may no longer be needed.
### LoggerExtensions
These are methods that wrap the existing API in standard ways.
## Responses
These should be pretty much the same as the upstream ASCOM.Standard project. I did update them to the latest ASCOM Platform versions.
To decide: naming, namespaces, any cleanup.