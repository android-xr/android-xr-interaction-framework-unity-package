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

  public static partial class NativeApi {
    /// <summary>
    /// Returns a PointerEvent containing all active pointers for the currently selected interaction
    /// type for both hands, as determined by automatic transitions.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="timestampNs">
    /// The timestamp at which to get the PointerEvent, in nanoseconds. For the vast majority of use
    /// cases, this should be the current system time.
    /// </param>
    /// <param name="outPointerEvent">
    /// A pre-allocated PointerEvent to store the results in.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_GetSelectedPointerEvent(IntPtr context, long timestampNs,
                                                            out PointerEvent outPointerEvent);

    /// <summary>
    /// Returns a PointerEvent containing all active pointers.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="interactionType">
    /// The interaction type from which to get the PointerEvent.
    /// </param>
    /// <param name="timestampNs">
    /// The timestamp at which to get the PointerEvent, in nanoseconds. For the vast majority of use
    /// cases, this should be the current system time.
    /// </param>
    /// <param name="outPointerEvent">
    /// A pre-allocated PointerEvent to store the results in.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_GetPointerEvent(IntPtr context, InteractionType interactionType,
                                                    long timestampNs,
                                                    out PointerEvent outPointerEvent);
  }
}  // Android.XR.InteractionFramework
