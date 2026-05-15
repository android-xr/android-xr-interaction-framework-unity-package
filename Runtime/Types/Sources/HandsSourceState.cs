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
  /// Represents the state of a single hand for a given frame.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct HandState {
    /// <summary>
    /// Indicates whether the hand is currently active.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsActive;

    /// <summary>
    /// The 6-dof pose of the pinch, as defined by the OpenXR EXT_hand_interaction
    /// spec.
    /// </summary>
    public Android.XR.InteractionFramework.Posef PinchPose;

    /// <summary>
    /// The 6-dof pose of the hand aim, as defined by the OpenXR EXT_hand_interaction
    /// spec.
    /// </summary>
    public Android.XR.InteractionFramework.Posef AimPose;

    /// <summary>
    /// The 6-dof pose of the poke, as defined by the OpenXR EXT_hand_interaction
    /// spec.
    /// </summary>
    public Android.XR.InteractionFramework.Posef PokePose;

    /// <summary>
    /// The 6-dof pose of the grip, as defined by the OpenXR EXT_hand_interaction
    /// spec.
    /// </summary>
    public Android.XR.InteractionFramework.Posef GripPose;

    /// <summary>
    /// The floating point value of the pinch gesture, as defined by the OpenXR
    /// EXT_hand_interaction spec.
    /// </summary>
    public float PinchValue;

    /// <summary>
    /// The floating point value of the aim activate gesture, as defined by the OpenXR
    /// EXT_hand_interaction spec.
    /// </summary>
    public float AimActivateValue;

    /// <summary>
    /// A vector representing the linear velocity of the hand's palm joint, as
    /// defined by the OpenXR XrHandJointVelocitiesEXT spec.
    /// </summary>
    public Android.XR.InteractionFramework.Vector3f PalmJointVelocity;
  }

  /// <summary>
  /// Represents the state of the hands input source for a given frame.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct HandsSourceState {
    /// <summary>
    /// Indicates whether the hands source state is valid.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    /// <summary>
    /// The current state of the left hand.
    /// </summary>
    public HandState LeftHand;

    /// <summary>
    /// The current state of the right hand.
    /// </summary>
    public HandState RightHand;
  }
}  // namespace Android.XR.InteractionFramework
