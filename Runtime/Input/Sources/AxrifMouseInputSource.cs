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

using Android.XR.InteractionFramework.Extensions;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Retrieves mouse data from InputActions and updates the source state in the AxrifSubsystem.
  /// </summary>
  public class AxrifMouseInputSource : AxrifInputSource {
    /// <summary>
    /// A reference to mouse InputActions.
    /// </summary>
    private AxrifMouseActionProperties _mouseActions;

    /// <summary>
    /// Creates a new AxrifMouseInputSource with the given mouse actions.
    /// </summary>
    /// <param name="mouseActions">The mouse action properties to use.</param>
    public AxrifMouseInputSource(AxrifMouseActionProperties mouseActions) {
      _mouseActions = mouseActions;
    }

    /// <summary>
    /// Uses the Axrif configuration to determine if the input source is active
    /// </summary>
    /// <param name="configurations">The new Axrif Configuration</param>
    public override void UpdateConfiguration(Configurations configurations) {
      _isActive = configurations.IsMouseEnabled;
    }

    public override void UpdateSource(AxrifSubsystem axrifSubsystem) {
      var mousePoseState = _mouseActions.pose.action.ReadValue<PoseState>();
      var currentMousePose = new Pose(mousePoseState.position, mousePoseState.rotation);
      XROriginUtility.TransformPose(ref currentMousePose);

      var scroll = _mouseActions.scroll.action.ReadValue<Vector2>();
      var mouseSourceState = new MouseSourceState() {
        IsValid = IsPoseStateFullyTracked(_mouseActions.trackingState),
        IsPrimaryClicking = _mouseActions.primaryClick.action.ReadValue<float>() >= 1f,
        IsSecondaryClicking = _mouseActions.secondaryClick.action.ReadValue<float>() >= 1f,
        IsTertiaryClicking = _mouseActions.tertiaryClick.action.ReadValue<float>() >= 1f,
        AimPose = currentMousePose.ToPosef(),
        HorizontalScrollValue = scroll.x,
        VerticalScrollValue = scroll.y
      };

      axrifSubsystem.UpdateMouseSourceState(mouseSourceState);
    }

    /// <summary>
    /// Enables the mouse InputActions
    /// </summary>
    protected override void EnableInputActions() {
      _mouseActions.EnableActions();
    }

    /// <summary>
    /// Disables the mouse InputActions
    /// </summary>
    protected override void DisableInputActions() {
      _mouseActions.DisableActions();
    }
  }
}  // namespace Android.XR.InteractionFramework
