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

using System.Collections.Generic;
using Android.XR.InteractionFramework.Extensions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

using Collider = Android.XR.InteractionFramework.Collider;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Manages the integration between Unity's XR Interaction Toolkit and the AxrifSubsystem.
  /// Handles registration and unregistration of interactables with the subsystem.
  /// </summary>
  [AddComponentMenu("Android XR/Interaction Framework Scene Integrator")]
  [DisallowMultipleComponent]
  public class AxrifSceneIntegrator : MonoBehaviour {
    [SerializeField, Tooltip("Whether or not to filter pokes for Canvases using a provided Tag. " +
                             "If this is false, all Canvases will be registered as poke-able.")]
    private bool _useCanvasTagForPoke = true;

    [SerializeField, Tooltip("The Tag name used to filter Poke interactions on Canvases")]
    private string _canvasPokeTag = "IsAxrifPokeCompatible";

    private AxrifSubsystem _axrifSubsystem;
    private XRInteractionManager _xrInteractionManager;

    // A list of tracked colliders for managing quick updates
    private RefList<AxrifTrackedCollider> _trackedColliders = new RefList<AxrifTrackedCollider>();

    // A dictionary of registered unity colliders for managing adds, removals, and updates.
    // The keys are the id of the corresponding AxrifCollider.
    private Dictionary<int, Collider> _colliderSet = new Dictionary<int, Collider>();

    /// <summary>
    /// Tracks whether scene UI elements and interactors have been registered as colliders.
    /// </summary>
    private bool _isRegistered;

    /// <summary>
    /// Gets a value indicating whether the integrator has been initialized with the required
    /// subsystem.
    /// </summary>
    public bool IsInitialized => _axrifSubsystem != null;

    /// <summary>
    /// Called when the component is enabled.
    /// </summary>
    private void OnEnable() {
      if (_axrifSubsystem != null && _xrInteractionManager != null && !_isRegistered) {
        RegisterAllColliders();
      }
    }

    /// <summary>
    /// Called when the component is disabled.
    /// </summary>
    private void OnDisable() {
      UnregisterAllColliders();
    }

    /// <summary>
    /// Detects changes in cached collider objects and updates them where necessary.
    /// </summary>
    private void FixedUpdate() {
      // Iterate backwards so we can safely remove invalid colliders in-place
      // without needing a separate reconciliation pass.
      for (int i = _trackedColliders.Count - 1; i >= 0; i--) {
        ref var trackedCollider = ref _trackedColliders[i];

        // Check if a tracked collider has lost its reference due to deletion or
        // if the collider id map is out of sync
        if (!trackedCollider.IsValid() || !_colliderSet.ContainsKey(trackedCollider.Id)) {
          RemoveColliderFromSubsystem(trackedCollider.Id);
          _trackedColliders.FastRemoveAt(i);
          continue;
        }

        if (trackedCollider.HasChanged()) {
          if (trackedCollider.TryToAxrifCollider(out Collider collider)) {
            if (SetCollider(collider)) {
              trackedCollider.UpdateTransform();
            } else {
              // If a collider is not updatable, clean up dangling collider
              RemoveColliderFromSubsystem(trackedCollider.Id);
              _trackedColliders.FastRemoveAt(i);
            }
          } else {
            // If a collider can no longer be converted, clean up
            RemoveColliderFromSubsystem(trackedCollider.Id);
            _trackedColliders.FastRemoveAt(i);
          }
        }
      }
    }

    /// <summary>
    /// Initializes the integrator with the required subsystems and managers.
    /// </summary>
    /// <param name="subsystem">The AxrifSubsystem instance.</param>
    /// <param name="manager">The XRInteractionManager instance.</param>
    public void Initialize(AxrifSubsystem subsystem, XRInteractionManager manager) {
      _axrifSubsystem = subsystem;
      _xrInteractionManager = manager;

      if (_axrifSubsystem != null && _xrInteractionManager != null && !_isRegistered) {
        RegisterAllColliders();
      }
    }

    /// <summary>
    /// Registers a Unity Collider object with the AxrifSceneIntegrator.
    /// </summary>
    /// <param name="collider">The collider object to register.</param>
    /// <returns>True if the collider has been successfully registered, false otherwise.</returns>
    public bool RegisterCollider(UnityEngine.Collider collider) {
      if (_axrifSubsystem == null) {
        return false;
      }
      int id = collider.GetInstanceID();
      var pokeFilter = collider.GetComponentInParent<XRPokeFilter>();
      bool isTouchable = pokeFilter != null;
      if (collider.TryToAxrifCollider(id, out Collider axrifCollider, isTouchable)) {
        if (SetCollider(axrifCollider)) {
          _trackedColliders.Add(AxrifTrackedCollider.GetTrackedCollider(id, collider, isTouchable));
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Registers a canvas UI element with the AxrifSceneIntegrator.
    /// </summary>
    /// <param name="canvas">The canvas UI element to register.</param>
    /// <returns>True if the canvas UI element has been successfully registered, false
    /// otherwise.</returns>
    public bool RegisterCanvas(Canvas canvas) {
      if (_axrifSubsystem == null) {
        return false;
      }
      if (!canvas.isRootCanvas || canvas.renderMode != RenderMode.WorldSpace) {
        Debug.LogWarningFormat(
            "[AxrifSceneIntegrator:RegisterCanvas] Failed to register canvas on {0}, must be a " +
                "root canvas and World Space render mode.",
            canvas.name);
        return false;
      }

      var rect = canvas.GetComponent<RectTransform>();
      if (rect == null) {
        Debug.LogWarningFormat(
            "[AxrifSceneIntegrator:RegisterCanvas] Failed to register canvas on {0}, " +
                "no RectTransform found.",
            canvas.name);
        return false;
      }
      int id = rect.GetInstanceID();
      bool isTouchable = _useCanvasTagForPoke ? rect.gameObject.CompareTag(_canvasPokeTag) : true;
      if (SetCollider(rect.ToAxrifCollider(id, isTouchable))) {
        _trackedColliders.Add(AxrifTrackedCollider.GetTrackedCollider(id, rect, isTouchable));
        return true;
      }
      return false;
    }

    /// <summary>
    /// Unregisters a Unity Collider object from the AxrifSceneIntegrator.
    /// </summary>
    /// <param name="collider">The collider object to unregister.</param>
    /// <returns>True if the collider has been successfully unregistered, false otherwise.</returns>
    public bool UnregisterCollider(UnityEngine.Collider collider) {
      return RemoveCollider(collider.GetInstanceID());
    }

    /// <summary>
    /// Unregisters a canvas UI element from the AxrifSceneIntegrator.
    /// </summary>
    /// <param name="canvas">The canvas UI element to unregister.</param>
    /// <returns>True if the canvas UI element has been successfully unregistered, false
    /// otherwise.</returns>
    public bool UnregisterCanvas(Canvas canvas) {
      var rect = canvas.GetComponent<RectTransform>();
      if (rect == null) {
        Debug.LogWarningFormat(
            "[AxrifSceneIntegrator:UnregisterCanvas] Failed to unregister canvas on " +
                "{0}, no RectTransform found.",
            canvas.name);
        return false;
      }
      return RemoveCollider(rect.GetInstanceID());
    }

    /// <summary>
    /// Registers all Canvas UI elements and Interactables on the scene as AXRIF colliders and
    /// caches them in local collections.
    /// </summary>
    public void RegisterAllColliders() {
      if (_isRegistered) {
        return;
      }

      RegisterInteractables();
      RegisterAllCanvases();

      _isRegistered = true;
    }

    /// <summary>
    /// Unregisters all currently cached interactables and UI elements and removes their
    /// corresponding AXRIF colliders.
    /// </summary>
    public void UnregisterAllColliders() {
      if (!_isRegistered) {
        return;
      }

      UnregisterInteractables();

      var idsToRemove = new List<int>(_colliderSet.Keys);
      foreach (var id in idsToRemove) {
        RemoveColliderFromSubsystem(id);
      }
      _trackedColliders.Clear();
      _colliderSet.Clear();
      _isRegistered = false;
    }

    /// <summary>
    /// Registers all current and future interactables with the subsystem.
    /// </summary>
    private void RegisterInteractables() {
      if (_axrifSubsystem == null) {
        Debug.LogWarning("[AxrifSceneIntegrator:RegisterInteractables] " +
                         "Failed to retrieve valid AxrifSubsystem reference");
        return;
      }

      if (_xrInteractionManager == null) {
        Debug.LogWarning("[AxrifSceneIntegrator:RegisterInteractables] " +
                         "Failed to retrieve valid XRInteractionManager reference");
        return;
      }

      _xrInteractionManager.interactableRegistered += OnInteractableRegistered;
      _xrInteractionManager.interactableUnregistered += OnInteractableUnregistered;

      // Register all interactables colliders already present in the scene.
      List<IXRInteractable> interactables = new List<IXRInteractable>();
      _xrInteractionManager.GetRegisteredInteractables(interactables);
      foreach (var interactable in interactables) {
        RegisterInteractableColliders(interactable);
      }
    }

    /// <summary>
    /// Unregisters IXRInteractable callbacks.
    /// </summary>
    private void UnregisterInteractables() {
      // Ensure the manager is still valid before attempting to unsubscribe or query.
      if (_xrInteractionManager != null) {
        _xrInteractionManager.interactableRegistered -= OnInteractableRegistered;
        _xrInteractionManager.interactableUnregistered -= OnInteractableUnregistered;
      }
    }

    /// <summary>
    /// Registers all found Canvas UI elements as AXRIF colliders.
    /// </summary>
    private void RegisterAllCanvases() {
      Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
      foreach (var canvas in allCanvases) {
        RegisterCanvas(canvas);
      }
    }

    /// <summary>
    /// Handler for when an interactable is registered.
    /// </summary>
    /// <param name="eventArgs">The event arguments containing the registered interactable.</param>
    private void OnInteractableRegistered(InteractableRegisteredEventArgs eventArgs) {
      if (_axrifSubsystem == null || eventArgs.interactableObject.colliders.Count == 0) {
        return;
      }
      RegisterInteractableColliders(eventArgs.interactableObject);
    }

    /// <summary>
    /// Handler for when an interactable is unregistered.
    /// </summary>
    /// <param name="eventArgs">The event arguments containing the unregistered
    /// interactable.</param>
    private void OnInteractableUnregistered(InteractableUnregisteredEventArgs eventArgs) {
      if (_axrifSubsystem == null || eventArgs.interactableObject.colliders.Count == 0) {
        return;
      }
      UnregisterInteractableColliders(eventArgs.interactableObject);
    }

    /// <summary>
    /// Registers all colliders referenced by the provided IXRInteractable object.
    /// </summary>
    /// <param name="interactable">An IXRInteractable object with attached colliders.</param>
    private void RegisterInteractableColliders(IXRInteractable interactable) {
      if (interactable.colliders.Count == 0) {
        return;
      }
      foreach (var unityCollider in interactable.colliders) {
        RegisterCollider(unityCollider);
      }
    }
    /// <summary>
    /// Unregisters all colliders referenced by the provided IXRInteractable object.
    /// </summary>
    /// <param name="interactable">An IXRInteractable object with attached colliders.</param>
    private void UnregisterInteractableColliders(IXRInteractable interactable) {
      foreach (var unityCollider in interactable.colliders) {
        RemoveCollider(unityCollider.GetInstanceID());
      }
    }

    /// <summary>
    /// Tries to add or update an AXRIF collider.
    /// </summary>
    /// <param name="axrifCollider">The collider to add or update.</param>
    /// <returns>True if the collider has been successfully added or updated, false
    /// otherwise.</returns>
    private bool SetCollider(Collider axrifCollider) {
      if (_axrifSubsystem == null) {
        return false;
      }

      _axrifSubsystem.SetCollider(axrifCollider);
      _colliderSet[axrifCollider.Id] = axrifCollider;
      return true;
    }

    /// <summary>
    /// Tries to remove an AXRIF collider by id and cleans up local state.
    /// </summary>
    /// <param name="id">The unique id of the collider to remove.</param>
    /// <returns>True if the collider has been successfully removed, false otherwise.</returns>
    private bool RemoveCollider(int id) {
      bool wasRemovedFromSubsystem = RemoveColliderFromSubsystem(id);
      for (int i = 0; i < _trackedColliders.Count; i++) {
        if (_trackedColliders[i].Id == id) {
          _trackedColliders.FastRemoveAt(i);
          break;
        }
      }
      return wasRemovedFromSubsystem;
    }

    /// <summary>
    /// Helper method to remove an AXRIF collider by id from the subsystem and collider set
    /// without modifying the tracked list.
    /// </summary>
    private bool RemoveColliderFromSubsystem(int id) {
      if (_axrifSubsystem == null) {
        return false;
      }

      if (_colliderSet.TryGetValue(id, out Collider axrifCollider)) {
        _axrifSubsystem.RemoveCollider(axrifCollider);
      }

      // Always remove local state to prevent ghost colliders
      _colliderSet.Remove(id);
      return true;
    }
  }
}  // namespace Android.XR.InteractionFramework
