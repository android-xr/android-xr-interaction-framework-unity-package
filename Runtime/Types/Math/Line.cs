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

using System.Runtime.InteropServices;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Represents a line with two endpoints.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct Line {
    /// <summary>
    /// The starting point of the line.
    /// </summary>
    public Android.XR.InteractionFramework.Vector3f Start;

    /// <summary>
    /// The end point of the line.
    /// </summary>
    public Android.XR.InteractionFramework.Vector3f End;

    /// <summary>
    /// The normalized direction vector of the line.
    /// </summary>
    public Android.XR.InteractionFramework.Vector3f Direction;

    /// <summary>
    /// Whether the line is intended to extend to infinity.
    /// </summary>
    public bool ToInfinity;

    public override string ToString() {
      return $"Start: {Start}, End: {End}, Direction: {Direction}";
    }
  }
}  // namespace Android.XR.InteractionFramework
