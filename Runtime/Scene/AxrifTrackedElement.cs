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

using System.Collections;
using UnityEngine;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// A utility class that registers UI Canvases or Colliders as Axrif colliders in the
  /// AxrifSceneIntegrator. This can be useful for objects with colliders that are not used with XRI
  /// Interactables and UI Canvases that are loaded at runtime.
  /// </summary>
  public class AxrifTrackedElement : MonoBehaviour {
    /// <summary>
    /// A reference to the AxrifSceneIntegrator for registering and unregistering this object as an
    /// axrif collider.
    /// </summary>
    private AxrifSceneIntegrator _sceneIntegrator;

    private void Awake() {
      if (!HasValidColliderComponent()) {
        enabled = false;
        return;
      }
      _sceneIntegrator = FindFirstObjectByType<AxrifSceneIntegrator>();
      if (!_sceneIntegrator) {
        Debug.LogWarning("[AxrifTrackedElement:Awake] Failed to find the AxrifSceneIntegrator.");
        enabled = false;
      }
    }

    /// <summary>
    /// Checks in editor if the GameObject has at least one of the required Unity Components to be
    /// registered with the AxrifSceneIntegrator.
    /// </summary>
    private void Reset() {
      HasValidColliderComponent();
    }

    /// <summary>
    /// When enabled, attempts to add any attached Unity Colliders, Unity Canvases, or both to the
    /// AxrifSceneIntegrator as Axrif colliders.
    /// </summary>
    private void OnEnable() {
      if (_sceneIntegrator == null) {
        return;
      }
      StartCoroutine(Register());
    }

    /// <summary>
    /// When disabled, attempts to remove registered Axrif colliders from the
    /// AxrifSceneIntegrator associated with attached and supported Unity Components.
    /// </summary>
    private void OnDisable() {
      if (_sceneIntegrator == null) {
        return;
      }
      if (TryGetComponent<UnityEngine.Collider>(out UnityEngine.Collider collider)) {
        _sceneIntegrator.UnregisterCollider(collider);
      }

      if (TryGetComponent(out Canvas canvas)) {
        _sceneIntegrator.UnregisterCanvas(canvas);
      }
    }

    /// <summary>
    /// Checks if the GameObject currently has a Unity canvas and/or a Unity collider.
    /// </summary>
    /// <returns></returns>
    private bool HasValidColliderComponent() {
      bool isValid = TryGetComponent<UnityEngine.Collider>(out UnityEngine.Collider _) ||
                     TryGetComponent(out Canvas _);
      if (!isValid) {
        Debug.LogWarning(
            "[AxrifTrackedElement:HasValidColliderComponent] Failed to find valid Collider " +
            "or Canvas to register.");
      }
      return isValid;
    }

    /// <summary>
    /// Registers the tracked collider.
    /// </summary>
    private IEnumerator Register() {
      yield return new WaitForEndOfFrame();
      if (!enabled || _sceneIntegrator == null) {
        yield break;
      }

      while (_sceneIntegrator != null && !_sceneIntegrator.IsInitialized) {
        yield return null;
      }

      if (_sceneIntegrator == null) {
        yield break;
      }

      if (TryGetComponent<UnityEngine.Collider>(out UnityEngine.Collider collider)) {
        _sceneIntegrator.RegisterCollider(collider);
      }

      if (TryGetComponent(out Canvas canvas)) {
        _sceneIntegrator.RegisterCanvas(canvas);
      }
    }
  }
}  // namespace Android.XR.InteractionFramework
