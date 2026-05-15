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
  /// Represents the state of the head input source for a given frame.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct HeadSourceState {
    /// <summary>
    /// Indicates whether the head source state is valid.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    /// <summary>
    /// The 6-dof pose of the head. Can be used to cast a ray toward
    /// potential interactable elements.
    /// </summary>
    public Android.XR.InteractionFramework.Posef Pose;
  }
}  // namespace Android.XR.InteractionFramework
