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
using UnityEngine;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Represents a quaternion used to describe 3D rotation.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Quaternionf : IEquatable<Quaternionf>,
                              IEquatable<Quaternion>,
                              IEquatable<System.Numerics.Quaternion> {
    public float X;
    public float Y;
    public float Z;
    public float W;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternionf(float x = 0f, float y = 0f, float z = 0f, float w = 1f) {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternionf(Quaternionf q) {
      X = q.X;
      Y = q.Y;
      Z = q.Z;
      W = q.W;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Quaternionf(Quaternion q) {
      X = -q.x;
      Y = -q.y;
      Z = q.z;
      W = q.w;
    }

    public static Quaternionf identity => new Quaternionf(0f, 0f, 0f, 1f);

    public Quaternion ToQuaternion() => new Quaternion(-X, -Y, Z, W);

    public bool Equals(Quaternionf other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    public bool Equals(Quaternion other) =>
        X.Equals(-other.x) && Y.Equals(-other.y) && Z.Equals(other.z) && W.Equals(other.w);

    public bool Equals(System.Numerics.Quaternion other) =>
        X.Equals(-other.X) && Y.Equals(-other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    public override string ToString() => $"({X}, {Y}, {Z}, {W})";
  }
}  // namespace Android.XR.InteractionFramework
