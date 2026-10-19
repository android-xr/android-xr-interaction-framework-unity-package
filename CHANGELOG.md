# Changelog

All notable changes to the Android XR Interaction Framework Unity Package will be added
here. It includes changes that affect public APIs, samples and runtime
behaviors.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this package adheres to
[Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.9.1-pre.1] - 2026-10-19

This update includes general fixes and improvements for compatibility with the upcoming XREAL Aura device.

### Editor version and package compatibility
  * Package dependencies updated in this version:
    * Android XR Extensions (`com.google.xr.extensions`) 1.4.0
    * XR Interaction Toolkit (`com.unity.xr.interaction.toolkit`) 3.3.2

### Known issues
  * None.

### Added
  * Added two additional fields to pointers to represent scaling and rotation. These fields are used primarily during Gaze & Pinch interactions.
  * Added an additional field in mouse source data to represent ancillary actions from specialty hardware. Transitions to and from mouse pointers will respect this new field.

### Changed
  * None.

### Deprecated
  * None.

### Removed
  * None.

### Fixed
  * Fixed an issue with the table in the AXRIF Demo scene not reliably appearing in front of the user.
  * Added a missing package dependency (`com.unity.visualscripting`) that was previously added transitively.
  * Fixed an issue where manifest requirements in build hooks were checked more often than necessary.
  * Updated outdated bindings for mouse interaction with the correct path.

## [0.9.0-pre.1] - 2026-05-19

This is the first preview release of the **Android XR Interaction Framework** Unity package. This version supports the following features:

-   Hand Raycasting
-   Gaze & Pinch (either with eyes or head)
-   XR Controllers
-   Spatial Mouse
-   Direct Touch (Poke)
-   Automatic Transitions
-   Android XR Hand and Reticle visuals
