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

using UnityEngine.SubsystemsImplementation;

namespace Android.XR.InteractionFramework {

  public partial class AxrifSubsystem {
    public abstract class Provider : SubsystemProvider<AxrifSubsystem> {
      public Configurations Configurations {
        get => _configurations;
        set => _configurations = value;
      }

      protected Configurations _configurations = new Configurations();

      /// <inheritdoc/>
      public abstract void UpdateHandsSourceState(in HandsSourceState handsSourceState);

      /// <inheritdoc/>
      public abstract void UpdateHeadSourceState(in HeadSourceState headSourceState);

      /// <inheritdoc/>
      public abstract void UpdateEyesSourceState(in EyesSourceState eyesSourceState);

      /// <inheritdoc/>
      public abstract void UpdateMouseSourceState(in MouseSourceState mouseSourceState);

      /// <inheritdoc/>
      public abstract void UpdateControllersSourceState(
          in ControllersSourceState controllersSourceState);

      /// <inheritdoc/>
      public abstract void SetCollider(in Collider collider);

      /// <inheritdoc/>
      public abstract void RemoveCollider(int collider_id);

      /// <inheritdoc/>
      public abstract PointerEvent GetPointerEvent(InteractionType interactionType);

      /// <inheritdoc/>
      public abstract PointerEvent GetSelectedPointerEvent();
    }
  }
}  // namespace Android.XR.InteractionFramework
