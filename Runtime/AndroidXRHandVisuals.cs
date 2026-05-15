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

using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Meshing;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// Manages the visual representation of a single hand in the XR environment.
  /// Handles mesh updates, material assignment, outlining, and switching between VR and Passthrough
  /// visual modes for this hand instance.
  /// </summary>
  public class AndroidXRHandVisuals : MonoBehaviour {
    private static readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int _dstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int _srcBlendAlpha = Shader.PropertyToID("_SrcBlendAlpha");
    private static readonly int _dstBlendAlpha = Shader.PropertyToID("_DstBlendAlpha");
    private static readonly int _fadeTransformPropID = Shader.PropertyToID("_FadeTransform");

    /// <summary>
    /// Gets or sets the outline width for this hand visual.
    /// Updates the shader properties on this instance's <see cref="Renderer"/> when changed.
    /// </summary>
    public float OutlineWidth {
      get => _outlineWidth;
      set {
        if (value != _outlineWidth) {
          _outlineWidth = value;
          SetRendererFloat(_handMeshRenderer, "_Offset", _outlineWidth, 2);
        }
      }
    }

    /// <summary>
    /// Gets or sets the base opacity for this hand visual.
    /// Updates the shader properties on this instance's <see cref="Renderer"/> when changed.
    /// </summary>
    public float BaseOpacity {
      get => _baseOpacity;
      set {
        if (value != _baseOpacity) {
          _baseOpacity = value;
          SetRendererFloat(_handMeshRenderer, "_AlphaMultiplier", _baseOpacity, 0);
        }
      }
    }

    /// <summary>
    /// Gets or sets the visibility state of this hand's mesh renderer.
    /// </summary>
    public bool IsVisible {
      get => _isVisible;
      set {
        if (_isVisible != value) {
          _isVisible = value;
          if (_handMeshRenderer != null) {
            _handMeshRenderer.enabled = _isVisible;
          }
        }
      }
    }

    /// <summary>
    /// Specifies the handedness (Left, Right, Invalid) this visual represents.
    /// </summary>
    public UnityEngine.XR.Hands.Handedness handedness = UnityEngine.XR.Hands.Handedness.Invalid;

    /// <summary>
    /// Gets the primary <see cref="Renderer"/> component used for the hand mesh.
    /// </summary>
    public Renderer HandMeshRenderer => _handMeshRenderer;

    /// <summary>
    /// Gets the runtime instance of the base color material.
    /// </summary>
    public Material RuntimeBaseColorMat { get; private set; }

    /// <summary>
    /// Gets the runtime instance of the outline material.
    /// </summary>
    public Material RuntimeOutlineMat { get; private set; }

    /// <summary>
    /// Gets or sets the root pose of the hand, used for shader transformations.
    /// </summary>
    public Pose RootPose { get; set; }

    [SerializeField]
    private Material _baseColorMat;
    [SerializeField]
    private Material _baseWriteDepthMat;
    [SerializeField]
    private Material _outlineMat;
    [SerializeField]
    private Renderer _handMeshRenderer;
    [SerializeField]
    private MeshFilter _handMeshFilter;
    [SerializeField]
    private BlendMode _passthroughSrcColorBlend;
    [SerializeField]
    private BlendMode _passthroughDstColorBlend;
    [SerializeField]
    private BlendMode _vrSrcColorBlend;
    [SerializeField]
    private BlendMode _vrDstColorBlend;
    [SerializeField]
    private BlendMode _passthroughSrcAlphaBlend;
    [SerializeField]
    private BlendMode _passthroughDstAlphaBlend;
    [SerializeField]
    private BlendMode _vrSrcAlphaBlend;
    [SerializeField]
    private BlendMode _vrDstAlphaBlend;

    private float _outlineWidth = 0.0007f;
    private float _baseOpacity = 0.4f;
    private bool _isVisible = true;

    /// <summary>
    /// A standalone function to set a float value on a <see cref="Renderer"/>'s material.
    /// </summary>
    /// <param name="targetRenderer">The renderer to modify.</param>
    /// <param name="keyword">The string name of the shader property.</param>
    /// <param name="floatValue">The float value to set.</param>
    /// <param name="materialIndex">The material index (only used when not using a
    /// <see cref="MaterialPropertyBlock"/>).</param>
    public static void SetRendererFloat(Renderer targetRenderer, string keyword, float floatValue,
                                        int materialIndex = 0) {
      if (targetRenderer == null) {
        return;
      }

      // This logic determines whether to use a MaterialPropertyBlock or modify the material
      // directly.
      bool isUsingSRP = GraphicsSettings.currentRenderPipeline != null;
      bool useMaterialPropertyBlock = !Application.isPlaying || !isUsingSRP;

      int propertyID = Shader.PropertyToID(keyword);

      if (useMaterialPropertyBlock) {
        var properties = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(properties);
        properties.SetFloat(propertyID, floatValue);
        targetRenderer.SetPropertyBlock(properties);
      } else {
        Material runtimeMat;
        if (targetRenderer.sharedMaterials[materialIndex].GetInstanceID() < -1) {
          runtimeMat = targetRenderer.sharedMaterials[materialIndex];
        } else {
          // This call gets or creates a unique runtime material instance
          runtimeMat = targetRenderer.materials[materialIndex];
        }

        runtimeMat.SetFloat(propertyID, floatValue);
      }
    }

    /// <summary>
    /// Assigns the instantiated runtime materials (Base, Depth, Outline) to the specified <see
    /// cref="Renderer"/>.
    /// </summary>
    /// <param name="handRenderer">The <see cref="Renderer"/> to assign materials to.</param>
    public void AssignMaterials(Renderer handRenderer) {
      if (RuntimeBaseColorMat != null) {
        handRenderer.sharedMaterials =
            new[] { RuntimeBaseColorMat, _baseWriteDepthMat, RuntimeOutlineMat };
      } else {
        handRenderer.sharedMaterials = new[] { _baseWriteDepthMat, RuntimeOutlineMat };
      }
    }

    /// <summary>
    /// Updates the shader matrices based on the provided <see cref="XRHand"/>'s root pose.
    /// </summary>
    /// <param name="hand">The <see cref="XRHand"/> data source.</param>
    public void RefreshHandMatrix(XRHand hand) {
      RefreshHandMatrix(hand.rootPose.position, hand.rootPose.rotation, Vector3.one);
    }

    /// <summary>
    /// Updates the shader matrices for the hand visuals.
    /// Calculates the inverse transform matrix and applies it to the runtime materials.
    /// </summary>
    /// <param name="position">The position of the hand root.</param>
    /// <param name="rotation">The rotation of the hand root.</param>
    /// <param name="scale">The scale of the hand.</param>
    public void RefreshHandMatrix(Vector3 position, Quaternion rotation, Vector3 scale) {
      var handRootMatrix = Matrix4x4.TRS(position, rotation, Vector3.one);
      var handRootMatrixInverse = handRootMatrix.inverse;
      RuntimeOutlineMat?.SetMatrix(_fadeTransformPropID, handRootMatrixInverse);
      RuntimeBaseColorMat?.SetMatrix(_fadeTransformPropID, handRootMatrixInverse);
    }

    /// <summary>
    /// Updates the mesh filter and renderer with vertex, normal, and index data from
    /// <see cref="XRHandMeshData"/>.
    /// </summary>
    /// <param name="meshData">The mesh data containing positions, normals, and indices.</param>
    public void SetHandMeshData(XRHandMeshData meshData) {
      if (meshData.TryGetRootPose(out var rootPose)) {
        transform.SetWorldPose(rootPose);
        if (_handMeshFilter != null && _handMeshFilter.sharedMesh == null) {
          _handMeshFilter.sharedMesh = new Mesh();
          _handMeshRenderer.allowOcclusionWhenDynamic = false;
          _handMeshRenderer.localBounds = new Bounds(
              Vector3.zero, Vector3.one * 100f);  // make sure dynamic mesh always gets rendered
          _handMeshFilter.sharedMesh.SetVertices(meshData.positions);
          _handMeshFilter.sharedMesh.SetNormals(meshData.normals);
          _handMeshFilter.sharedMesh.SetIndices(meshData.indices, MeshTopology.Triangles, 0);
        } else {
          _handMeshFilter.sharedMesh.SetVertices(meshData.positions);
          _handMeshFilter.sharedMesh.SetNormals(meshData.normals);
        }
      }
    }

    /// <summary>
    /// Updates material blend properties based on whether passthrough is active.
    /// </summary>
    /// <param name="passthrough">True for passthrough mode blending, False for VR mode
    /// blending.</param>
    public void RefreshHandOpacity(bool passthrough) {
      var srcAlphaBlend = passthrough ? _passthroughSrcAlphaBlend : _vrSrcAlphaBlend;
      var dstAlphaBlend = passthrough ? _passthroughDstAlphaBlend : _vrDstAlphaBlend;
      if (RuntimeBaseColorMat) {
        RuntimeBaseColorMat.SetFloat(_srcBlend,
                                     (float)(passthrough ? BlendMode.Zero : _vrSrcColorBlend));
        RuntimeBaseColorMat.SetFloat(_dstBlend,
                                     (float)(passthrough ? BlendMode.One : _vrDstColorBlend));
        RuntimeBaseColorMat.SetFloat(_srcBlendAlpha,
                                     (float)(passthrough ? BlendMode.Zero : srcAlphaBlend));
        RuntimeBaseColorMat.SetFloat(_dstBlendAlpha,
                                     (float)(passthrough ? BlendMode.One : dstAlphaBlend));
      }
      if (RuntimeOutlineMat) {
        RuntimeOutlineMat.SetFloat(
            _srcBlend, (float)(passthrough ? _passthroughSrcColorBlend : _vrSrcColorBlend));
        RuntimeOutlineMat.SetFloat(
            _dstBlend, (float)(passthrough ? _passthroughDstColorBlend : _vrDstColorBlend));
        RuntimeOutlineMat.SetFloat(_srcBlendAlpha, (float)srcAlphaBlend);
        RuntimeOutlineMat.SetFloat(_dstBlendAlpha, (float)dstAlphaBlend);
      }
    }

    private void Awake() {
      if (_baseColorMat) {
        RuntimeBaseColorMat = Instantiate(_baseColorMat);
      }
      if (_outlineMat) {
        RuntimeOutlineMat = Instantiate(_outlineMat);
      }
      AssignMaterials(_handMeshRenderer);
      // Ensure _isVisible state matches renderer state on awake
      if (_handMeshRenderer != null) {
        _isVisible = _handMeshRenderer.enabled;
      }
    }

    private void Update() {
      if (handedness != UnityEngine.XR.Hands.Handedness.Invalid) {
        if (!this.enabled) {
          return;
        }
        var targetHandInverseMatrix =
            Matrix4x4.TRS(this.RootPose.position, this.RootPose.rotation, Vector3.one).inverse;
        RuntimeOutlineMat?.SetMatrix(_fadeTransformPropID, targetHandInverseMatrix);
        RuntimeBaseColorMat?.SetMatrix(_fadeTransformPropID, targetHandInverseMatrix);
      }
    }
  }
}
