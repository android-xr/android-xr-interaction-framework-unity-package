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

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Retrieves controller data from InputActions and updates the source state in the
  /// AxrifSubsystem.
  /// </summary>
  public class AxrifControllerInputSource : AxrifInputSource {
    // <summary>
    // A reference to the left controller InputActions.
    // </summary>
    private AxrifControllerActionProperties _leftController;
    // <summary>
    // A reference to the right controller InputActions.
    // </summary>
    private AxrifControllerActionProperties _rightController;

    /// <summary>
    /// The current state of both controllers.
    /// </summary>
    private ControllersSourceState _controllerSourceState;

    /// <summary>
    /// Creates a new AxrifControllerInputSource with the given left and right controller actions.
    /// </summary>
    /// <param name="leftController">The left controller action properties to use.</param>
    /// <param name="rightController">The right controller action properties to use.</param>
    public AxrifControllerInputSource(AxrifControllerActionProperties leftController,
                                      AxrifControllerActionProperties rightController) {
      _leftController = leftController;
      _rightController = rightController;
    }

    /// <summary>
    /// Uses the Axrif configuration to determine if the input source is active.
    /// </summary>
    /// <param name="configurations">The new Axrif Configuration.</param>
    public override void UpdateConfiguration(Configurations configurations) {
      _isActive = configurations.IsControllerEnabled;
    }

    /// <summary>
    /// Reads current values from controller InputActions and updates the ControllerSourceState in
    /// the AxrifSubSystem.
    /// </summary>
    /// <param name="axrifSubsystem">A reference to the AxrifSubsystem.</param>
    public override void UpdateSource(AxrifSubsystem axrifSubsystem) {
      _controllerSourceState.IsValid = true;
      SetControllerState(_leftController, ref _controllerSourceState.LeftController);
      SetControllerState(_rightController, ref _controllerSourceState.RightController);
      axrifSubsystem.UpdateControllersSourceState(_controllerSourceState);
    }

    /// <summary>
    /// Enables the left and right controller InputActions.
    /// </summary>
    protected override void EnableInputActions() {
      _leftController.EnableActions();
      _rightController.EnableActions();
    }

    /// <summary>
    /// Disables the left and right controller InputActions.
    /// </summary>
    protected override void DisableInputActions() {
      _leftController.DisableActions();
      _rightController.DisableActions();
    }

    /// <summary>
    /// Sets a ControllerState given the provided controller action properties.
    /// </summary>
    /// <param name="controllerAction">The ControllerAction holder for the controller hand.</param>
    /// <param name="controllerState">A reference to the ControllerState to set.</param>
    private void SetControllerState(in AxrifControllerActionProperties controllerAction,
                                    ref ControllerState controllerState) {
      var aim = new Pose(controllerAction.aimPose.position.action.ReadValue<Vector3>(),
                        controllerAction.aimPose.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref aim);

      controllerState.IsActive = IsPoseStateFullyTracked(controllerAction.trackingState);
      controllerState.AimPose = aim.ToPosef();
      controllerState.IsPrimaryPressed = controllerAction.primary.action.IsPressed();
      controllerState.IsSecondaryPressed = controllerAction.secondary.action.IsPressed();
      controllerState.TriggerValue = controllerAction.trigger.action.ReadValue<float>();
      controllerState.ThumbstickXValue = controllerAction.thumbstick.action.ReadValue<Vector2>().x;
      controllerState.ThumbstickYValue = controllerAction.thumbstick.action.ReadValue<Vector2>().y;
      controllerState.IsThumbstickPressed = controllerAction.thumbstickPress.action.IsPressed();
    }
  }
}  // namespace Android.XR.InteractionFramework
