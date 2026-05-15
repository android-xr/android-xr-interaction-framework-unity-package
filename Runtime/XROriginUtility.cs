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
using Unity.XR.CoreUtils;

namespace Android.XR.InteractionFramework {
  public static class XROriginUtility {
    private static XROrigin _sXROrigin;

    public static XROrigin FindXROrigin() {
#pragma warning disable CS0618  // Type or member is obsolete
      return _sXROrigin = _sXROrigin != null ? _sXROrigin : Object.FindObjectOfType<XROrigin>();
#pragma warning restore CS0618  // Type or member is obsolete
    }

    public static Camera GetOriginCamera() {
      if (_sXROrigin != null) {
        return _sXROrigin.Camera;
      }

      var camera = FindXROrigin()?.Camera;
      if (camera == null) {
        camera = Camera.main;
      }

      return camera;
    }

    public static Transform GetOriginTransform() {
      if (_sXROrigin != null) {
        return _sXROrigin.transform;
      }

      return FindXROrigin()?.transform;
    }

    public static Transform GetTrackablesParent() {
      if (_sXROrigin != null) {
        return _sXROrigin.TrackablesParent;
      }

      return FindXROrigin()?.TrackablesParent;
    }

    public static GameObject GetCameraFloorOffsetObject() {
      if (_sXROrigin != null) {
        return _sXROrigin.CameraFloorOffsetObject;
      }

      return FindXROrigin()?.CameraFloorOffsetObject;
    }

    /// <summary>
    /// Converts the XR Origin based position to World scene position
    /// </summary>
    /// <param name="point"> Position to transform </param>
    /// <param name="applyCameraOffset"> Should apply camera Y-offset? </param>
    public static void TransformPoint(ref Vector3 point, bool applyCameraOffset = true) {
      var origin = GetOriginTransform();
      if (origin == null) {
        return;
      }

      point = origin.TransformPoint(point);

      if (applyCameraOffset) {
        var cameraOffset = GetCameraFloorOffsetObject();
        if (cameraOffset != null) {
          point.y += cameraOffset.transform.position.y - origin.position.y;
        }
      }
    }

    /// <summary>
    /// Converts the XR Origin based vector to a world scene vector.
    /// </summary>
    /// <param name="vector">The Vector to transform </param>
    public static void TransformVector(ref Vector3 vector) {
      var origin = GetOriginTransform();
      if (origin == null) {
        return;
      }
      vector = origin.TransformVector(vector);
    }

    /// <summary>
    /// Converts XR Origin based pose to World scene pose
    /// </summary>
    /// <param name="pose"> Pose to transform </param>
    /// <param name="applyCameraOffset"> Should apply camera Y-offset? </param>
    public static void TransformPose(ref Pose pose, bool applyCameraOffset = true) {
      var origin = GetOriginTransform();
      if (origin == null) {
        return;
      }

      pose = origin.TransformPose(pose);

      if (applyCameraOffset) {
        var cameraOffset = GetCameraFloorOffsetObject();
        if (cameraOffset != null) {
          var originToCamera = origin.InverseTransformPose(cameraOffset.transform.GetWorldPose());
          pose = originToCamera.ApplyOffsetTo(pose);
        }
      }
    }
  }
}  // namespace Android.XR.InteractionFramework
