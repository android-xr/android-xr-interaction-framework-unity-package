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
    /// Adds a collider to the scene, or updates an existing collider with the same id.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="collider">
    /// The collider to add or update.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_SetCollider(IntPtr context, in Collider collider);

    /// <summary>
    /// Removes a collider from the scene.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="id">
    /// The unique id of the collider to remove.
    /// </param>
    [DllImport(kLibraryName, CharSet = CharSet.Auto)]
    public static extern void Axrif_RemoveCollider(IntPtr context, int collider_id);

    /// <summary>
    /// Clears all previously added colliders, leaving an empty scene.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_ClearAllColliders(IntPtr context);
  }
}  // Android.XR.InteractionFramework
