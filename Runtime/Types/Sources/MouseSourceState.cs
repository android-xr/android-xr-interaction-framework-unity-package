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
  /// Represents the state of the mouse input source for a given frame.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct MouseSourceState {
    /// <summary>
    /// Indicates whether the mouse source state is valid.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    /// <summary>
    /// The 6-dof pose of the mouse pointer aim. Can be used to cast a ray toward
    /// potential interactable elements. The position component of the pose is
    /// static, typically at the position of the user's head.
    /// </summary>
    public Android.XR.InteractionFramework.Posef AimPose;

    /// <summary>
    /// Indicates whether the primary button on the mouse is clicking.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsPrimaryClicking;

    /// <summary>
    /// Indicates whether the secondary button on the mouse is clicking.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsSecondaryClicking;

    /// <summary>
    /// Indicates whether the tertiary button on the mouse is clicking.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsTertiaryClicking;

    /// <summary>
    /// A value between -1 and 1 indicating the amount of scroll in the vertical
    /// direction.
    /// </summary>
    public float VerticalScrollValue;

    /// <summary>
    /// A value between -1 and 1 indicating the amount of scroll in the horizontal
    /// direction.
    /// </summary>
    public float HorizontalScrollValue;
  }
}  // namespace Android.XR.InteractionFramework
