// Copyright 2025 Google LLC
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// Manages the hand mesh visualization process by interfacing with the XR Hand Subsystem.
  /// It subscribes to hand tracking events, retrieves mesh data, and passes it to
  /// <see cref="AndroidXRHandVisuals"/> components. This component is designed to be a singleton.
  /// </summary>
  public class AndroidXRHandMeshVisualizer : MonoBehaviour {
    private static AndroidXRHandMeshVisualizer _instance;

    /// <summary>
    /// Singleton instance accessor for AndroidXRHandMeshVisualizer.
    /// Finds the first object of this type if the instance is null.
    /// </summary>
    public static AndroidXRHandMeshVisualizer Instance {
      get {
        if (_instance == null) {
          _instance =
              FindFirstObjectByType<AndroidXRHandMeshVisualizer>(FindObjectsInactive.Include);
        }

        return _instance;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this visualizer is currently subscribed to
    /// <see cref="XRHandSubsystem"/> events.
    /// </summary>
    public bool SubscribedToHandEvents { get; private set; }

    [SerializeField]
    private AndroidXRHandVisuals _handVisualPrefab;

    private XRHandSubsystem _handSubsystem;
    private AndroidXRHandVisuals _leftHandVisual;
    private AndroidXRHandVisuals _rightHandVisual;
    private AndroidXRHandVisuals[] _handVisuals;

    private XRHandMeshDataQueryParams _handMeshQuery =
        new XRHandMeshDataQueryParams { allocator = Allocator.Persistent };

#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// Retrieves all active <see cref="AndroidXRHandVisuals"/> instances in the scene.
    /// Caches the result after the first call.
    /// </summary>
    /// <returns>An array of <see cref="AndroidXRHandVisuals"/>.</returns>
    public AndroidXRHandVisuals[] GetHandVisuals() {
      if (_handVisuals == null) {
        _handVisuals = FindObjectsByType<AndroidXRHandVisuals>(FindObjectsSortMode.None);
      }

      return _handVisuals;
    }

    /// <summary>
    /// Forces a refresh of opacity and blend modes for all hand visuals based on the passthrough
    /// state.
    /// </summary>
    /// <param name="passthrough">True if passthrough is active, false for VR mode.</param>
    public void RefreshHandVisuals(bool passthrough) {
      foreach (var handVisual in GetHandVisuals()) {
        if (handVisual != null) {
          handVisual.RefreshHandOpacity(passthrough);
        }
      }
    }

    /// <summary>
    /// Initializes the singleton instance and enforces that only one instance exists.
    /// </summary>
    private void Awake() {
      if (_instance == null) {
        _instance = this;
      } else if (_instance != this) {
        Debug.LogWarning(
            "[AndroidXRHandMeshVisualizer:Awake] - Another instance already exists. Destroying this one.");
        Destroy(this);
      }
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Refreshes the <see cref="XRHandSubsystem"/> reference and subscribes to hand events if a
    /// subsystem is found.
    /// </summary>
    private void OnEnable() {
      RefreshHandSubsystem();
      if (_handSubsystem != null && !SubscribedToHandEvents) {
        SubscribeToHandEvents();
      }
    }

    /// <summary>
    /// Called on the frame when a script is first enabled, before any of the Update methods.
    /// Checks for the presence of the hand visual prefab, refreshes the <see
    /// cref="XRHandSubsystem"/>, and subscribes to hand events if not already subscribed.
    /// </summary>
    private void Start() {
      if (_handVisualPrefab == null) {
        Debug.LogError(
            $"[AndroidXRHandMeshVisualizer:Start] - Hand visual prefab missing on {this.gameObject.name}.");
      }

      RefreshHandSubsystem();

      if (!SubscribedToHandEvents) {
        SubscribeToHandEvents();
      }
    }

    /// <summary>
    /// Called every frame. Tries to get the latest hand mesh data from the subsystem
    /// to ensure the visual representation is synchronized with the latest tracking data.
    /// Updates the <see cref="AndroidXRHandVisuals"/> components if new data is available.
    /// Instantiates the hand visuals if they don't exist yet.
    /// </summary>
    private void Update() {
      if (_handSubsystem != null) {
        if (_handSubsystem.TryGetMeshData(out var data, ref _handMeshQuery)) {
          if (_leftHandVisual == null) {
            _leftHandVisual = Instantiate(_handVisualPrefab, transform);
            _leftHandVisual.transform.SetWorldPose(Pose.identity);
            _leftHandVisual.gameObject.name = "Left Hand Mesh Visual";
            _leftHandVisual.handedness = UnityEngine.XR.Hands.Handedness.Left;

            data.leftHand.TryGetRootPose(out Pose rootPose);
            _leftHandVisual.RootPose = rootPose;
          }

          if (_rightHandVisual == null) {
            _rightHandVisual = Instantiate(_handVisualPrefab, transform);
            _rightHandVisual.transform.SetWorldPose(Pose.identity);
            _rightHandVisual.gameObject.name = "Right Hand Mesh Visual";
            _rightHandVisual.handedness = UnityEngine.XR.Hands.Handedness.Right;

            data.rightHand.TryGetRootPose(out Pose rootPose);
            _rightHandVisual.RootPose = rootPose;
          }

          if (_leftHandVisual != null) {
            _leftHandVisual.SetHandMeshData(data.leftHand);
          }

          if (_rightHandVisual != null) {
            _rightHandVisual.SetHandMeshData(data.rightHand);
          }

          data.rightHand.Dispose();
          data.leftHand.Dispose();
        }
      } else {
        RefreshHandSubsystem();
        if (!SubscribedToHandEvents) {
          SubscribeToHandEvents();
        }
      }
    }

    /// <summary>
    /// Called when the object becomes disabled or inactive.
    /// Unsubscribes from all <see cref="XRHandSubsystem"/> events to prevent memory leaks and
    /// errors.
    /// </summary>
    private void OnDisable() {
      if (_handSubsystem != null && SubscribedToHandEvents) {
        _handSubsystem.updatedHands -= OnHandsUpdated;
        _handSubsystem.trackingAcquired -= OnHandTrackingAcquired;
        _handSubsystem.trackingLost -= OnHandTrackingLost;
        SubscribedToHandEvents = false;
      }
    }
    /// <summary>
    /// Searches for and sets the active <see cref="XRHandSubsystem"/> instance.
    /// Logs warnings if no subsystem is found or running.
    /// </summary>
    private void RefreshHandSubsystem() {
      List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();

      if (_handSubsystem == null) {
        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count == 0) {
          Debug.LogWarning(
              "[AndroidXRHandMeshVisualizer:RefreshHandSubsystem] - Hand Tracking Subsystem not found, can't subscribe to hand tracking status. Enable that feature in the OpenXR project settings and ensure OpenXR is enabled as the plug-in provider.",
              this);
        } else {
          _handSubsystem = handSubsystems.FirstOrDefault(x => x.running);
          if (_handSubsystem == null) {
            Debug.Log(
                "[AndroidXRHandMeshVisualizer:RefreshHandSubsystem] - No running hand tracking subsystem found.");
          } else {
            Debug.Log(
                $"[AndroidXRHandMeshVisualizer:RefreshHandSubsystem] - {handSubsystems.Count} Hand Tracking Subsystem found. Using '{_handSubsystem}'");
          }
        }
      }
    }

    /// <summary>
    /// Subscribes this component to the <see cref="XRHandSubsystem"/> events: updatedHands,
    /// trackingAcquired, and trackingLost.
    /// </summary>
    private void SubscribeToHandEvents() {
      _handSubsystem.updatedHands += OnHandsUpdated;
      _handSubsystem.trackingAcquired += OnHandTrackingAcquired;
      _handSubsystem.trackingLost += OnHandTrackingLost;
      SubscribedToHandEvents = true;
    }

    /// <summary>
    /// Event handler for when hand tracking is lost for a specific hand.
    /// Disables the renderer for the corresponding hand visual.
    /// </summary>
    /// <param name="hand">The hand that lost tracking.</param>
    private void OnHandTrackingLost(XRHand hand) {
      switch (hand.handedness) {
        case UnityEngine.XR.Hands.Handedness.Left:
          if (_leftHandVisual != null) {
            _leftHandVisual.HandMeshRenderer.enabled = false;
          }
          break;

        case UnityEngine.XR.Hands.Handedness.Right:
          if (_rightHandVisual != null) {
            _rightHandVisual.HandMeshRenderer.enabled = false;
          }
          break;
      }
    }

    /// <summary>
    /// Event handler for when hand tracking is acquired for a specific hand.
    /// Enables the renderer for the corresponding hand visual.
    /// </summary>
    /// <param name="hand">The hand that acquired tracking.</param>
    private void OnHandTrackingAcquired(XRHand hand) {
      switch (hand.handedness) {
        case UnityEngine.XR.Hands.Handedness.Left:
          if (_leftHandVisual != null) {
            _leftHandVisual.HandMeshRenderer.enabled = true;
          }
          break;

        case UnityEngine.XR.Hands.Handedness.Right:
          if (_rightHandVisual != null) {
            _rightHandVisual.HandMeshRenderer.enabled = true;
          }
          break;
      }
    }

    /// <summary>
    /// Event handler for when the <see cref="XRHandSubsystem"/> updates the hands.
    /// Specifically, during the BeforeRender update type, it refreshes the hand matrix
    /// (the inverse transform for fading/outlining effects) on the visuals.
    /// </summary>
    /// <param name="handSubsystem">The <see cref="XRHandSubsystem"/> that updated.</param>
    /// <param name="flag">Flags indicating the success of the update.</param>
    /// <param name="updateType">The type of update that occurred (e.g., BeforeRender).</param>
    private void OnHandsUpdated(XRHandSubsystem handSubsystem,
                                XRHandSubsystem.UpdateSuccessFlags flag,
                                XRHandSubsystem.UpdateType updateType) {
      if (updateType == XRHandSubsystem.UpdateType.BeforeRender) {
        if (_leftHandVisual != null) {
          _leftHandVisual.RefreshHandMatrix(handSubsystem.leftHand);
        }

        if (_rightHandVisual != null) {
          _rightHandVisual.RefreshHandMatrix(handSubsystem.rightHand);
        }
      }
    }
#endif
  }
}
