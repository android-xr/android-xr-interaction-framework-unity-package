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
    /// Creates an instance of the Interaction Framework with the provided `configurations`.
    /// </summary>
    /// <param name="configurations">
    /// The configurations used to initialize interaction modules.
    /// </param>
    /// <param name="out_context">
    /// A pointer to the context object pointer used to perform subsequent calls to Interaction
    /// Framework functions. `out_context` *must* point to nullptr when provided to this function,
    /// otherwise creation is skipped.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_Create(in Configurations configurations,
                                           ref IntPtr out_context);

    /// <summary>
    /// Updates the configurations used by the Interaction Framework. Note: This method may delay
    /// transitions, so it should not be called per-frame.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    /// <param name="configurations">
    /// The new configurations to use.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_UpdateConfigurations(IntPtr context,
                                                         in Configurations configurations);

    /// <summary>
    /// Cleans up memory and destroys the instance of the Interaction Framework. This method should
    /// always be called before the retained context pointer goes out of scope.
    /// </summary>
    /// <param name="context">
    /// The context handle used to identify the Interaction Framework instance.
    /// </param>
    [DllImport(kLibraryName)]
    public static extern void Axrif_Destroy(IntPtr context);
  }
}  // Android.XR.InteractionFramework
