// Copyright 2026 Google LLC
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

using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Base class for retrieving necessary data to fill out input source states.
  /// </summary>
  public abstract class AxrifInputSource {
    protected bool _isActive;

    /// <summary>
    /// Whether or not the input source is active given the current configuration
    /// </summary>
    public bool IsActive => _isActive;

    /// <summary>
    /// Uses the Axrif configuration to determine if the input source is active
    /// </summary>
    /// <param name="configurations">The new Axrif Configuration</param>
    public abstract void UpdateConfiguration(Configurations configurations);

    /// <summary>
    /// Updates the source state in the AxrifSubsystem
    /// </summary>
    /// <param name="axrifSubsystem">A reference to the AxrifSubsystem</param>
    public abstract void UpdateSource(AxrifSubsystem axrifSubsystem);

    /// <summary>
    /// Enables all InputActions
    /// </summary>
    public virtual void Enable() {
      EnableInputActions();
    }

    /// <summary>
    /// Disables all InputActions
    /// </summary>
    public virtual void Disable() {
      DisableInputActions();
    }
    protected abstract void EnableInputActions();
    protected abstract void DisableInputActions();

    /// <summary>
    /// Checks whether a pose state tracking action property is currently fully tracked.
    /// </summary>
    /// <param name="poseState">An input action property representing a pose tracking state.</param>
    /// <returns>True if the tracking state is fully tracked, false otherwise.</returns>
    protected bool IsPoseStateFullyTracked(InputActionProperty poseState) {
      if (poseState.action != null) {
        int state = poseState.action.ReadValue<int>();
        return (state & (int)InputTrackingState.Position) != 0 &&
               (state & (int)InputTrackingState.Rotation) != 0;
      }
      return false;
    }
  }
}  // namespace Android.XR.InteractionFramework
