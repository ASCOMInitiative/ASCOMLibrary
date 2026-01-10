---
uid: PierSideAndPointingState
title: PierSide And PointingState
tocTitle: Terminology: PierSide And PointingState
# linkText: Optional Text to Use For Links
keywords: pierside, pointingstate, sideofpier, destinationsideofpier
# alt-uid: optional-alternate-id
# summary: Optional summary abstract
---

## Introduction
For historical reasons, the name of the Platform's PierSide enum does not reflect its true meaning, see this link for further information:
[What is the meaning of pointing state?](https://ascom-standards.org/newdocs/ptgstate-faq.html).

Since we had the opportunity of a fresh start with the ASCOM Library, we decided to give the Library enum a name that better reflected its actual meaning. 
The PointingState enum is intended for use with Library components while the PierSide enum is intended for use with Platform components.

There is a 1 to 1 mapping and equivalence between the values and meanings of the two enums as shown below:

| PierSide Enum          | PointingState Enum             |
| :--------:             | :-------:                      |  
| `PierSide.pierEast`    | `PointingState.Normal`         |  
| `PierSide.pierWest`    | `PointingState.ThroughThePole` |  
| `PierSide.pierUnknown` | `PointingState.Unknown`        |  
