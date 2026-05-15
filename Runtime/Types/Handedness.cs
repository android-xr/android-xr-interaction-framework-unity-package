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
  /// Represents which hand is being used to interact.
  /// </summary>
  public enum Handedness : sbyte {
    /// <summary>
    /// The left hand is being used to interact.
    /// </summary>
    Left = 0,

    /// <summary>
    /// The right hand is being used to interact.
    /// </summary>
    Right = 1,

    /// <summary>
    /// Both hands are being used to interact.
    /// </summary>
    Both = 2,
  }

}  // namespace Android.XR.InteractionFramework
