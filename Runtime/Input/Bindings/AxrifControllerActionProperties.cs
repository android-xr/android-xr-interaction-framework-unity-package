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
  /// A convenience and container class for InputActionProperties associated with the controller
  /// input source
  /// </summary>
  [Serializable]
  public class AxrifControllerActionProperties {
    [Tooltip("Whether or not the controller is tracked")]
    public InputActionProperty trackingState;

    [Tooltip("The pose of the controller")]
    public AxrifPoseActionProperties aimPose;

    [Tooltip("The primary action of the controller")]
    public InputActionProperty primary;

    [Tooltip("The secondary action of the controller")]
    public InputActionProperty secondary;

    [Tooltip("The trigger value of the controller")]
    public InputActionProperty trigger;

    [Tooltip("The thumbstick vector of the controller")]
    public InputActionProperty thumbstick;

    [Tooltip("The thumbstick press of the controller")]
    public InputActionProperty thumbstickPress;

    /// <summary>
    /// Enable all InputActionProperties
    /// </summary>
    public void EnableActions() {
      aimPose.EnableActions();
      primary.EnableDirectAction();
      secondary.EnableDirectAction();
      trigger.EnableDirectAction();
      thumbstick.EnableDirectAction();
      thumbstickPress.EnableDirectAction();
      trackingState.EnableDirectAction();
    }

    /// <summary>
    /// Disable all InputActionProperties
    /// </summary>
    public void DisableActions() {
      aimPose.DisableActions();
      primary.DisableDirectAction();
      secondary.DisableDirectAction();
      trigger.DisableDirectAction();
      thumbstick.DisableDirectAction();
      thumbstickPress.DisableDirectAction();
      trackingState.DisableDirectAction();
    }
  }
}  // namespace Android.XR.InteractionFramework
