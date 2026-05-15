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

using UnityEngine;
using Android.XR.InteractionFramework.Extensions;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// A struct that holds a reference to a Unity Collider or RectTransform and tracks its
  /// transformation matrix.
  /// </summary>
  public struct AxrifTrackedCollider {
    /// <summary>
    /// The type of Unity component that is being tracked.
    /// </summary>
    public enum TrackedType {
      UnityCollider = 0,
      RectTransform = 1,
    }

    /// <summary>
    /// The unique id of the tracked collider.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Indicates whether the collider can be interacted with directly via poke.
    /// </summary>
    public bool IsTouchable { get; private set; }

    /// <summary>
    /// A reference to the Unity Collider that is being tracked.
    /// </summary>
    public UnityEngine.Collider UnityCollider { get; private set; }

    /// <summary>
    /// A reference to the RectTransform that is being tracked.
    /// </summary>
    public RectTransform Rect { get; private set; }

    /// <summary>
    /// The type of Unity component that is being tracked.
    /// </summary>
    public TrackedType Type { get; private set; }

    /// <summary>
    /// The previous transformation matrix of the tracked collider.
    /// </summary>
    public Matrix4x4 PreviousMatrix;

    /// <summary>
    /// Checks if the tracked collider is valid.
    /// </summary>
    /// <returns>True if the underlying Unity component is valid, false otherwise.</returns>
    public bool IsValid() {
      if (Type == TrackedType.UnityCollider) {
        return UnityCollider != null;
      }
      return Rect != null;
    }

    /// <summary>
    /// Checks if the tracked collider has changed since the last update.
    /// </summary>
    /// <returns>True if the tracked collider has changed, false otherwise.</returns>
    public bool HasChanged() {
      if (Type == TrackedType.UnityCollider) {
        return PreviousMatrix != UnityCollider.transform.localToWorldMatrix;
      }
      return PreviousMatrix != Rect.transform.localToWorldMatrix;
    }

    /// <summary>
    /// Updates the transformation matrix of the tracked collider.
    /// </summary>
    public void UpdateTransform() {
      if (Type == TrackedType.UnityCollider) {
        PreviousMatrix = UnityCollider.transform.localToWorldMatrix;
        return;
      }
      PreviousMatrix = Rect.transform.localToWorldMatrix;
    }

    /// <summary>
    /// Tries to convert the tracked collider to an Axrif collider.
    /// </summary>
    /// <param name="axrifCollider">The resulting Axrif collider.</param>
    /// <returns>True if the conversion was successful, false otherwise.</returns>
    public bool TryToAxrifCollider(out Collider axrifCollider) {
      if (Type == TrackedType.UnityCollider) {
        return UnityCollider.TryToAxrifCollider(Id, out axrifCollider, IsTouchable);
      }
      axrifCollider = Rect.ToAxrifCollider(Id, IsTouchable);
      return true;
    }

    /// <summary>
    /// Creates a tracked collider from a Unity Collider.
    /// </summary>
    /// <param name="id">The unique id of the tracked collider.</param>
    /// <param name="collider">The Unity Collider to create the tracked collider from.</param>
    /// <param name="isTouchable">
    /// Whether the collider can be interacted with directly via poke.
    /// </param>
    /// <returns>The tracked collider.</returns>
    public static AxrifTrackedCollider GetTrackedCollider(int id, UnityEngine.Collider collider,
                                                          bool isTouchable) {
      return new AxrifTrackedCollider() { Id = id, UnityCollider = collider,
                                          IsTouchable = isTouchable,
                                          Type = TrackedType.UnityCollider,
                                          PreviousMatrix = collider.transform.localToWorldMatrix };
    }

    /// <summary>
    /// Creates a tracked collider from a RectTransform.
    /// </summary>
    /// <param name="id">The unique id of the tracked collider.</param>
    /// <param name="rectTransform">The RectTransform to create the tracked collider from.</param>
    /// <param name="isTouchable">
    /// Whether the collider can be interacted with directly via poke.
    /// </param>
    /// <returns>The tracked collider.</returns>
    public static AxrifTrackedCollider GetTrackedCollider(int id, RectTransform rectTransform,
                                                          bool isTouchable) {
      return new AxrifTrackedCollider() { Id = id, Rect = rectTransform, IsTouchable = isTouchable,
                                          Type = TrackedType.RectTransform,
                                          PreviousMatrix = rectTransform.localToWorldMatrix };
    }
  }

}  // namespace Android.XR.InteractionFramework
