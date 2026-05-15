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
  /// A convenience class for a Pose containing InputActionProperties for a Vector3 position and
  /// Quaternion rotation
  /// </summary>
  [Serializable]
  public class AxrifPoseActionProperties {
    [Tooltip("Vector3 position input action")]
    public InputActionProperty position;

    [Tooltip("Quaternion rotation input action")]
    public InputActionProperty rotation;

    /// <summary>
    /// Enable all InputActionProperties
    /// </summary>
    public void EnableActions() {
      position.EnableDirectAction();
      rotation.EnableDirectAction();
    }

    /// <summary>
    /// Disable all InputActionProperties
    /// </summary>
    public void DisableActions() {
      position.DisableDirectAction();
      rotation.DisableDirectAction();
    }
  }
}  // namespace Android.XR.InteractionFramework
