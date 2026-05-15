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

using UnityEngine;
using UnityEngine.InputSystem;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// A configuration object for the Axrif Eye Input Source
  /// </summary>
  [CreateAssetMenu(menuName = "AXRIF/Eye Input Source Asset")]
  public class AxrifEyeSourceAsset : ScriptableObject {
    [Tooltip("The pose tracking state of Eye Gaze.")]
    public InputActionProperty EyePoseTrackingState;

    [Tooltip("The Input Action Bindings to retrieve data for Eye.")]
    public AxrifPoseActionProperties EyeActions;

    /// <summary>
    /// Create a new AxrifEyeInputSource instance for retrieving Eye input data.
    /// </summary>
    /// <returns>The new AxrifEyeInputSource instance.</returns>
    public AxrifEyeInputSource CreateInstance() {
      return new AxrifEyeInputSource(EyeActions, EyePoseTrackingState);
    }
  }
}  // namespace Android.XR.InteractionFramework
