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
  /// A convenience and container class for InputActionProperties associated with the mouse input
  /// source
  /// </summary>
  [Serializable]
  public class AxrifMouseActionProperties {
    [Tooltip("The tracking state of the mouse.")]
    public InputActionProperty trackingState;
    [Tooltip("The pose of the mouse.")]
    public InputActionProperty pose;
    [Tooltip("The mouse's primary click.")]
    public InputActionProperty primaryClick;

    [Tooltip("The mouse's secondary click.")]
    public InputActionProperty secondaryClick;

    [Tooltip("The mouse's tertiary click.")]
    public InputActionProperty tertiaryClick;

    [Tooltip("The mouse's scroll value.")]
    public InputActionProperty scroll;

    /// <summary>
    /// Enable all InputActionProperties
    /// </summary>
    public void EnableActions() {
      trackingState.EnableDirectAction();
      pose.EnableDirectAction();
      primaryClick.EnableDirectAction();
      secondaryClick.EnableDirectAction();
      tertiaryClick.EnableDirectAction();
      scroll.EnableDirectAction();
    }

    /// <summary>
    /// Disable all InputActionProperties
    /// </summary>
    public void DisableActions() {
      trackingState.DisableDirectAction();
      pose.DisableDirectAction();
      primaryClick.DisableDirectAction();
      secondaryClick.DisableDirectAction();
      tertiaryClick.DisableDirectAction();
      scroll.DisableDirectAction();
    }
  }
}  // namespace Android.XR.InteractionFramework
