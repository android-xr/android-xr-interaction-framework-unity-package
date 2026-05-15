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

namespace Android.XR.InteractionFramework {

  public class AxrifSubsystemProvider : AxrifSubsystem.Provider {
    private IntPtr _context = IntPtr.Zero;

    /// <inheritdoc/>
    public override void Start() {
      NativeApi.Axrif_Create(in _configurations, ref _context);
      if (_context == IntPtr.Zero) {
        Debug.LogError("[AxrifSubsystemProvider:Start] Axrif creation failed");
      }
    }

    /// <inheritdoc/>
    public override void Stop() {}

    /// <inheritdoc/>
    public override void Destroy() {
      NativeApi.Axrif_Destroy(_context);
      _context = IntPtr.Zero;
    }

    /// <inheritdoc/>
    public override void UpdateHandsSourceState(in HandsSourceState handsSourceState) {
      NativeApi.Axrif_UpdateHandsSourceState(_context, in handsSourceState);
    }

    /// <inheritdoc/>
    public override void UpdateHeadSourceState(in HeadSourceState headSourceState) {
      NativeApi.Axrif_UpdateHeadSourceState(_context, in headSourceState);
    }

    /// <inheritdoc/>
    public override void UpdateEyesSourceState(in EyesSourceState eyesSourceState) {
      NativeApi.Axrif_UpdateEyesSourceState(_context, in eyesSourceState);
    }

    /// <inheritdoc/>
    public override void UpdateMouseSourceState(in MouseSourceState mouseSourceState) {
      NativeApi.Axrif_UpdateMouseSourceState(_context, in mouseSourceState);
    }

    public override void UpdateControllersSourceState(
        in ControllersSourceState controllersSourceState) {
      NativeApi.Axrif_UpdateControllersSourceState(_context, in controllersSourceState);
    }

    /// <inheritdoc/>
    public override void SetCollider(in Collider collider) {
      NativeApi.Axrif_SetCollider(_context, in collider);
    }

    /// <inheritdoc/>
    public override void RemoveCollider(int collider_id) {
      NativeApi.Axrif_RemoveCollider(_context, collider_id);
    }

    /// <inheritdoc/>
    public override PointerEvent GetPointerEvent(InteractionType interactionType) {
      long monotonicNs = (long)(UnityEngine.Time.realtimeSinceStartupAsDouble * 1_000_000_000.0);
      NativeApi.Axrif_GetPointerEvent(_context, interactionType, monotonicNs,
                                      out PointerEvent pointerEvent);
      return pointerEvent;
    }

    /// <inheritdoc/>
    public override PointerEvent GetSelectedPointerEvent() {
      long monotonicNs = (long)(UnityEngine.Time.realtimeSinceStartupAsDouble * 1_000_000_000.0);
      NativeApi.Axrif_GetSelectedPointerEvent(_context, monotonicNs, out PointerEvent pointerEvent);
      return pointerEvent;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegisterDescriptor() {
      AxrifSubsystem.Descriptor.Create(new AxrifSubsystem.Descriptor.Cinfo() {
        id = "AxrifSubsystemProvider", providerType = typeof(AxrifSubsystemProvider)
      });
    }
  }
}  // namespace Android.XR.InteractionFramework
