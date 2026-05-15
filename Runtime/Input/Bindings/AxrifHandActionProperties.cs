// Copyright 2026 Google LLC
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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// A convenience and container class for InputActionProperties associated with the hand input
  /// source
  /// </summary>
  [Serializable]
  public class AxrifHandActionProperties {
    [Tooltip("The pose state of the hand.")]
    public InputActionProperty poseState;
    [Tooltip("The activation value of the hand.")]
    public InputActionProperty aimActivateValue;

    [Tooltip("The pinch value of the hand.")]
    public InputActionProperty pinchValue;

    [Tooltip("The aim pose of the hand.")]
    public AxrifPoseActionProperties aimPose;

    [Tooltip("The grip pose of the hand.")]
    public AxrifPoseActionProperties gripPose;

    [Tooltip("The poke pose of the hand.")]
    public AxrifPoseActionProperties pokePose;

    [Tooltip("The pinch pose of the hand.")]
    public AxrifPoseActionProperties pinchPose;

    /// <summary>
    /// Enable all InputActionProperties
    /// </summary>
    public void EnableActions() {
      poseState.EnableDirectAction();
      aimActivateValue.EnableDirectAction();
      pinchValue.EnableDirectAction();
      aimPose.EnableActions();
      gripPose.EnableActions();
      pokePose.EnableActions();
      pinchPose.EnableActions();
    }

    /// <summary>
    /// Disable all InputActionProperties
    /// </summary>
    public void DisableActions() {
      poseState.DisableDirectAction();
      aimActivateValue.DisableDirectAction();
      pinchValue.DisableDirectAction();
      aimPose.DisableActions();
      gripPose.DisableActions();
      pokePose.DisableActions();
      pinchPose.DisableActions();
    }
  }
}  // namespace Android.XR.InteractionFramework
