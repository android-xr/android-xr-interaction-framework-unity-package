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
  /// Represents a 3-dimensional vector.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Vector3f : IEquatable<Vector3f>,
                           IEquatable<Vector3>,
                           IEquatable<System.Numerics.Vector3> {
    public float X;
    public float Y;
    public float Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3f(float x = 0f, float y = 0f, float z = 0f) {
      X = x;
      Y = y;
      Z = z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3f(Vector3f v) {
      X = v.X;
      Y = v.Y;
      Z = v.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3f(Vector3 v) {
      X = v.x;
      Y = v.y;
      Z = -v.z;
    }

    public static Vector3f zero => new Vector3f(0f, 0f, 0f);

    public Vector3 ToVector3() => new Vector3(X, Y, -Z);

    public bool Equals(Vector3f other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

    public bool Equals(Vector3 other) =>
        X.Equals(other.x) && Y.Equals(other.y) && Z.Equals(-other.z);

    public bool Equals(System.Numerics.Vector3 other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(-other.Z);

    public override string ToString() => $"({X}, {Y}, {Z})";
  }
}  // namespace Android.XR.InteractionFramework