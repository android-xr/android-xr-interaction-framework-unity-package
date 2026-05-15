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
  /// Represents a snapshot of the pointers for a given interaction type.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  public struct PointerEvent {
    /// <summary>
    /// Indicates whether the <c>PointerEvent</c> is valid. A
    /// <c>PointerEvent</c> is valid only if it contains at least one valid
    /// pointer.
    /// </summary>
    [MarshalAs(UnmanagedType.I1)]
    public bool IsValid;

    /// <summary>
    /// The number of valid pointers in the <c>Pointers</c> array.
    /// </summary>
    public sbyte ValidPointerCount;

    /// <summary>
    /// A valid pointer event may contain up to 10 valid pointers. Each
    /// subsequent pointer returned requires that all previous pointers are
    /// valid (but they may be inactive). The <c>ValidPointerCount</c> field
    /// indicates the number of valid pointers in the <c>Pointers</c> array.
    /// There is no guarantee on the order of the pointers returned in a
    /// <c>PointerEvent</c>. The individual pointer types must be checked on
    /// each usage.
    ///
    /// Note: This manual array management is necessary to allow for type
    /// marshalling across interface boundaries. Special care must be taken when
    /// reading and writing pointers to avoid out of bounds access.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public Android.XR.InteractionFramework.Pointer[] Pointers;

    public override string ToString() {
      var pointers = string.Empty;
      for (var index = 0; index < ValidPointerCount; index++) {
        var pointer = Pointers[index];
        pointers += $"{pointer}, ";
      }

      return $"IsValid = {IsValid}, " + $"ValidPointerCount = {ValidPointerCount}, " +
             $"Pointers = {pointers}";
    }
  }
}  // namespace Android.XR.InteractionFramework
