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

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// A configuration object for the Axrif Controller Input Source.
  /// </summary>
  [CreateAssetMenu(menuName = "AXRIF/Controller Input Source Asset")]
  public class AxrifControllerSourceAsset : ScriptableObject {
    [Tooltip("The Input Action Bindings to retrieve data for the Left Controller")]
    public AxrifControllerActionProperties LeftControllerActions;

    [Tooltip("The Input Action Bindings to retrieve data for the Right Controller")]
    public AxrifControllerActionProperties RightControllerActions;

    /// <summary>
    /// Create a new AxrifControllerInputSource instance for retrieving controller input data.
    /// </summary>
    /// <returns>The new AxrifControllerInputSource instance.</returns>
    public AxrifControllerInputSource CreateInstance() {
      return new AxrifControllerInputSource(LeftControllerActions, RightControllerActions);
    }
  }
}  // namespace Android.XR.InteractionFramework
