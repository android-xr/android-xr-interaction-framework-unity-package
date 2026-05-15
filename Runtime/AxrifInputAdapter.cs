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

using System.Collections.Generic;
using UnityEngine;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Adapter class that bridges XRI InputActions to the AxrifSubsystem.
  /// </summary>
  [AddComponentMenu("Android XR/Interaction Framework Input Adapter")]
  [DisallowMultipleComponent]
  public class AxrifInputAdapter : MonoBehaviour {
    [SerializeField, Tooltip("Supplies the AxrifHandInputSource with InputAction bindings")]
    private AxrifHandSourceAsset _handSourceAsset;
    [SerializeField, Tooltip("Supplies the AxrifHeadInputSource with InputAction bindings")]
    private AxrifHeadSourceAsset _headSourceAsset;
    [SerializeField, Tooltip("Supplies the AxrifEyeInputSource with InputAction bindings")]
    private AxrifEyeSourceAsset _eyeSourceAsset;
    [SerializeField, Tooltip("Supplies the AxrifMouseInputSource with InputAction bindings")]
    private AxrifMouseSourceAsset _mouseSourceAsset;
    [SerializeField, Tooltip("Supplies the AxrifControllerInputSource with InputAction bindings")]
    private AxrifControllerSourceAsset _controllerSourceAsset;

    /// <summary>
    /// The list of input sources created from Axrif Source assets.
    /// </summary>
    private List<AxrifInputSource> _inputSources = new();

    /// <summary>
    /// The subsystem instance to update with input data.
    /// </summary>
    private AxrifSubsystem _axrifSubsystem;

    /// <summary>
    /// Whether or not the AxrifInputAdapter is registered with the AxrifSubsystem's
    /// onSourceUpdateRequired callback.
    /// </summary>
    private bool _isRegistered;

    /// <summary>
    /// Called when the component is enabled.
    /// </summary>
    private void OnEnable() {
      if (_axrifSubsystem != null && !_isRegistered) {
        RegisterSourceUpdates();
      }
      foreach (var inputSource in _inputSources) {
        inputSource.Enable();
      }
    }

    /// <summary>
    /// Called when the component is disabled.
    /// </summary>
    private void OnDisable() {
      UnregisterSourceUpdates();
      foreach (var inputSource in _inputSources) {
        inputSource.Disable();
      }
    }

    /// <summary>
    /// Supplies the AxrifInputAdapter with a reference to the AxrifSubsystem and the active Axrif
    /// Configuration to determine which AxrifInputSources are active.
    /// </summary>
    /// <param name="subsystem">A reference to the AxrifSubsystem.</param>
    /// <param name="configurations">The active Axrif Configuration.</param>
    public void Initialize(AxrifSubsystem subsystem, Configurations configurations) {
      if (!CheckSourceAsset(_handSourceAsset) || !CheckSourceAsset(_headSourceAsset) ||
          !CheckSourceAsset(_eyeSourceAsset) || !CheckSourceAsset(_mouseSourceAsset)) {
        enabled = false;
        return;
      }
      _inputSources.Clear();

      _inputSources.Add(_handSourceAsset.CreateInstance());
      _inputSources.Add(_headSourceAsset.CreateInstance());
      _inputSources.Add(_eyeSourceAsset.CreateInstance());
      _inputSources.Add(_mouseSourceAsset.CreateInstance());
      _inputSources.Add(_controllerSourceAsset.CreateInstance());

      _axrifSubsystem = subsystem;

      RegisterSourceUpdates();
      UpdateConfiguration(configurations);
    }

    /// <summary>
    /// Registers the AxrifSubsystem onSourceUpdateRequired event.
    /// </summary>
    public void RegisterSourceUpdates() {
      if (_axrifSubsystem == null) {
        Debug.LogWarning("[AxrifInputAdapter:RegisterSourceUpdates] " +
                         "Failed to retrieve valid AxrifSubsystem reference");
        return;
      }

      if (_isRegistered) {
        return;
      }

      _axrifSubsystem.onSourceUpdateRequired += OnSourceUpdateRequired;
      _isRegistered = true;
    }

    /// <summary>
    /// Unregisters the AxrifSubsystem onSourceUpdateRequired event.
    /// </summary>
    public void UnregisterSourceUpdates() {
      if (!_isRegistered) {
        return;
      }
      if (_axrifSubsystem != null) {
        _axrifSubsystem.onSourceUpdateRequired -= OnSourceUpdateRequired;
      }
      _isRegistered = false;
    }

    /// <summary>
    /// Updates the configuration to determine which AxrifInputSources are active.
    /// </summary>
    public void UpdateConfiguration(Configurations configurations) {
      foreach (var inputSource in _inputSources) {
        inputSource.UpdateConfiguration(configurations);
      }
    }

    /// <summary>
    /// Handles the AxrifSubsystem onSourceUpdateRequired event.
    /// </summary>
    private void OnSourceUpdateRequired() {
      if (_inputSources == null) {
        return;
      }
      foreach (var inputSource in _inputSources) {
        if (inputSource.IsActive) {
          inputSource.UpdateSource(_axrifSubsystem);
        }
      }
    }

    /// <summary>
    /// Checks that the provided Source Asset has a valid reference.
    /// </summary>
    /// <param name="sourceAsset">A ScriptableObject representing the Input SourceAsset</param>
    /// <returns>True if the sourceAsset has a valid reference, otherwise false</returns>
    private bool CheckSourceAsset(ScriptableObject sourceAsset) {
      if (sourceAsset == null) {
        Debug.LogError("[AxrifInputAdapter:CheckSourceAsset] Source asset is null");
        return false;
      }
      return true;
    }
  }
}  // namespace Android.XR.InteractionFramework
