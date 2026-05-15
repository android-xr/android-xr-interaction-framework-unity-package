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

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Mock AXRIF provider which can be used to mock the subsystem and unity scripts parts only
  /// without calling any extern functions.
  /// </summary>
  public class AxrifSubsystemMockProvider : AxrifSubsystem.Provider {
    /// <inheritdoc/>
    public override void Start() {}

    /// <inheritdoc/>
    public override void Stop() {}

    /// <inheritdoc/>
    public override void Destroy() {}

    /// <inheritdoc/>
    public override void UpdateHandsSourceState(in HandsSourceState handsSourceState) {}

    /// <inheritdoc/>
    public override void UpdateHeadSourceState(in HeadSourceState headSourceState) {}

    /// <inheritdoc/>
    public override void UpdateEyesSourceState(in EyesSourceState eyesSourceState) {}

    /// <inheritdoc/>
    public override void UpdateMouseSourceState(in MouseSourceState mouseSourceState) {}

    /// <inheritdoc/>
    public override void UpdateControllersSourceState(
        in ControllersSourceState controllersSourceState) {}

    /// <inheritdoc/>
    public override void SetCollider(in Collider collider) {}

    /// <inheritdoc/>
    public override void RemoveCollider(int collider_id) {}

    /// <inheritdoc/>
    public override PointerEvent GetPointerEvent(InteractionType interactionType) => new();

    /// <summary>The pointer event to return from <see cref="GetSelectedPointerEvent"/>.</summary>
    public PointerEvent SelectedPointerEvent { get; set; } = new();

    /// <inheritdoc/>
    public override PointerEvent GetSelectedPointerEvent() => SelectedPointerEvent;

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RegisterDescriptor() {
      AxrifSubsystem.Descriptor.Create(new AxrifSubsystem.Descriptor.Cinfo() {
        id = "AxrifSubsystemMockProvider", providerType = typeof(AxrifSubsystemMockProvider)
      });
    }
#endif
  }
}  // namespace Android.XR.InteractionFramework
