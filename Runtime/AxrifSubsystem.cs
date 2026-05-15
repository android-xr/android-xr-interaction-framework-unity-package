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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SubsystemsImplementation;

namespace Android.XR.InteractionFramework {

  public partial class AxrifSubsystem
      : SubsystemWithProvider<AxrifSubsystem, AxrifSubsystem.Descriptor, AxrifSubsystem.Provider> {
    /// <summary>Require source update event.</summary>
    public event Action onSourceUpdateRequired;

    /// <summary>Update event.</summary>
    public event Action<PointerEvent> onPointerEventUpdated;

    /// <summary>Input mode changed event for the left hand.</summary>
    public event Action<InteractionType> OnInputModeChangedLeft;

    /// <summary>Input mode changed event for the right hand.</summary>
    public event Action<InteractionType> OnInputModeChangedRight;

    /// <summary>Current input mode for the left hand.</summary>
    public InteractionType? CurrentInputModeLeft { get; private set; } = null;

    /// <summary>Current input mode for the right hand.</summary>
    public InteractionType? CurrentInputModeRight { get; private set; } = null;

    /// <summary>The provider for this subsystem.</summary>
    public new Provider provider => base.provider;

    public void SetConfigurations(Configurations configurations) {
      this.provider.Configurations = configurations;
    }

#region Colliders
    public void SetCollider(Collider collider) => provider.SetCollider(in collider);

    public void RemoveCollider(Collider collider) => provider.RemoveCollider(collider.Id);

#endregion

    internal void UpdateEyesSourceState(EyesSourceState sourceState) =>
        provider.UpdateEyesSourceState(in sourceState);

    internal void UpdateHeadSourceState(HeadSourceState sourceState) =>
        provider.UpdateHeadSourceState(in sourceState);

    internal void UpdateHandsSourceState(HandsSourceState sourceState) =>
        provider.UpdateHandsSourceState(in sourceState);

    internal void UpdateMouseSourceState(MouseSourceState sourceState) =>
        provider.UpdateMouseSourceState(in sourceState);

    internal void UpdateControllersSourceState(ControllersSourceState sourceState) =>
        provider.UpdateControllersSourceState(in sourceState);

    protected override void OnStart() {
      base.OnStart();
      InputSystem.onAfterUpdate += OnAfterInputSystemUpdate;
    }

    protected override void OnStop() {
      base.OnStop();
      InputSystem.onAfterUpdate -= OnAfterInputSystemUpdate;
    }

    private void OnAfterInputSystemUpdate() {
      onSourceUpdateRequired?.Invoke();
      var pointerEvent = provider.GetSelectedPointerEvent();
      onPointerEventUpdated?.Invoke(pointerEvent);

      InteractionType? nextInputModeLeft = null;
      InteractionType? nextInputModeRight = null;

      if (pointerEvent.IsValid) {
        for (var index = 0; index < pointerEvent.ValidPointerCount; index++) {
          var pointer = pointerEvent.Pointers[index];
          var inputMode = GetInteractionTypeFromPointer(pointer.Type);
          if (pointer.Handedness == Handedness.Left || pointer.Handedness == Handedness.Both) {
            nextInputModeLeft = inputMode;
          }
          if (pointer.Handedness == Handedness.Right || pointer.Handedness == Handedness.Both) {
            nextInputModeRight = inputMode;
          }
        }
      }

      if (nextInputModeLeft != CurrentInputModeLeft && nextInputModeLeft != null) {
        CurrentInputModeLeft = nextInputModeLeft;
        OnInputModeChangedLeft?.Invoke((InteractionType)CurrentInputModeLeft);
      }
      if (nextInputModeRight != CurrentInputModeRight && nextInputModeRight != null) {
        CurrentInputModeRight = nextInputModeRight;
        OnInputModeChangedRight?.Invoke((InteractionType)CurrentInputModeRight);
      }
    }

    private InteractionType? GetInteractionTypeFromPointer(Pointer.PointerType pointerType) {
      switch (pointerType) {
        case Pointer.PointerType.HandRaycast:
          return InteractionType.HandRaycast;
        case Pointer.PointerType.HandPoke:
          return InteractionType.HandPoke;
        case Pointer.PointerType.HandGazePinch:
        case Pointer.PointerType.Gaze:
          return InteractionType.GazePinch;
        case Pointer.PointerType.Mouse:
          return InteractionType.Mouse;
        case Pointer.PointerType.Controller:
          return InteractionType.Controller;
        default:
          return null;
      }
    }
  }
}  // namespace Android.XR.InteractionFramework
