// Copyright 2025 Google LLC
// Copyright Qualcomm Technologies, Inc. and/or its affiliates. All rights reserved.
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
  /// Tracked pose driver that behaves as Input System's TrackedPoseDriver but using Axrif Pointer
  /// event as data source.
  /// </summary>
  [AddComponentMenu("XR/AXRIF/Tracked Pose Driver (AndroidXR Pointer Events)")]
  public class TrackedPoseDriver : MonoBehaviour {
    public enum TrackingType { RotationAndPosition, RotationOnly, PositionOnly }

    [SerializeField]
    private TrackingType _trackingType = TrackingType.RotationAndPosition;

    [SerializeField]
    private PointerMask _pointerMask = PointerMask.None;

    [SerializeField]
    private Handedness _handedness = Handedness.Left;

    private AxrifSubsystem _subsystem;

    public void OnEnable() {
      if (_subsystem == null && !AxrifSubsystemUtils.TryGetSubsystem(out _subsystem)) {
        Debug.Log("[TrackedPoseDriver:OnEnable] No axrif subsystem");
        enabled = false;
        return;
      }

      _subsystem.onPointerEventUpdated += OnPointerEventUpdated;
    }

    public void OnDisable() {
      if (_subsystem != null) {
        _subsystem.onPointerEventUpdated -= OnPointerEventUpdated;
      }
    }

    private void OnPointerEventUpdated(PointerEvent pointers) {
      for (var index = 0; index < pointers.ValidPointerCount; index++) {
        var pointer = pointers.Pointers[index];
        var handedness = pointers.Pointers[index].Handedness;
        var flag = 1 << (sbyte)pointer.Type;
        if (_pointerMask.HasFlag((PointerMask)flag) && (handedness == _handedness)) {
          UpdateTracker(pointer);
        }
      }
    }

    public void UpdateTracker(Pointer pointer) {
      if (!pointer.IsValid) {
        Debug.LogWarning("[TrackedPoseDriver:UpdateTracker] Pointer is not valid");
        return;
      }

      if (!pointer.IsActive) {
        return;
      }

      if (_trackingType == TrackingType.RotationAndPosition ||
          _trackingType == TrackingType.PositionOnly) {
        transform.position = pointer.Line.Start.ToVector3();
      }

      if (_trackingType == TrackingType.RotationAndPosition ||
          _trackingType == TrackingType.RotationOnly) {
        Vector3 direction = pointer.Line.Direction.ToVector3();
        if (direction != Vector3.zero) {
          transform.rotation = Quaternion.LookRotation(direction);
        }
      }
    }
  }
}  // namespace Android.XR.InteractionFramework
