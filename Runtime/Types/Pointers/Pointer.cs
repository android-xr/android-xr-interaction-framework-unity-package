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

using System;
using System.Runtime.InteropServices;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Represents the type of the pointer held in a pointer event. Some interaction modes may create
  /// multiple pointer types.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Pointer {
    public enum PointerType : sbyte {
      // Used by the Hand Raycast interaction type.
      HandRaycast = 0,
      // Used by the Hand Poke interaction type.
      HandPoke = 1,
      // Used by the Gaze Pinch interaction type. Gaze pointers are used to aim and HandGazePinch
      // pointers are used for selection.
      HandGazePinch = 2,
      Gaze = 3,
      // Used by the Mouse interaction type.
      Mouse = 4,
      // Used by the Controller interaction type.
      Controller = 5,
      // Used by the Surface Touch interaction type.
      SurfaceTouch = 6,
    }

    /// <summary>
    /// Flags representing the current activation states for a pointer.
    /// </summary>
    [Flags]
    public enum AxrifPointerActivationFlagType : int {
      // Inactive flag.
      None = 0,

      // The primary button (typically the Left Mouse Button). Used for selection, clicking, and
      // primary interactions.
      PrimaryButton = 1 << 0,

      // The secondary button (typically the Right Mouse Button). Used for context menus or
      // alternative interactions.
      SecondaryButton = 1 << 1,

      // The tertiary button (typically the Middle Mouse Button or Scroll Wheel click). Used for
      // advanced navigation, tab opening, or auxiliary actions.
      TertiaryButton = 1 << 2
    }

    /// <summary>
    /// Indicates whether the pointer is valid.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    /// <summary>
    /// Indicates whether the pointer is active. A pointer may become inactive if the underlying
    /// source data becomes invalid (i.e. due to tracking loss) or if the interaction module
    /// determines that the pointer is not currently needed.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsActive;

    /// <summary>
    /// The type of the pointer.
    /// </summary>
    public PointerType Type;

    /// <summary>
    /// The handedness of the pointer.
    /// </summary>
    public Handedness Handedness;

    /// <summary>
    /// The line along which the pointer is cast.
    /// </summary>
    public Android.XR.InteractionFramework.Line Line;

    /// <summary>
    /// A value from -1 to 1 indicating the amount of horizontal scrolling provided by a device with
    /// built-in scrolling capabilities (e.g., a mouse).
    /// </summary>
    public float HorizontalScroll;

    /// <summary>
    /// A value from -1 to 1 indicating the amount of vertical scrolling provided by a device with
    /// built-in scrolling capabilities (e.g., a mouse).
    /// </summary>
    public float VerticalScroll;

    /// <summary>
    /// The amount of activation from [0, 1]. 0 represents no activation, 1 represents full
    /// activation. Some interactions may choose to populate this field as a simple boolean value.
    /// </summary>
    public float ActivationAmount;

    /// <summary>
    /// Used to hold additional information about the pointer's activation, e.g. additional or
    /// alternate buttons.
    /// </summary>
    public AxrifPointerActivationFlagType ActivationFlags;

    /// <summary>
    /// The scaling ratio for pointers that support scaling (e.g. GazePinch with
    /// both hands). A value of 1.0 indicates no scaling. Greater values indicate
    /// an increase in scaling, and smaller values indicate a decrease in scaling.
    /// </summary>
    public float ScaleRatio;

    /// <summary>
    /// The rotation angle in radians around the pointer axis for pointers that
    /// support rotation (e.g. GazePinch with both hands), relative to the initial
    /// angle when the rotation began.
    /// </summary>
    public float RotationValue;

    public override string ToString() {
      return $"IsActive = {IsActive} - " + $"Type = {Type} - " + $"Handedness = {Handedness} - " +
             $"Line = {Line} - " + $"ActivationAmount = {ActivationAmount} - " +
             $"ActivationFlags = {ActivationFlags}" + $"ScaleRatio = {ScaleRatio}" +
             $"RotationValue = {RotationValue}";
    }
  }
}  // namespace Android.XR.InteractionFramework
