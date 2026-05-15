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
  /// Retrieves eye data from InputActions and updates the source state in the AxrifSubsystem.
  /// </summary>
  public class AxrifEyeInputSource : AxrifInputSource {
    /// <summary>
    /// A reference to the eye InputActions
    /// </summary>
    private AxrifPoseActionProperties _gazePose;

    /// <summary>
    /// A reference to the pose tracking state of gaze.
    /// </summary>
    private InputActionProperty _gazePoseTrackingState;

    /// <summary>
    /// Creates a new AxrifEyeInputSource with the given gaze pose actions.
    /// </summary>
    /// <param name="gazePose">The gaze pose action properties to use.</param>
    /// <param name="gazePoseTrackingState">The gaze tracking state action property to use.</param>
    public AxrifEyeInputSource(AxrifPoseActionProperties gazePose,
                               InputActionProperty gazePoseTrackingState) {
      _gazePose = gazePose;
      _gazePoseTrackingState = gazePoseTrackingState;
    }

    /// <summary>
    /// Uses the Axrif configuration to determine if the input source is active
    /// </summary>
    /// <param name="configurations">The new Axrif Configuration</param>
    public override void UpdateConfiguration(Configurations configurations) {
      _isActive = configurations.IsGazePinchEnabled;
    }

    /// <summary>
    /// Reads current values from eye InputActions and updates the EyesSourceState in the
    /// AxrifSubSystem
    /// </summary>
    /// <param name="axrifSubsystem">A reference to the AxrifSubsystem</param>
    public override void UpdateSource(AxrifSubsystem axrifSubsystem) {
      var pose = new Pose(_gazePose.position.action.ReadValue<Vector3>(),
                          _gazePose.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref pose);

      var eyeSourceState = new EyesSourceState() {
        IsValid = IsPoseStateFullyTracked(_gazePoseTrackingState),
        AimPose = pose.ToPosef(),
      };
      axrifSubsystem.UpdateEyesSourceState(eyeSourceState);
    }

    /// <summary>
    /// Enables the eye pose position and rotation InputActions
    /// </summary>
    protected override void EnableInputActions() {
      _gazePose.EnableActions();
      _gazePoseTrackingState.EnableDirectAction();
    }

    /// <summary>
    /// Disables the eye pose position and rotation InputActions
    /// </summary>
    protected override void DisableInputActions() {
      _gazePose.DisableActions();
      _gazePoseTrackingState.DisableDirectAction();
    }
  }
}  // namespace Android.XR.InteractionFramework
