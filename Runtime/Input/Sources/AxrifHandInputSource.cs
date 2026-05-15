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

using System.Collections.Generic;
using Android.XR.InteractionFramework.Extensions;
using UnityEngine;
using UnityEngine.XR.Hands;

using Hand = UnityEngine.XR.Hands.Handedness;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Retrieves hand data from InputActions and updates the source state in the AxrifSubsystem.
  /// </summary>
  public class AxrifHandInputSource : AxrifInputSource {
    /// <summary>
    /// A reference to the left hand InputActions.
    /// </summary>
    private AxrifHandActionProperties _leftHand;

    /// <summary>
    /// A reference to the right hand InputActions.
    /// </summary>
    private AxrifHandActionProperties _rightHand;

    /// <summary>
    /// A reference to the active XRHandSubystem.
    /// </summary>
    private XRHandSubsystem _handSubsystem;

    /// <summary>
    /// The current state of both hands.
    /// </summary>
    private HandsSourceState _handSourceState;

    /// <summary>
    /// Creates a new AxrifHandInputSource with the given left and right hand actions.
    /// </summary>
    /// <param name="leftHand">The left hand action properties to use.</param>
    /// <param name="rightHand">The right hand action properties to use.</param>
    public AxrifHandInputSource(AxrifHandActionProperties leftHand,
                                AxrifHandActionProperties rightHand) {
      _leftHand = leftHand;
      _rightHand = rightHand;
      _handSourceState = new HandsSourceState();
    }

    /// <summary>
    /// Uses the Axrif configuration to determine if the input source is active.
    /// </summary>
    /// <param name="configurations">The new Axrif Configuration</param>
    public override void UpdateConfiguration(Configurations configurations) {
      _isActive = configurations.IsGazePinchEnabled || configurations.IsHandPokeEnabled ||
                  configurations.IsHandRaycastEnabled || configurations.IsMouseEnabled;
    }

    /// <summary>
    /// Reads current values from hand InputActions and updates the HandsSourceState in the
    /// AxrifSubSystem.
    /// </summary>
    /// <param name="axrifSubsystem">A reference to the AxrifSubsystem</param>
    public override void UpdateSource(AxrifSubsystem axrifSubsystem) {
      SetHandState(_leftHand, ref _handSourceState.LeftHand, Hand.Left);
      SetHandState(_rightHand, ref _handSourceState.RightHand, Hand.Right);
      _handSourceState.IsValid = true;
      axrifSubsystem.UpdateHandsSourceState(_handSourceState);
    }

    /// <summary>
    /// Enables the left and right hand InputActions.
    /// </summary>
    protected override void EnableInputActions() {
      _leftHand.EnableActions();
      _rightHand.EnableActions();
      // Get a reference to the active XRHandSubsystem.
      var handSubsystems = new List<XRHandSubsystem>();
      SubsystemManager.GetSubsystems(handSubsystems);
      foreach (var handSubsystem in handSubsystems)
        if (handSubsystem.running) {
          _handSubsystem = handSubsystem;
          return;
        }
    }

    /// <summary>
    /// Disables the left and right hand InputActions.
    /// </summary>
    protected override void DisableInputActions() {
      _leftHand.DisableActions();
      _rightHand.DisableActions();
      _handSubsystem = null;
    }

    /// <summary>
    /// Sets a HandState given the provided hand action properties.
    /// </summary>
    /// <param name="handAction">The InputAction holder for the provided hand</param>
    /// <param name="handState">A reference to the HandState to set</param>
    /// <param name="hand">Either the left or right hand. Used in determining the palm joint's
    /// linear velocity.</param>
    private void SetHandState(in AxrifHandActionProperties handAction, ref HandState handState,
                              Hand hand) {
      var aim = new Pose(handAction.aimPose.position.action.ReadValue<Vector3>(),
                         handAction.aimPose.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref aim);

      var grip = new Pose(handAction.gripPose.position.action.ReadValue<Vector3>(),
                          handAction.gripPose.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref grip);

      var poke = new Pose(handAction.pokePose.position.action.ReadValue<Vector3>(),
                          handAction.pokePose.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref poke);

      var pinch = new Pose(handAction.pinchPose.position.action.ReadValue<Vector3>(),
                           handAction.pinchPose.rotation.action.ReadValue<Quaternion>());
      XROriginUtility.TransformPose(ref pinch);
      handState.IsActive = IsPoseStateFullyTracked(handAction.poseState);
      handState.AimActivateValue = handAction.aimActivateValue.action.ReadValue<float>();
      handState.PinchValue = handAction.pinchValue.action.ReadValue<float>();
      handState.AimPose = aim.ToPosef();
      handState.GripPose = grip.ToPosef();
      handState.PokePose = poke.ToPosef();
      handState.PinchPose = pinch.ToPosef();
      handState.PalmJointVelocity = GetPalmVelocity(hand).ToVector3f();
    }

    /// <summary>
    /// Uses the XRHandSubsystem to get the linear velocity of the palm joint.
    /// </summary>
    /// <param name="hand">The hand to return the palm joint's velocity from.</param>
    /// <returns>The palm velocity if the hand is
    /// tracked and its palm joint is tracking linear velocity. Otherwise, returns
    /// Vector3.zero.</returns>
    private Vector3 GetPalmVelocity(Hand hand) {
      if (_handSubsystem == null || hand == Hand.Invalid) {
        return Vector3.zero;
      }
      XRHand xrHand = hand == Hand.Left ? _handSubsystem.leftHand : _handSubsystem.rightHand;
      if (xrHand.isTracked) {
        var palm = xrHand.GetJoint(XRHandJointID.Palm);
        if (palm.trackingState.HasFlag(XRHandJointTrackingState.LinearVelocity)) {
          if (palm.TryGetLinearVelocity(out Vector3 velocity)) {
            XROriginUtility.TransformVector(ref velocity);
            return velocity;
          }
        }
      }
      return Vector3.zero;
    }
  }
}  // namespace Android.XR.InteractionFramework
