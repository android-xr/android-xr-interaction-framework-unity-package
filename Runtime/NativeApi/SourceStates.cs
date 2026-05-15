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
    /// Updates the state of the 6DoF controllers input source.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="controllersSourceState">
    /// The new state of the controllers input source.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_UpdateControllersSourceState(
        IntPtr context, in ControllersSourceState controllersSourceState);

    /// <summary>
    /// Updates the state of the eyes input source.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="eyesSourceState">
    /// The new state of the eyes input source.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_UpdateEyesSourceState(IntPtr context,
                                                          in EyesSourceState eyesSourceState);

    /// <summary>
    /// Updates the state of the hands input source.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="handsSourceState">
    /// The new state of the hands input source.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_UpdateHandsSourceState(IntPtr context,
                                                           in HandsSourceState handsSourceState);

    /// <summary>
    /// Updates the state of the head input source.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="headSourceState">
    /// The new state of the head input source.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_UpdateHeadSourceState(IntPtr context,
                                                          in HeadSourceState headSourceState);

    /// <summary>
    /// Updates the state of the mouse input source.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="mouseSourceState">
    /// The new state of the mouse input source.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_UpdateMouseSourceState(IntPtr context,
                                                           in MouseSourceState mouseSourceState);
  }
}  // Android.XR.InteractionFramework