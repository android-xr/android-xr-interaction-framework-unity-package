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
using System.Collections.Generic;
using Unity.XR.Management.AndroidManifest.Editor;
using UnityEngine.XR.OpenXR;

namespace Android.XR.InteractionFramework.Editor {
  public class AxrifGlobalBuildHook : IAndroidManifestRequirementProvider {
    public ManifestRequirement ProvideManifestRequirement() {
      var manifestRequirement = new ManifestRequirement {
        SupportedXRLoaders = new HashSet<Type>() { typeof(OpenXRLoader) },

        NewElements =
            new List<ManifestElement>() {
              // <uses-native-library android:name="libopenxr.google.so" android:required="true" />
              new ManifestElement() {
                ElementPath = new List<string> { "manifest", "application", "uses-native-library" },
                Attributes = new Dictionary<string, string>() { { "name", "libopenxr.google.so" },
                                                                { "required", "true" } }
              },
              // <property android:name="android.window.PROPERTY_XR_ACTIVITY_START_MODE"
              // android:value="XR_ACTIVITY_START_MODE"/>
              new ManifestElement() {
                ElementPath =
                    new List<string> { "manifest", "application", "activity", "property" },
                Attributes =
                    new Dictionary<string, string>() {
                      { "name", "android.window.PROPERTY_XR_ACTIVITY_START_MODE" },
                      { "value", "XR_ACTIVITY_START_MODE_FULL_SPACE_UNMANAGED" }
                    }
              }
            },
      };

      return manifestRequirement;
    }
  }
}  // namespace Android.XR.InteractionFramework.Editor
