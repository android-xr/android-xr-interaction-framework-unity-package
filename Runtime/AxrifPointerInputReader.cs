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

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Android.XR.InteractionFramework {

  [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRInputDeviceButtonReader)]
  [AddComponentMenu("XR/AXRIF/XR Input Reader (AndroidXR Pointer Events)")]
  public class AxrifPointerInputReader : MonoBehaviour, IXRInputButtonReader {
    [SerializeField, Tooltip("Selects which pointer types to read.")]
    private PointerMask _pointerMask = PointerMask.None;

    [SerializeField, Tooltip("Selects which hand to read input from.")]
    private Handedness _handedness = Handedness.Left;

    [Space]
    [SerializeField, Tooltip("Sends updates for pointer active states.")]
    private UnityEvent<bool> _onPointerActiveUpdate;

    /// <summary>
    /// The attached interactor to enable or disable based on the current pointer data.
    /// </summary>
    private XRBaseInteractor _interactor;

    /// <summary>
    /// A reference to the Axrif Subsystem. Used to get pointer data.
    /// </summary>
    private AxrifSubsystem _subsystem;

    /// <summary>
    /// The activation value associated with the pointer data.
    /// </summary>
    private float _activationValue = -1f;

    /// <summary>
    /// The activation value at the end of the previous frame.
    /// </summary>
    private float _previousActivationValue = -1f;

    /// <summary>
    /// Whether or not an activation was performed this frame.
    /// </summary>
    private bool _wasPerformedThisFrame;

    /// <summary>
    /// Whether or not an activation was completed this frame.
    /// </summary>
    private bool _wasCompletedThisFrame;

    /// <summary>
    /// Whether or not the pointer is currently active
    /// </summary>
    private bool _isPointerActive = true;

    public void Awake() {
      _interactor = GetComponent<XRBaseInteractor>();
    }

    public void OnEnable() {
      if (_subsystem == null && !AxrifSubsystemUtils.TryGetSubsystem(out _subsystem)) {
        Debug.Log("[AxrifPointerInputReader:OnEnable] No axrif subsystem");
        enabled = false;
        return;
      }

      _subsystem.onPointerEventUpdated += OnPointerEventUpdated;
    }

    public void OnDisable() {
      if (_subsystem != null) {
        _subsystem.onPointerEventUpdated -= OnPointerEventUpdated;
      }
    }

    /// <summary>
    /// Returns the current activation value.
    /// </summary>
    /// <returns>Current activation value.</returns>
    public float ReadValue() => _activationValue;

    /// <summary>
    /// Attempts to read the current activation value.
    /// </summary>
    /// <param name="value">The current activation value if successful.</param>
    /// <returns>True if the current activation value was read successfully, otherwise
    /// false.</returns>
    public bool TryReadValue(out float value) {
      value = _activationValue;
      return true;
    }

    /// <summary>
    /// Returns whether the current activation value is 1.
    /// </summary>
    /// <returns>True if the current activation value is 1, otherwise false.</returns>
    public bool ReadIsPerformed() => _activationValue == 1f;

    /// <summary>
    /// Returns whether the activation value was first 1 on the current frame.
    /// </summary>
    /// <returns>True if the activation value was first 1 on this frame, otherwise false.</returns>
    public bool ReadWasPerformedThisFrame() => _wasPerformedThisFrame;

    /// <summary>
    /// Returns whether the activation value is no longer 1 on the current frame.
    /// </summary>
    /// <returns>True if the activation value is no longer 1 starting on the current frame,
    /// otherwise false.</returns>
    public bool ReadWasCompletedThisFrame() => _wasCompletedThisFrame;

    public void Update() {
      _wasPerformedThisFrame = _activationValue == 1f && _previousActivationValue < 1f;
      _wasCompletedThisFrame = _previousActivationValue == 1f && _activationValue < 1f;
      _previousActivationValue = _activationValue;
    }

    /// <summary>
    /// Updates activation and pointer status given PointerEvent data.
    /// </summary>
    /// <param name="pointers">Pointer event data from Axrif.</param>
    private void OnPointerEventUpdated(PointerEvent pointers) {
      var pointerIsValid = false;
      _activationValue = -1f;
      for (var index = 0; index < pointers.ValidPointerCount; index++) {
        var pointer = pointers.Pointers[index];
        var flag = 1 << (sbyte)pointer.Type;
        if (_pointerMask.HasFlag((PointerMask)flag) && _handedness == pointer.Handedness) {
          bool pointerIsActive = pointer.IsValid && pointer.IsActive;
          if (pointerIsActive) {
            _activationValue = pointer.ActivationAmount;
            pointerIsValid = true;
            break;
          }
        }
      }

      // Activate interactor if at least one of the pointer types is valid
      if (_interactor != null) {
        _interactor.enabled = pointerIsValid;
      }
      if (pointerIsValid != _isPointerActive) {
        _onPointerActiveUpdate?.Invoke(pointerIsValid);
        _isPointerActive = pointerIsValid;
      }
    }
  }
}  // namespace Android.XR.InteractionFramework
