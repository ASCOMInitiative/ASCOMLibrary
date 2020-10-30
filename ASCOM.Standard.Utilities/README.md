# Notes / Open issues / questions

## Conversions
These should be pretty much the same as the upstream ASCOM.Standard project. I have made no changes. I don't know where they came from and they need to be checked for completeness and function.

## Helpers
These are all newly added. The are helper methods for Alpaca interfaces (making it easier to handle Exceptions and Responses).
The open question is if they belong here or in ASCOM.Standard.Types (they are generic).

## Utilities
* ConsoleLogger is an ILogger implementation that logs out to the console, using color coding messages
* Logger is a static class which contains an ILogger. ASCOM Standard logs out here, but a developer can switch the logging provider. This allows the platform to use the same logger as the developed software. All methods are safe, they should never be able to throw.
* TraceLogger is a cross platform implementation of the ASCOM Platform's TraceLogger that exposes ILogger. This is the default logger for ASCOM Standard
* XMLProfile is a IProfile implementation that stores settings in an xml file. By default this is stored in $HOME/.config/stuff on Linux and User/.ASCOM/stuff on Windows. You can see the full paths and how they are created in the class. The reason Windows uses .ASCOM instead of .config is that .config is created by third party software on Windows and could be removed by that software. The locations need to be approved and this needs to be tested on MacOS (coming soon).