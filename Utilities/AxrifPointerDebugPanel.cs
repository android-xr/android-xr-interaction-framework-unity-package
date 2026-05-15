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
using System.Text;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Delivers selected pointer details to a set of Text fields
  /// for debugging purposes at runtime
  /// </summary>
  public class AxrifPointerDebugPanel : MonoBehaviour {
    [SerializeField]
    private bool active = true;
    [SerializeField]
    private string pointerCountPrefix = "Pointer Count: ";
    [SerializeField]
    private UnityEngine.UI.Text pointerCountText;
    [SerializeField]
    private string pointerTypePrefix = "First Pointer Type: ";
    [SerializeField]
    private UnityEngine.UI.Text pointerTypeText;
    [SerializeField]
    private UnityEngine.UI.Text firstPointerText;
    [SerializeField]
    private UnityEngine.UI.Text secondPointerText;
    private AxrifSubsystem _axrifSubsystem;

    void OnEnable() {
      gameObject.SetActive(active);
      if (!active) {
        return;
      }

      if (_axrifSubsystem == null && !AxrifSubsystemUtils.TryGetSubsystem(out _axrifSubsystem)) {
        Debug.Log("[AxrifPointerDebugPanel:OnEnable] No axrif subsystem");
        enabled = false;
        return;
      }
      if (pointerCountText == null || firstPointerText == null || secondPointerText == null ||
          pointerTypeText == null) {
        Debug.Log("[AxrifPointerDebugPanel:OnEnable] Failed to find all text references");
        enabled = false;
        return;
      }
      _axrifSubsystem.onPointerEventUpdated += OnPointerEventData;
    }

    void OnDisable() {
      if (_axrifSubsystem != null) {
        _axrifSubsystem.onPointerEventUpdated -= OnPointerEventData;
      }
    }

    private void OnPointerEventData(PointerEvent e) {
      if (!e.IsValid) {
        pointerCountText.text = "No valid pointers.";
        pointerTypeText.text = "";
        firstPointerText.text = "";
        secondPointerText.text = "";
        return;
      }
      string count = e.ValidPointerCount.ToString();
      pointerCountText.text = String.Format("{0} {1}", pointerCountPrefix, count);

      string type = e.ValidPointerCount > 0 ? e.Pointers[0].Type.ToString() : "None";
      pointerTypeText.text = String.Format("{0} {1}", pointerTypePrefix, type);
      HandlePointerData(e.Pointers[0], firstPointerText, 1, e.ValidPointerCount > 0);
      HandlePointerData(e.Pointers[1], secondPointerText, 2, e.ValidPointerCount > 1);
    }

    private void HandlePointerData(Pointer p, UnityEngine.UI.Text text, int count, bool valid) {
      if (!valid) {
        text.text = "";
        return;
      }
      StringBuilder b = new StringBuilder();
      b.Append(string.Format("Pointer: {0}", count));
      b.Append("\n");
      b.Append(string.Format("Type: {0}", GetReadablePointerType(p.Type)));
      b.Append("\n");
      b.Append(string.Format("Hand: {0}", p.Handedness.ToString()));
      b.Append("\n");
      b.Append(string.Format("Activation: {0}", p.ActivationAmount));
      text.text = b.ToString();
    }
    private string GetReadablePointerType(Pointer.PointerType type) {
      switch (type) {
        case Pointer.PointerType.Gaze:
          return "Gaze";
        case Pointer.PointerType.Controller:
          return "Controller";
        case Pointer.PointerType.HandPoke:
          return "Poke";
        case Pointer.PointerType.HandRaycast:
          return "Hand Ray";
        case Pointer.PointerType.HandGazePinch:
          return "Hand Gaze Pinch";
        case Pointer.PointerType.Mouse:
          return "Mouse";
        default:
          return string.Empty;
      }
    }
  }
}  // namespace Android.XR.InteractionFramework
