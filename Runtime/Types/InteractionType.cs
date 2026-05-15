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

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Represents a type of interaction that can be performed. These types map
  /// directly to interaction modules.
  /// </summary>
  public enum InteractionType : sbyte {
    /// <summary>
    /// Either Eye or Head is used to point and hand pinches are used to select.
    /// </summary>
    GazePinch = 0,

    /// <summary>
    /// Hands are used to point and select from a distance.
    /// </summary>
    HandRaycast = 1,

    /// <summary>
    /// Hands are used to reach out and touch surfaces directly.
    /// </summary>
    HandPoke = 2,

    /// <summary>
    /// Hardware mouse is used to point and select.
    /// </summary>
    Mouse = 3,

    /// <summary>
    /// 6DoF controllers are used to point and select.
    /// </summary>
    Controller = 4,
  }
}  // namespace Android.XR.InteractionFramework
