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
using UnityEditor.Build.Reporting;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace Android.XR.InteractionFramework.Editor {
  public class AxrifEyeGazeInteractionProfileBuildHook : OpenXRFeatureBuildHooks {
    public override int callbackOrder => 0;
    public override Type featureType => typeof(EyeGazeInteraction);

    protected override void OnPreprocessBuildExt(BuildReport report) {
      // Ignored
    }

    protected override void OnPostGenerateGradleAndroidProjectExt(string path) {
      // Ignored
    }

    protected override void OnPostprocessBuildExt(BuildReport report) {
      // Ignored
    }

    protected override ManifestRequirement ProvideManifestRequirementExt() {
      var manifestRequirement = new ManifestRequirement {
        SupportedXRLoaders = new HashSet<Type>() { typeof(OpenXRLoader) },
        NewElements =
            new List<ManifestElement>() {
              // <uses-permission android:name="android.permission.EYE_TRACKING_FINE" />
              new ManifestElement {
                ElementPath = new List<string> { "manifest", "uses-permission" },
                Attributes =
                    new Dictionary<string, string>() { { "name",
                                                         "android.permission.EYE_TRACKING_FINE" } }
              },
            }
      };

      return manifestRequirement;
    }
  }
}  // namespace Android.XR.InteractionFramework.Editor
