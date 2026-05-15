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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Android.XR.InteractionFramework.Editor {
  /// <summary>
  /// Class to add Axrif prefabs via the Unity Editor menu.
  /// </summary>
  public class AxrifMenuItems {
    private const string AxrifOriginName = "Axrif XR Origin.prefab";
    private const string AxrifInteractionManager = "Axrif Interaction Manager.prefab";
    private const string AxrifDebugPanel = "Axrif Debug Panel.prefab";
    private const string AxrifLeftGazePinchInteractor = "Axrif Left GazePinch Interactor.prefab";
    private const string AxrifRightGazePinchInteractor = "Axrif Right GazePinch Interactor.prefab";
    private const string AxrifLeftInteractor = "Axrif Left Interactor.prefab";
    private const string AxrifRightInteractor = "Axrif Right Interactor.prefab";
    private const string AxrifLeftPokeInteractor = "Axrif Left Poke Interactor.prefab";
    private const string AxrifRightPokeInteractor = "Axrif Right Poke Interactor.prefab";
    private const string AxrifMouseInteractor = "Axrif Mouse Interactor.prefab";
    private const string AxrifGazeInteractor = "Axrif Gaze Interactor.prefab";

    /// <summary>Gets the root path of the package.</summary>
    /// <returns>The path to the package root.</returns>
    private static string GetPackageRoot() {
      string[] guids = AssetDatabase.FindAssets("t:Script AxrifMenuItems");
      if (guids.Length > 0) {
        string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(scriptPath);
        if (info != null) {
          return info.assetPath;
        }
      }
      Debug.LogError("Failed to resolve package root.");
      return null;
    }

    /// <summary>Gets the full asset path for a given prefab name.</summary>
    /// <param name="prefabName">The name of the prefab file.</param>
    /// <returns>The full path to the prefab asset.</returns>
    private static string GetPrefabPath(string prefabName) {
      string prefabPath = Path.Combine("Prefabs", prefabName);
      string root = GetPackageRoot();
      if (string.IsNullOrEmpty(root)) {
        return null;
      }
      return Path.Combine(root, prefabPath).Replace('\\', '/');
    }

    /// <summary>Instantiates a prefab in the current scene from the given asset path.</summary>
    /// <param name="prefabName">The name of the Prefab Asset.</param>
    private static void CreatePrefab(string prefabName) {
      string path = GetPrefabPath(prefabName);
      if (path == null) {
        Debug.LogErrorFormat(
            "[AxrifMenuItems:CreatePrefab] could not find valid prefab path for {0}", prefabName);
        return;
      }
      GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

      if (prefab != null) {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (Selection.activeGameObject != null) {
          GameObjectUtility.SetParentAndAlign(instance, Selection.activeGameObject);
        }

        Undo.RegisterCreatedObjectUndo(instance, "Spawn " + instance.name);
        Selection.activeObject = instance;
      } else {
        Debug.LogErrorFormat("[AxrifMenuItems:CreatePrefab] could not find prefab at {0}", path);
      }
    }

    /// <summary>Creates the Axrif Origin prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Origin", false, 10)]
    static void CreateAxrifOrigin() {
      CreatePrefab(AxrifOriginName);
    }

    /// <summary>Creates the Axrif Interaction Manager prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Interaction Manager", false, 10)]
    static void CreateAxrifInteractionManager() {
      CreatePrefab(AxrifInteractionManager);
    }

    /// <summary>Creates the Axrif Debug Panel prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Debug Panel", false, 50)]
    static void CreateAxrifDebugPanel() {
      CreatePrefab(AxrifDebugPanel);
    }

    /// <summary>Creates the Axrif Left Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Left Interactor", false, 30)]
    static void CreateAxrifLeftInteractor() {
      CreatePrefab(AxrifLeftInteractor);
    }

    /// <summary>Creates the Axrif Right Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Right Interactor", false, 30)]
    static void CreateAxrifRightInteractor() {
      CreatePrefab(AxrifRightInteractor);
    }

    /// <summary>Creates the Axrif Gaze Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Gaze Interactor", false, 30)]
    static void CreateAxrifGazeInteractor() {
      CreatePrefab(AxrifGazeInteractor);
    }

    /// <summary>Creates the Axrif Left Gaze Pinch Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Left Gaze Pinch Interactor", false, 30)]
    static void CreateAxrifLeftGazePinchInteractor() {
      CreatePrefab(AxrifLeftGazePinchInteractor);
    }

    /// <summary>Creates the Axrif Right Gaze Pinch Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Right Gaze Pinch Interactor", false, 30)]
    static void CreateAxrifRightGazePinchInteractor() {
      CreatePrefab(AxrifRightGazePinchInteractor);
    }

    /// <summary>Creates the Axrif Left Poke Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Left Poke Interactor", false, 30)]
    static void CreateAxrifLeftPokeInteractor() {
      CreatePrefab(AxrifLeftPokeInteractor);
    }

    /// <summary>Creates the Axrif Right Poke Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Right Poke Interactor", false, 30)]
    static void CreateAxrifRightPokeInteractor() {
      CreatePrefab(AxrifRightPokeInteractor);
    }

    /// <summary>Creates the Axrif Mouse Interactor prefab in the scene.</summary>
    [MenuItem("GameObject/XR/AXRIF/Axrif Mouse Interactor", false, 30)]
    static void CreateAxrifMouseInteractor() {
      CreatePrefab(AxrifMouseInteractor);
    }
  }
}  // namespace Android.XR.InteractionFramework.Editor
