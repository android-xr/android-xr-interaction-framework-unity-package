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

namespace Android.XR.InteractionFramework.Extensions {

  /// <summary>
  /// Extensions extending UnityEngine.Collider class.
  /// </summary>
  public static class ColliderExtensions {
    /// <summary>
    /// A pre-allocated array used to retrieve RectTransform world corners.
    /// Shared statically to avoid garbage collection allocations during frequent updates.
    /// </summary>
    private static readonly Vector3[] _rectCorners = new Vector3[4];
    /// <summary>
    /// Tries to create an AXRIF Collider from a 3D Unity Collider object.
    /// This function supports Unity's BoxCollider and SphereCollider.
    /// </summary>
    /// <param name="unityCollider">
    /// A UnityEngine.Collider object. This must be a BoxCollider or a SphereCollider 3D Collider
    /// type.
    /// </param>
    /// <param name="id"> A unique id for the axrif collider.</param>
    /// <param name="axrifCollider">The resulting AXRIF collider if successful.</param>
    /// <param name="isTouchable">
    /// Whether the collider can be interacted with directly via poke.
    /// </param>
    /// <returns>
    /// True if an AXRIF collider of type Box or Sphere was successfully created, false otherwise.
    /// </returns>
    public static bool TryToAxrifCollider(this UnityEngine.Collider unityCollider, int id,
                                          out Collider axrifCollider, bool isTouchable = false) {
      axrifCollider =
          new Collider { Id = id, Rotation = unityCollider.transform.rotation.ToQuaternionf(),
                         IsTouchable = isTouchable };
      if (unityCollider is BoxCollider box) {
        axrifCollider.Type = Collider.ColliderType.Box;
        axrifCollider.Center = unityCollider.transform.TransformPoint(box.center).ToVector3f();
        axrifCollider.Length = box.size.x * Mathf.Abs(box.transform.lossyScale.x);
        axrifCollider.Height = box.size.y * Mathf.Abs(box.transform.lossyScale.y);
        axrifCollider.Depth = box.size.z * Mathf.Abs(box.transform.lossyScale.z);
        return true;
      } else if (unityCollider is SphereCollider sphere) {
        float scale = Mathf.Max(Mathf.Abs(sphere.transform.lossyScale.x),
                                Mathf.Abs(sphere.transform.lossyScale.y),
                                Mathf.Abs(sphere.transform.lossyScale.z));
        axrifCollider.Type = Collider.ColliderType.Sphere;
        axrifCollider.Center = unityCollider.transform.TransformPoint(sphere.center).ToVector3f();
        axrifCollider.Radius = sphere.radius * scale;
        return true;
      } else {
        Debug.LogWarning(
            "[ColliderExtension:TryToAxrifCollider] Unsupported Unity Collider detected: " +
            unityCollider.name);
        return false;
      }
    }

    /// <summary>
    /// Tries to create an AXRIF Collider from a 3D Unity Collider object with id equal to the
    /// Unity Collider's instance id. This function supports Unity's BoxCollider and SphereCollider.
    /// </summary>
    /// <param name="unityCollider">
    /// A UnityEngine.Collider object. This must be a BoxCollider or a SphereCollider 3D Collider
    /// type.
    /// </param>
    /// <param name="axrifCollider">The resulting AXRIF collider if successful.</param>
    /// <returns>
    /// True if an AXRIF collider of type Box or Sphere was successfully created, false otherwise.
    /// </returns>
    public static bool TryToAxrifCollider(this UnityEngine.Collider unityCollider,
                                          out Collider axrifCollider) {
      return unityCollider.TryToAxrifCollider(unityCollider.GetInstanceID(), out axrifCollider);
    }

    /// <summary>
    /// Creates a Rectangular AXRIF Collider from a RectTransform UI object.
    /// </summary>
    /// <param name="rect">The RectTansformUI object.</param>
    /// <param name="id"> A unique id for the axrif collider.</param>
    /// <param name="isTouchable">
    /// Whether the collider can be interacted with directly via poke.
    /// </param>
    /// <returns>A rectangular AXRIF Collider.</returns>
    public static Collider ToAxrifCollider(this RectTransform rect, int id,
                                           bool isTouchable = false) {
      // Indices:
      // 0: Bottom Left, 1: Top Left
      // 2: Top Right, 3: Bottom Right
      rect.GetWorldCorners(_rectCorners);
      return new Collider { Id = id,
                            IsTouchable = isTouchable,
                            Rotation = rect.transform.rotation.ToQuaternionf(),
                            Type = Collider.ColliderType.Rect,
                            Center = ((_rectCorners[2] + _rectCorners[0]) / 2f).ToVector3f(),
                            Length = Vector3.Distance(_rectCorners[0], _rectCorners[3]),
                            Height = Vector3.Distance(_rectCorners[0], _rectCorners[1]) };
    }

    /// <summary>
    /// Creates a Rectangular AXRIF Collider from a RectTransform UI object.
    /// </summary>
    /// <param name="rect">The RectTansformUI object.</param>
    /// <returns>A rectangular AXRIF Collider.</returns>
    public static Collider ToAxrifCollider(this RectTransform rect) {
      return rect.ToAxrifCollider(rect.GetInstanceID());
    }
  }
}  // namespace Android.XR.InteractionFramework
