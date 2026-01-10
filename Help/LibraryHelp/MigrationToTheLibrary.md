---
uid: MigrationFromPlatform
title: Migrating clients and devices to the ASCOM Library
tocTitle: Migrating to the ASCOM Library
# linkText: Optional Text to Use For Links
# keywords: keyword, term 1, term 2, "term, with comma"
# alt-uid: optional-alternate-id
# summary: Optional summary abstract
---

<!-- Comments like this one will not appear in the generated topic.
If uncommented, this will generate an auto-outline in presentation styles that don't automatically create one
such as the VS2013 and Open XML presentation styles.
<autoOutline /> -->

## Device Interfaces
With the exception of some enum value names, the interfaces implemented by the library are identical to those defined in the 
[ASCOM Master Interface Document](https://ascom-standards.org/newdocs).
While the enum value names have been made shorter, the values available are identical to those in the equivalent enums defined in the master document 
and used in ASCOM Platform components.

For example, the `PierSide` enum defined in the Platform has been replaced by the `PointingState` enum in the Library.

## Enum Mappings
The following tables shows the mapping between the ASCOM Platform enums and the ASCOM Library enums.

### AlignmentModes => AlignmentMode
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `AlignmentModes.altaz` | `AlignmentMode.AltAz`         |
| `AlignmentModes.equatorial` | `AlignmentMode.Equatorial` |
| `AlignmentModes.polarscope` | `AlignmentMode.PolarScope` |
| `AlignmentModes.unknown` | `AlignmentMode.Unknown`     |

### CalibratorStatus => CalibratorState
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `CalibratorStatus.calibratorOff` | `CalibratorState.Off`     |
| `CalibratorStatus.calibratorOn` | `CalibratorState.On`       |
| `CalibratorStatus.calibratorError` | `CalibratorState.Error`   |
| `CalibratorStatus.calibratorUnknown` | `CalibratorState.Unknown` |

### CameraStates => CameraState
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `CameraStates.cameraIdle` | `CameraState.Idle`           |
| `CameraStates.cameraWaiting` | `CameraState.Waiting`       |
| `CameraStates.cameraExposing` | `CameraState.Exposing`     |
| `CameraStates.cameraReading` | `CameraState.Reading`       |
| `CameraStates.cameraDownload` | `CameraState.Downloading`   |
| `CameraStates.cameraError` | `CameraState.Error`         |
| `CameraStates.cameraUnknown` | `CameraState.Unknown`       |	

### CoverStatus => CoverState
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `CoverStatus.coverOpen` | `CoverState.Open`             |
| `CoverStatus.coverClosed` | `CoverState.Closed`         |
| `CoverStatus.coverOpening` | `CoverState.Opening`       |
| `CoverStatus.coverClosing` | `CoverState.Closing`       |
| `CoverStatus.coverError` | `CoverState.Error`           |
| `CoverStatus.coverUnknown` | `CoverState.Unknown`       |

### DriveRates => DriveRate
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `Driverates.driveSidereal` | `DriveRate.Sidereal`         |
| `Driverates.driveLunar` | `DriveRate.Lunar`             |
| `Driverates.driveSolar` | `DriveRate.Solar`             |
| `Driverates.driveKing` | `DriveRate.King`               |

### EquatorialCoordinateType => EquatorialCoordinateType
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `EquatorialCoordinateType.equTopocentric` | `EquatorialCoordinateType.Topocentric` |	
| `EquatorialCoordinateType.equLocalTopocentric` | `EquatorialCoordinateType.Topocentric` |	
| `EquatorialCoordinateType.equBarycentric` | `EquatorialCoordinateType.Barycentric` |
| `EquatorialCoordinateType.equHeliocentric` | `EquatorialCoordinateType.Heliocentric` |
| `EquatorialCoordinateType.equUnknown` | `EquatorialCoordinateType.Unknown` |

### GuideDirections => GuideDirection
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `GuideDirections.guideNorth` | `GuideDirection.North`     |
| `GuideDirections.guideSouth` | `GuideDirection.South`     |	
| `GuideDirections.guideEast` | `GuideDirection.East`       |
| `GuideDirections.guideWest` | `GuideDirection.West`       |
| `GuideDirections.guideUnknown` | `GuideDirection.Unknown`   |

### PierSide => PointingState
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |	
| `PierSide.pierEast`    | `PointingState.Normal`         |
| `PierSide.pierWest`    | `PointingState.ThroughThePole` |
| `PierSide.pierUnknown` | `PointingState.Unknown`        |	

See this link for further information on why PierSide was revised in the Library: <link xlink:href="PierSideAndPointingState" >Side of pier and pointing state</link>.

### SensorType => SensorType
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `SensorTypes.Monochrome` | `SensorType.Monochrome`   |
| `SensorTypes.Color` | `SensorType.Color`       |
| `SensorTypes.RGGB` | `SensorType.RGGB`       |
| `SensorTypes.CMYG` | `SensorType.CMYG`           |
| `SensorTypes.CMYG2` | `SensorType.CMYG2`       |
| `SensorTypes.LRGB` | `SensorType.LRGB`       |

### ShutterState => ShutterState
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `ShutterState.shutterOpen` | `ShutterState.Open`         |
| `ShutterState.shutterClosed` | `ShutterState.Closed`     |
| `ShutterState.shutterOpening` | `ShutterState.Opening`   |
| `ShutterState.shutterClosing` | `ShutterState.Closing`   |
| `ShutterState.shutterError` | `ShutterState.Error`       |
| `ShutterState.shutterUnknown` | `ShutterState.Unknown`   |

### TelescopeAxes => TelescopeAxis
| ASCOM Platform Enum    | ASCOM Library Enum            |
| :--------:             | :-------:                     |
| `TelescopeAxes.axisPrimary` | `TelescopeAxis.Primary`   |
| `TelescopeAxes.axisSecondary` | `TelescopeAxis.Secondary` |
| `TelescopeAxes.axisTertiary` | `TelescopeAxis.Tertiary` |
| `TelescopeAxes.axisUnknown` | `TelescopeAxis.Unknown`   |
