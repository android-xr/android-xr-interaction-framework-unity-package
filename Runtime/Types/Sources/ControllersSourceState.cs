// Copyright 2025 Google LLC
// Copyright Qualcomm Technologies, Inc. and/or its affiliates. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Runtime.InteropServices;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Represents the state of a single controller for a given frame.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct ControllerState {
    /// <summary>
    /// Indicates whether the controller is currently active.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsActive;

    /// <summary>
    /// The 6-dof pose of the controller aim, as defined by the OpenXR spec.
    /// </summary>
    public Android.XR.InteractionFramework.Posef AimPose;

    /// <summary>
    /// The boolean state of the primary action (A or X), as defined by the OpenXR spec.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsPrimaryPressed;

    /// <summary>
    /// The boolean state of the secondary action (B or Y), as defined by the OpenXR spec.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsSecondaryPressed;

    /// <summary>
    /// The X-axis value of the controller thumbstick.
    /// </summary>
    public float ThumbstickXValue;

    /// <summary>
    /// The Y-axis value of the controller thumbstick.
    /// </summary>
    public float ThumbstickYValue;

    /// <summary>
    /// The boolean state of the thumbstick press action.
    /// </summary>
    public bool IsThumbstickPressed;

    /// <summary>
    /// The trigger value of the controller.
    /// </summary>
    public float TriggerValue;
  }

  /// <summary>
  /// Represents the state of the 6DoF controllers input source for a given frame.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct ControllersSourceState {
    /// <summary>
    /// Indicates whether the controllers source state is valid.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    /// <summary>
    /// The current state of the left controller.
    /// </summary>
    public ControllerState LeftController;

    /// <summary>
    /// The current state of the right controller.
    /// </summary>
    public ControllerState RightController;
  }
}  // namespace Android.XR.InteractionFramework
