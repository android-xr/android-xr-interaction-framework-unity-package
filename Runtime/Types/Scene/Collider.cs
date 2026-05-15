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

using System;
using System.Runtime.InteropServices;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// A collider in the scene graph.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Collider : IEquatable<Collider> {
    /// <summary>
    /// Represents a type of collider that can be represented in the scene graph.
    /// </summary>
    public enum ColliderType : sbyte {
      /// <summary>
      /// A 2D rectangle in 3D space.
      /// </summary>
      Rect = 0,
      /// <summary>
      /// A 3D rectangular prism.
      /// </summary>
      Box = 1,
      /// <summary>
      /// A 3D sphere.
      /// </summary>
      Sphere = 2,
    }

    /// <summary>
    /// The unique identifier for this collider.
    /// </summary>
    public int Id;

    /// <summary>
    /// The type of the collider.
    /// </summary>
    public ColliderType Type;

    /// <summary>
    /// Indicates whether the collider can be interacted with directly via poke.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsTouchable;

    /// <summary>
    /// The center point of the collider.
    /// </summary>
    public Android.XR.InteractionFramework.Vector3f Center;

    /// <summary>
    /// The rotation applied to the collider, in the form of a unit quaternion.
    /// </summary>
    public Android.XR.InteractionFramework.Quaternionf Rotation;

    /// <summary>
    /// The length of the collider, along the local X axis.
    /// Required for colliders of type: Rect, Box
    /// </summary>
    public float Length;

    /// <summary>
    /// The height of the collider, along the local Y axis.
    /// Required for colliders of type: Rect, Box
    /// </summary>
    public float Height;

    /// <summary>
    /// The depth of the collider, along the local Z axis.
    /// Required for colliders of type: Box
    /// </summary>
    public float Depth;

    /// <summary>
    /// The radius of the collider.
    /// Required for colliders of type: Sphere
    /// </summary>
    public float Radius;

    public readonly bool Equals(Collider other) => Id == other.Id;

    public override bool Equals(object obj) {
      return obj is Collider other && Equals(other);
    }

    // Collider Ids should be unique.
    public override int GetHashCode() {
      return Id;
    }
  }
}  // namespace Android.XR.InteractionFramework
