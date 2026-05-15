// Copyright 2026 Google LLC
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

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Android.XR.InteractionFramework.Editor.Sample {
  /// <summary>
  /// Editor script within the sample to verify dependencies once imported.
  /// Runs on editor load/script compilation after the sample is in the Assets folder.
  /// </summary>
  [InitializeOnLoad]
  public class AxrifSampleDependencyProcessor {
    /// <summary>
    /// Key GUIDs to search for to ensure the sample scene will run.
    /// Maps from asset GUID to a description of the asset.
    /// </summary>
    private static readonly Dictionary<string, string> RequiredAssets =
        new Dictionary<string, string>() {
          { "d56cf1aacbea4704cac4d7983df1a0ad", "Arrow.prefab (GameObject)" },
          { "7cf3ac7bcd2e5471fb54d5f5da9aad56", "Cube.fbx (GameObject)" },
          { "f5afcb7f49afad5419049090809f5cc7", "Cube.prefab (GameObject)" },
          { "bf65382e5e6d14e7f8140e4204ce07e2", "Cylinder.fbx (GameObject)" },
          { "0ad34abafad169848a38072baa96cdb2", "XRGrabInteractable.cs (MonoScript)" },
        };

    static AxrifSampleDependencyProcessor() {
      EditorApplication.delayCall += RunDependencyCheck;
    }

    private static void RunDependencyCheck() {
      CheckSampleDependencies();
    }

    /// <summary>
    /// Verifies that all required sample dependencies are present in the project.
    /// Logs an error for any missing dependencies.
    /// </summary>
    private static void CheckSampleDependencies() {
      if (RequiredAssets.Count == 0) {
        return;
      }
      int found = 0;
      foreach (var requiredAsset in RequiredAssets) {
        string guid = requiredAsset.Key;
        string description = requiredAsset.Value;
        if (string.IsNullOrEmpty(guid))
          continue;

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
          Debug.LogErrorFormat(
              "[AxrifSampleDependencyProcessor:CheckSampleDependencies] Missing required asset " +
                  "for AXRIF sample: {0} (GUID: {1}). Please ensure the necessary packages and " +
                  "their samples are installed.",
              description, guid);
        } else {
          ++found;
        }
      }
      if (found < RequiredAssets.Count) {
        Debug.LogError(
            "[AxrifSampleDependencyProcessor:CheckSampleDependencies] Some required assets for " +
            "the AXRIF sample are missing. Please be sure the prerequisite packages and assets " +
            "are installed.");
      }
    }
  }
}  // namespace Android.XR.InteractionFramework.Editor.Sample
#endif
