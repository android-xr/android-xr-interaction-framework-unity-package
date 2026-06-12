# Android XR Interaction Framework - Unity Package

The Android XR Interaction Framework (AXRIF) provides familiar, high-level, opinionated interactions for OpenXR applications on Android XR. It bridges the gap between system-level interactions and in-app interactions, offering an intuitive and cohesive way to handle user input across various game engines.

Building natural and comfortable interaction design from scratch is difficult and can consume a large portion of your development time. Use AXRIF to inherit the exact input, interaction, and transition behaviors of the Android XR system in your own applications.

- **Seamless Multimodal Transitions:** AXRIF features a built-in Transition Manager that handles automatic transitions between peripherals, hands, eyes, and direct touch.

- **Future-Proof Compatibility:** When new interaction modes are added in future releases of Android XR, AXRIF automatically includes support for them with minimal developer effort.

- **Flexible Control:** You can choose to enable only the specific interaction modes your app needs, and you can override the framework via API at any time if you need to temporarily take manual control of input logic.

## Supported Input Modalities
AXRIF supports a comprehensive suite of input modalities out of the box. Transitions between all supported modalities are available and handled automatically.


| Modality       | Description                                                                                            |
|----------------|--------------------------------------------------------------------------------------------------------|
| XR Controllers | 6DoF controllers are used to point and select from a distance.                                         |
| Gaze + Pinch   | Eyes are used to aim and hands are used to select. Can be configured to use head aim rather than eyes. |
| Direct Touch   | Reach out and poke to directly interact with UI elements.                                              |
| Hand Raycast   | Hands are used to point and select from a distance.                                                    |
| Mouse          | Supports familiar mouse interactions like click and scroll, projected into your 3D scene.              |

## Architecture at a Glance
To ensure maximum performance and cross-engine compatibility, AXRIF is split into two primary components:
- **Core Library:** This component houses the universally shared, engine-agnostic logic for interactions.
- **Engine Plug-ins:** These integration layers convert native data types into the respective engine’s analogous structures.

## How To Use:

The AXRIF Unity Plugin integrates directly with Unity's XR Interaction Toolkit (XRI) to maximize compatibility with the input systems you are already familiar with. To get started, please follow the [Unity Setup Guide](https://developer.android.com/develop/xr/unity/interaction-framework) to add the package to your app.
