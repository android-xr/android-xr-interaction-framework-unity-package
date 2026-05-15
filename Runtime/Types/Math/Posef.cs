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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.XR.InteractionFramework.Extensions;
using UnityEngine;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// A 6-dof rigid transform comprising position and rotation.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Posef : IEquatable<Posef>, IEquatable<Pose> {
    /// <summary>
    /// 3-dof position component.
    /// </summary>
    public Android.XR.InteractionFramework.Vector3f Position;

    /// <summary>
    /// 3-dof rotation component.
    /// </summary>
    public Android.XR.InteractionFramework.Quaternionf Rotation;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Posef(Vector3f position, Quaternionf rotation) {
      Position = position;
      Rotation = rotation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Posef(Posef pose) {
      Position = pose.Position;
      Rotation = pose.Rotation;
    }

    public static Posef identity => new Posef(Vector3f.zero, Quaternionf.identity);

    public Pose ToPose() => new Pose(Position.ToVector3(), Rotation.ToQuaternion());

    public static Posef FromPose(Pose pose) => new Posef(pose.position.ToVector3f(),
                                                         pose.rotation.ToQuaternionf());

    public bool Equals(Posef other) =>
        Position.Equals(other.Position) && Rotation.Equals(other.Rotation);

    public bool Equals(Pose other) =>
        Position.Equals(other.position) && Rotation.Equals(other.rotation);

    public override string ToString() {
      return $"(position : {Position}, rotation: {Rotation})";
    }
  }
}  // namespace Android.XR.InteractionFramework
