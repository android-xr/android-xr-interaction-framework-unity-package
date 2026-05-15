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

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Represents a bitmask of pointer type that can be triggered during an interaction.
  /// This enables multimodal interaction where a single interactor can respond to multiple
  /// pointer types.
  /// </summary>
  [Flags]
  public enum PointerMask {
    None = 0,
    Hand = 1 << 0,
    HandPoke = 1 << 1,
    HandGazePinch = 1 << 2,
    Eye = 1 << 3,
    Mouse = 1 << 4,
    Controller = 1 << 5,
    SurfaceTouch = 1 << 6,
  }
}  // namespace Android.XR.InteractionFramework
