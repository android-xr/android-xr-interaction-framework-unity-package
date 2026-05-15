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
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Retrieves head data from InputActions and updates the source state in the AxrifSubsystem.
  /// </summary>
  public class AxrifHeadInputSource : AxrifInputSource {
    /// <summary>
    /// A reference to the head InputActions.
    /// </summary>
    private AxrifPoseActionProperties _head;

    /// <summary>
    /// A reference to the pose tracking state of Head.
    /// </summary>
    private InputActionProperty _headTrackingState;

    /// <summary>
    /// Creates a new AxrifHeadInputSource with the given head pose actions.
    /// </summary>
    /// <param name="head">The head action properties to use.</param>
    /// <param name="headTrackingState">The head tracking state action property to use.</param>
    public AxrifHeadInputSource(AxrifPoseActionProperties head,
                                InputActionProperty headTrackingState) {
      _head = head;
      _headTrackingState = headTrackingState;
    }

    /// <summary>
    /// Uses the Axrif configuration to determine if the input source is active.
    /// </summary>
    /// <param name="configurations">The new Axrif Configuration</param>
    public override void UpdateConfiguration(Configurations configurations) {
      _isActive = configurations.IsGazePinchEnabled || configurations.IsHandRaycastEnabled;
    }

    /// <summary>
    /// Reads current values from head InputActions and updates the HeadSourceState in the
    /// AxrifSubSystem.
    /// </summary>
    /// <param name="axrifSubsystem">A reference to the AxrifSubsystem</param>
    public override void UpdateSource(AxrifSubsystem axrifSubsystem) {
      var pose = new Pose(_head.position.action.ReadValue<Vector3>(),
                          _head.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref pose);

      var headSourceState = new HeadSourceState() {
        IsValid = IsPoseStateFullyTracked(_headTrackingState),
        Pose = pose.ToPosef(),
      };
      axrifSubsystem.UpdateHeadSourceState(headSourceState);
    }

    /// <summary>
    /// Enables the position and rotation pose InputActions for head.
    /// </summary>
    protected override void EnableInputActions() {
      _head.EnableActions();
      _headTrackingState.EnableDirectAction();
    }

    /// <summary>
    /// Disables the position and rotation pose InputActions for head.
    /// </summary>
    protected override void DisableInputActions() {
      _head.DisableActions();
      _headTrackingState.DisableDirectAction();
    }
  }
}  // namespace Android.XR.InteractionFramework
