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
using UnityEngine;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Contains information for configuring interaction modules and behaviors.
  /// </summary>
  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public struct Configurations {
    /// <summary>
    /// Indicates whether the Controller interaction mode is enabled.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    [Tooltip("Indicates whether the Controller interaction mode is enabled.")]
    public bool IsControllerEnabled;

    /// <summary>
    /// Indicates whether Gaze and Pinch interaction mode is enabled.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    [Tooltip("Indicates whether the Gaze and Pinch interaction mode is enabled.")]
    public bool IsGazePinchEnabled;

    /// <summary>
    /// Indicates whether to use Head for Gaze and Pinch interaction mode.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool UseHeadForGazeInteraction;

    /// <summary>
    /// Indicates whether the Hand Raycast interaction mode is enabled.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    [Tooltip("Indicates whether the Hand Raycast interaction mode is enabled.")]
    public bool IsHandRaycastEnabled;

    /// <summary>
    /// Indicates whether the Hand Poke interaction mode is enabled.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    [Tooltip("Indicates whether the Hand Poke interaction mode is enabled.")]
    public bool IsHandPokeEnabled;

    /// <summary>
    /// Indicates whether the Mouse interaction mode is enabled.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    [Tooltip("Indicates whether the Mouse interaction mode is enabled.")]
    public bool IsMouseEnabled;

    /// <summary>
    /// Indicates whether to use sensitive thresholds for Gaze and Pinch transitions.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    [Tooltip("Indicates whether to use sensitive thresholds for Gaze and Pinch transitions.")]
    public bool UseSensitiveTransitions;
  }
}  // namespace Android.XR.InteractionFramework
