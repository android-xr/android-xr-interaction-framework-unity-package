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
  /// A configuration object for the Axrif Head Input Source.
  /// </summary>
  [CreateAssetMenu(menuName = "AXRIF/Head Input Source Asset")]
  public class AxrifHeadSourceAsset : ScriptableObject {
    [Tooltip("The pose tracking state of Head.")]
    public InputActionProperty HeadPoseTrackingState;

    [Tooltip("The Input Action Bindings to retrieve data for Head")]
    public AxrifPoseActionProperties HeadActions;

    /// <summary>
    /// Create a new AxrifHeadInputSource instance for retrieving head input data.
    /// </summary>
    /// <returns>The new AxrifHeadInputSource instance.</returns>
    public AxrifHeadInputSource CreateInstance() {
      return new AxrifHeadInputSource(HeadActions, HeadPoseTrackingState);
    }
  }
}  // namespace Android.XR.InteractionFramework
