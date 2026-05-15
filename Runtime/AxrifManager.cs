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
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Android.XR.InteractionFramework {

  /// <summary>Class used to call Android XR Interaction Framework APIs.</summary>
  [RequireComponent(typeof(XRInteractionManager))]
  [AddComponentMenu("Android XR/Interaction Framework Manager")]
  public class AxrifManager : MonoBehaviour {
    [SerializeField]
    private Configurations configurations;

    private AxrifSubsystem _axrifSubsystem;
    private AxrifSceneIntegrator _sceneIntegrator;
    private AxrifInputAdapter _inputAdapter;

    [Space]
    [SerializeField, Tooltip("Input mode changed event for the left hand.")]
    private UnityEvent<InteractionType> _onInputModeChangedLeft = new UnityEvent<InteractionType>();

    [SerializeField, Tooltip("Input mode changed event for the right hand.")]
    private UnityEvent<InteractionType> _onInputModeChangedRight =
        new UnityEvent<InteractionType>();

    /// <summary>Input mode changed event for the left hand.</summary>
    public UnityEvent<InteractionType> OnInputModeChangedLeft => _onInputModeChangedLeft;

    /// <summary>Input mode changed event for the right hand.</summary>
    public UnityEvent<InteractionType> OnInputModeChangedRight => _onInputModeChangedRight;

    /// <summary>Current input mode for the left hand.</summary>
    public InteractionType? CurrentInputModeLeft =>
        _axrifSubsystem != null ? _axrifSubsystem.CurrentInputModeLeft : null;

    /// <summary>Current input mode for the right hand.</summary>
    public InteractionType? CurrentInputModeRight =>
        _axrifSubsystem != null ? _axrifSubsystem.CurrentInputModeRight : null;

    public void OnEnable() {
      if (_axrifSubsystem == null && !AxrifSubsystemUtils.TryGetSubsystem(out _axrifSubsystem)) {
        Debug.Log("[AxrifManager:OnEnable] No axrif subsystem");
        enabled = false;
        return;
      }

      // Currently configurations changes need a restart of the subsystem.
      _axrifSubsystem.SetConfigurations(configurations);
      _axrifSubsystem.Start();

      var xrInteractionManager = GetComponent<XRInteractionManager>();
      if (_sceneIntegrator == null) {
        _sceneIntegrator = GetComponent<AxrifSceneIntegrator>();
        if (_sceneIntegrator == null) {
          _sceneIntegrator = gameObject.AddComponent<AxrifSceneIntegrator>();
        }
      }
      _sceneIntegrator.enabled = true;
      _sceneIntegrator.Initialize(_axrifSubsystem, xrInteractionManager);

      _inputAdapter = GetComponent<AxrifInputAdapter>();
      if (_inputAdapter == null) {
        _inputAdapter = gameObject.AddComponent<AxrifInputAdapter>();
      }
      _inputAdapter.Initialize(_axrifSubsystem, configurations);
      _inputAdapter.enabled = true;

      _axrifSubsystem.OnInputModeChangedLeft += _onInputModeChangedLeft.Invoke;
      _axrifSubsystem.OnInputModeChangedRight += _onInputModeChangedRight.Invoke;
    }

    public void OnDisable() {
      if (_axrifSubsystem == null) {
        return;
      }

      if (_sceneIntegrator != null) {
        _sceneIntegrator.enabled = false;
      }
      if (_inputAdapter != null) {
        _inputAdapter.enabled = false;
      }

      _axrifSubsystem.OnInputModeChangedLeft -= _onInputModeChangedLeft.Invoke;
      _axrifSubsystem.OnInputModeChangedRight -= _onInputModeChangedRight.Invoke;

      _axrifSubsystem.Stop();
    }

    public void OnDestroy() {
      if (_axrifSubsystem == null) {
        return;
      }

      _axrifSubsystem.Destroy();
    }
  }
}  // namespace Android.XR.InteractionFramework
