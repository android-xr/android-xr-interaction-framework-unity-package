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
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Controls a reticle for an XR interactor.
  /// Manages the instantiation, positioning, distance-based scaling, and smoothing
  /// of a reticle prefab based on raycast hits from an XRI Interactor/>.
  /// </summary>
  public class AxrifInteractorReticle : MonoBehaviour {
    /// <summary>
    /// The maximum number of raycasts to store.
    /// </summary>
    const int MaxRaycastHits = 10;
    /// <summary>
    /// The time it takes to smooth from last hit distance to the default distance.
    /// </summary>
    const float DistanceSmoothingTime = 0.016f;
    [SerializeField, Tooltip("The prefab to use as the reticle. Will be " + "instantiated.")]
    private GameObject _reticlePrefab;

    [SerializeField,
     Tooltip("The default distance to display the reticle when no hit has occurred yet.")]
    private float _defaultDistance = 1f;

    [SerializeField, Tooltip("Base scaling factor for the reticle prefab.")]
    private float _prefabScalingFactor = 1f;

    [SerializeField, Tooltip("The time it takes to smooth the reticle position.")]
    private float _reticleSmoothingTime = 0f;

    [SerializeField, Tooltip("If true, the reticle will be drawn even when no raycast hit occurs.")]
    private bool _drawOnNoHit = true;

    [SerializeField, Tooltip("If true, the reticle will scale with distance " +
                             "to maintain a consistent apparent size.")]
    private bool _undoDistanceScaling = true;

    [SerializeField, Tooltip("The maximum distance to cast rays.")]
    private float _maxRaycastDistance = 10;

    [SerializeField, Tooltip("Layer mask to filter raycast targets.")]
    private LayerMask _raycastMask = -1;

    [SerializeField, Tooltip("If true, the reticle remains visible even when " +
                             "the interactor is selecting an object.")]
    private bool _drawWhileSelecting = false;

    [SerializeField, Tooltip("The max angle from the last good raycast hit direction to the " +
                             "current ray direction before the last distance changes to the " +
                             "default distance.")]
    private float _maxAngle = 15f;

    /// <summary>
    /// A reference to the reticle.
    /// </summary>
    private GameObject _reticleInstance;

    /// <summary>
    /// The attached XRInteractor
    /// </summary>
    private XRBaseInputInteractor _interactor;

    /// <summary>
    /// The distance from the last successful raycast.
    /// </summary>
    private float _lastHitDistance;

    /// <summary>
    /// Whether or not the interactor is currently selecting.
    /// </summary>
    private bool _isSelecting = false;

    /// <summary>
    /// The direction of the last successful raycast.
    /// </summary>
    private Vector3 _lastHitDirection;

    /// <summary>
    /// Used to prevent reticle swim when the visualization is first activated.
    /// </summary>
    private bool _isPositionInitialized = false;

    /// <summary>
    /// A cache of raycast hits to support multiple raycasts across UI and 3d colliders
    /// </summary>
    private readonly RaycastHit[] _raycastHits = new RaycastHit[MaxRaycastHits];

    /// <summary>
    /// The minimum dot threshold before the reticle distance switches to the default
    /// casting distance. This only happens when there are no successful raycasts.
    /// </summary>
    private float _minDotThreshold;

    /// <summary>
    /// Used to help smooth distance transitions between the last knownd distance and
    /// the default distance of the reticle.
    /// </summary>
    private float _distanceVelocity;

    /// <summary>
    /// Used to help smooth the reticle visualization
    /// </summary>
    private Vector3 _positionVelocity;

    private PhysicsScene _localPhysicsScene;

    private void Awake() {
      _minDotThreshold = Mathf.Cos(_maxAngle * Mathf.Deg2Rad);
      _localPhysicsScene = gameObject.scene.GetPhysicsScene();

      _interactor = GetComponent<XRBaseInputInteractor>();
      if (_interactor == null) {
        Debug.LogError(
            "AxrifInteractorReticle requires an XRBaseInputInteractor component on the " +
                "same GameObject.",
            this);
        enabled = false;
        return;
      }

      if (_reticlePrefab != null) {
        _reticleInstance = Instantiate(_reticlePrefab);
        _reticleInstance.name = "ReticleInstance";
        _reticleInstance.SetActive(false);
      } else {
        Debug.LogError("Reticle Prefab is not set in AxrifInteractorReticle.", this);
        enabled = false;
        return;
      }

      _lastHitDistance = _defaultDistance;
    }

    private void OnEnable() {
      if (_interactor != null) {
        _interactor.selectEntered.AddListener(HandleSelectEntered);
        _interactor.selectExited.AddListener(HandleSelectExited);
        _isSelecting = _interactor.hasSelection;
      }
      _isPositionInitialized = false;
      _lastHitDistance = _defaultDistance;
    }

    private void OnDisable() {
      if (_interactor != null) {
        _interactor.selectEntered.RemoveListener(HandleSelectEntered);
        _interactor.selectExited.RemoveListener(HandleSelectExited);
      }

      SetReticleActive(false);
    }

    private void OnDestroy() {
      if (_reticleInstance != null) {
        Destroy(_reticleInstance);
      }
    }

    /// <summary>
    /// Event handler called when the interactor enters a selection state.
    /// </summary>
    /// <param name="args">The event arguments associated with the select enter event.</param>
    private void HandleSelectEntered(SelectEnterEventArgs args)
    {
      _isSelecting = true;
    }

    /// <summary>
    /// Event handler called when the interactor exits a selection state.
    /// </summary>
    /// <param name="args">The event arguments associated with the select exit event.</param>
    private void HandleSelectExited(SelectExitEventArgs args)
    {
      _isSelecting = false;
    }

    /// <summary>
    /// Sets the active state of the reticle instance and resets its position
    /// initialization if deactivated.
    /// </summary>
    /// <param name="isActive">Whether the reticle should be active or not.</param>
    private void SetReticleActive(bool isActive) {
      if (isActive == _reticleInstance.activeSelf)
        return;

      _reticleInstance.SetActive(isActive);
      if (!isActive) {
        _isPositionInitialized = false;
      }
    }

    /// <summary>
    /// Attempts to perform a raycast using the interactor to find a hit point for the reticle.
    /// </summary>
    /// <param name="position">The output world position of the raycast hit, or Vector3.zero if no
    /// hit.</param> <param name="normal">The output normal of the surface hit, or Vector3.up if no
    /// hit.</param> <param name="distance">The distance to the hit point, or 0f if no hit.</param>
    /// <returns>True if the raycast hits a valid target, false otherwise.</returns>
    private bool TryPerformRaycast(out Vector3 position, out Vector3 normal, out float distance) {
      position = Vector3.zero;
      normal = Vector3.up;
      distance = 0f;

      bool hasRaycastHit = false;
      if (_interactor is XRRayInteractor rayInteractor) {
        if (rayInteractor.TryGetCurrentRaycast(out var raycastHit, out _, out var uiRaycastHit,
                                               out _, out var isUIHitClosest)) {
          if (isUIHitClosest && uiRaycastHit.HasValue) {
            RaycastResult hit = uiRaycastHit.Value;
            if (hit.distance <= _maxRaycastDistance) {
              position = hit.worldPosition;
              normal = hit.worldNormal;
              distance = hit.distance;
              bool isHittingBackOfCanvas =
                  Vector3.Dot(rayInteractor.rayOriginTransform.forward, normal) > 0.0f;
              if (isHittingBackOfCanvas)
                normal *= -1;
              hasRaycastHit = true;
            }
          } else if (raycastHit.HasValue) {
            RaycastHit hit = raycastHit.Value;
            if (hit.distance <= _maxRaycastDistance) {
              position = hit.point;
              normal = hit.normal;
              distance = hit.distance;
              hasRaycastHit = true;
            }
          }
        }
      } else {
        int hitCount = _localPhysicsScene.Raycast(_interactor.attachTransform.position,
                                                  _interactor.attachTransform.forward, _raycastHits,
                                                  _maxRaycastDistance, _raycastMask);
        if (hitCount > 0) {
          int closestHitIndex = 0;
          float minDistance = float.MaxValue;
          for (int i = 0; i < hitCount; ++i) {
            if (_raycastHits[i].distance < minDistance && _raycastHits[i].collider != null) {
              minDistance = _raycastHits[i].distance;
              closestHitIndex = i;
            }
          }

          if (minDistance < float.MaxValue) {
            position = _raycastHits[closestHitIndex].point;
            normal = _raycastHits[closestHitIndex].normal;
            distance = _raycastHits[closestHitIndex].distance;
            hasRaycastHit = true;
          }
        }
      }
      return hasRaycastHit;
    }

    private void LateUpdate() {
      if (_reticleInstance == null || _interactor == null || !_interactor.enabled) {
        SetReticleActive(false);
        return;
      }

      bool hasRaycastHit = TryPerformRaycast(out Vector3 raycastPosition, out Vector3 raycastNormal,
                                             out float currentHitDistance);

      if (hasRaycastHit) {
        _lastHitDistance = currentHitDistance;
        _lastHitDirection = _interactor.attachTransform.forward;
      } else {
        if (Vector3.Dot(_lastHitDirection, _interactor.attachTransform.forward) <
            _minDotThreshold) {
          _lastHitDistance = Mathf.SmoothDamp(_lastHitDistance, _defaultDistance,
                                              ref _distanceVelocity, DistanceSmoothingTime);
        } else {
          _distanceVelocity = 0f;
        }
        raycastPosition = _interactor.attachTransform.position +
                          _interactor.attachTransform.forward * _lastHitDistance;
      }

      bool shouldBeActive = true;
      if (!_drawWhileSelecting && _isSelecting) {
        shouldBeActive = false;
      }
      if (!hasRaycastHit && !_drawOnNoHit) {
        shouldBeActive = false;
      }

      SetReticleActive(shouldBeActive);

      if (!_reticleInstance.activeSelf) {
        return;
      }

      // Update Position with Smoothing
      if (_reticleSmoothingTime > 0 && _isPositionInitialized) {
        _reticleInstance.transform.position = Vector3.SmoothDamp(
            _reticleInstance.transform.position, raycastPosition, ref _positionVelocity,
            _reticleSmoothingTime, float.MaxValue, Time.unscaledDeltaTime);
      } else {
        _reticleInstance.transform.position = raycastPosition;
        _positionVelocity = Vector3.zero;
        _isPositionInitialized = true;
      }

      Camera mainCamera = Camera.main;
      if (hasRaycastHit) {
        _reticleInstance.transform.rotation = Quaternion.LookRotation(raycastNormal);
      } else if (mainCamera != null) {
        _reticleInstance.transform.LookAt(mainCamera.transform);
      } else {
        _reticleInstance.transform.rotation =
            Quaternion.LookRotation(_interactor.attachTransform.forward);
      }

      float scale = _prefabScalingFactor;
      if (_undoDistanceScaling) {
        float distanceToCamera = mainCamera != null
                                     ? Vector3.Distance(mainCamera.transform.position,
                                                        _reticleInstance.transform.position)
                                     : _lastHitDistance;
        scale *= distanceToCamera;
      }
      _reticleInstance.transform.localScale = new Vector3(scale, scale, scale);
    }
}
}  // namespace Android.XR.InteractionFramework
