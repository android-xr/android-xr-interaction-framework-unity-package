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

using NUnit.Framework;
using UnityEngine;
using System;

namespace Android.XR.InteractionFramework {

  /// This test class exercises the functionality of the Android XR Interaction Framework Native
  /// Library. If the underlying cc signatures change, this file must also be edited to match.
  public class NativeApiTest {
    IntPtr context = IntPtr.Zero;

    [SetUp]
    public void PluginSetUp() {
      context = IntPtr.Zero;
    }

    [TearDown]
    public void PluginTearDown() {
      NativeApi.Axrif_Destroy(context);
    }

    [Test]
    public void CreateWhenNotCreatedPopulatesContext() {
      Configurations configurations = new Configurations();
      NativeApi.Axrif_Create(in configurations, ref context);

      Assert.That(context, Is.Not.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void CreateWhenCreatedReturnsExistingContext() {
      Configurations configurations = new Configurations();
      NativeApi.Axrif_Create(in configurations, ref context);

      IntPtr expectedContext = context;

      NativeApi.Axrif_Create(in configurations, ref context);

      Assert.That(context, Is.EqualTo(expectedContext));
    }

    [Test]
    public void UpdateConfigurationsDisablesInteractions() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsHandRaycastEnabled = true };
      HeadSourceState headSourceState = new HeadSourceState { IsValid = true };
      HandsSourceState handsSourceState =
          new HandsSourceState { IsValid = true, LeftHand = { IsActive = true } };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);

      configurations.IsHandRaycastEnabled = false;
      NativeApi.Axrif_UpdateConfigurations(context, in configurations);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);
    }

    [Test]
    public void UpdateConfigurationsEnablesInteractions() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsHandRaycastEnabled = false };
      HeadSourceState headSourceState = new HeadSourceState { IsValid = true };
      HandsSourceState handsSourceState =
          new HandsSourceState { IsValid = true, LeftHand = { IsActive = true } };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      configurations.IsHandRaycastEnabled = true;
      NativeApi.Axrif_UpdateConfigurations(context, in configurations);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void SetColliderWithNewColliderAddsCollider() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsHandPokeEnabled = true };
      HandsSourceState handsSourceState = new HandsSourceState {
        IsValid = true,
        LeftHand =
            new HandState { IsActive = true, PokePose = new Posef(new Vector3f(0f, 0f, 0f),
                                                                  new Quaternionf(0f, 0f, 0f, 1f)) }
      };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      Collider collider = new Collider { Id = 1,
                                         Type = Collider.ColliderType.Rect,
                                         IsTouchable = true,
                                         Center = new Vector3f(0f, 0f, -0.01f),
                                         Rotation = new Quaternionf(0f, 0f, 0f, 1f),
                                         Length = 20f,
                                         Height = 10f };
      NativeApi.Axrif_SetCollider(context, in collider);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      // Poke interaction is only active if there's a touchable collider nearby, so successfully
      // getting a PointerEvent confirms the collider was added and is working as expected.
      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(1));
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.HandPoke));
    }

    [Test]
    public void SetColliderWithExistingColliderUpdatesCollider() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsHandPokeEnabled = true };
      HandsSourceState handsSourceState = new HandsSourceState {
        IsValid = true,
        LeftHand =
            new HandState { IsActive = true, PokePose = new Posef(new Vector3f(0f, 0f, 0f),
                                                                  new Quaternionf(0f, 0f, 0f, 1f)) }
      };

      // Add a collider at a distance that is too far away to interact with.
      Collider collider = new Collider { Id = 1,
                                         Type = Collider.ColliderType.Rect,
                                         IsTouchable = true,
                                         Center = new Vector3f(0f, 0f, 100f),
                                         Rotation = new Quaternionf(0f, 0f, 0f, 1f),
                                         Length = 20f,
                                         Height = 10f };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_SetCollider(context, in collider);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      // Update the collider to a distance that is close enough to interact with.
      collider.Center = new Vector3f(0f, 0f, -0.01f);
      NativeApi.Axrif_SetCollider(context, in collider);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void RemoveColliderMakesHandPokeUnavailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsHandPokeEnabled = true };
      HandsSourceState handsSourceState = new HandsSourceState {
        IsValid = true,
        LeftHand =
            new HandState { IsActive = true, PokePose = new Posef(new Vector3f(0f, 0f, 0f),
                                                                  new Quaternionf(0f, 0f, 0f, 1f)) }
      };

      Collider collider = new Collider { Id = 1,
                                         Type = Collider.ColliderType.Rect,
                                         IsTouchable = true,
                                         Center = new Vector3f(0f, 0f, -0.01f),
                                         Rotation = new Quaternionf(0f, 0f, 0f, 1f),
                                         Length = 20f,
                                         Height = 10f };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_SetCollider(context, in collider);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);

      NativeApi.Axrif_RemoveCollider(context, /*colliderId=*/1);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);
    }

    [Test]
    public void ClearAllCollidersMakesHandPokeUnavailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsHandPokeEnabled = true };
      HandsSourceState handsSourceState = new HandsSourceState {
        IsValid = true,
        LeftHand =
            new HandState { IsActive = true, PokePose = new Posef(new Vector3f(0f, 0f, 0f),
                                                                  new Quaternionf(0f, 0f, 0f, 1f)) }
      };

      Collider collider = new Collider { Id = 1,
                                         Type = Collider.ColliderType.Rect,
                                         IsTouchable = true,
                                         Center = new Vector3f(0f, 0f, -0.01f),
                                         Rotation = new Quaternionf(0f, 0f, 0f, 1f),
                                         Length = 20f,
                                         Height = 10f };

      Collider anotherCollider = new Collider { Id = 2,
                                                Type = Collider.ColliderType.Rect,
                                                IsTouchable = true,
                                                Center = new Vector3f(0f, 0f, -0.01f),
                                                Rotation = new Quaternionf(0f, 0f, 0f, 1f),
                                                Length = 10f,
                                                Height = 5f };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_SetCollider(context, in collider);
      NativeApi.Axrif_SetCollider(context, in anotherCollider);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);

      NativeApi.Axrif_ClearAllColliders(context);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);
    }

    [Test]
    public void UpdateControllersSourceStateMakesControllerPointerAvailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsControllerEnabled = true };
      ControllersSourceState controllersSourceState =
          new ControllersSourceState { IsValid = true,
                                       LeftController = new ControllerState { IsActive = true } };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      NativeApi.Axrif_UpdateControllersSourceState(context, in controllersSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void UpdateHeadSourceStateMakesGazePointerAvailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsGazePinchEnabled = true };
      EyesSourceState eyesSourceState = new EyesSourceState { IsValid = true };
      HeadSourceState headSourceState = new HeadSourceState { IsValid = true };
      HandsSourceState handsSourceState = new HandsSourceState { IsValid = true };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void UpdateHandsSourceStateMakesGazePointerAvailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsGazePinchEnabled = true };
      EyesSourceState eyesSourceState = new EyesSourceState { IsValid = true };
      HeadSourceState headSourceState = new HeadSourceState { IsValid = true };
      HandsSourceState handsSourceState = new HandsSourceState { IsValid = true };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void UpdateEyesSourceStateMakesGazePointerAvailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsGazePinchEnabled = true };
      EyesSourceState eyesSourceState = new EyesSourceState { IsValid = true };
      HeadSourceState headSourceState = new HeadSourceState { IsValid = true };
      HandsSourceState handsSourceState = new HandsSourceState { IsValid = true };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void UpdateMouseSourceStateMakesMousePointerAvailable() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations { IsMouseEnabled = true };
      MouseSourceState mouseSourceState = new MouseSourceState { IsValid = true };

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);

      NativeApi.Axrif_UpdateMouseSourceState(context, in mouseSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
    }

    [Test]
    public void GetPointerEventReturnsInvalidEventWhenInteractionNotEnabled() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsGazePinchEnabled = false;
      EyesSourceState eyesSourceState = new EyesSourceState();
      eyesSourceState.IsValid = true;
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);

      NativeApi.Axrif_GetPointerEvent(context, InteractionType.GazePinch,
                                      startTime.ToUnixTimeMilliseconds() * 1_000_000,
                                      out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.False);
    }

    [Test]
    public void GetPointerEventReturnsValidEvent() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsGazePinchEnabled = true;
      HeadSourceState headSourceState = new HeadSourceState();
      headSourceState.IsValid = true;
      EyesSourceState eyesSourceState = new EyesSourceState();
      eyesSourceState.IsValid = true;
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;
      handsSourceState.LeftHand.IsActive = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);

      NativeApi.Axrif_GetPointerEvent(context, InteractionType.GazePinch,
                                      startTime.ToUnixTimeMilliseconds() * 1_000_000,
                                      out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(1));
      Assert.That(pointerEvent.Pointers[0].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[0].IsActive, Is.True);
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.Gaze));
      Assert.That(pointerEvent.Pointers[0].Handedness, Is.EqualTo(Handedness.Both));
    }

    [Test]
    public void GetSelectedPointerEventReturnsExpectedMouseEvent() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsGazePinchEnabled = true;
      configurations.IsHandRaycastEnabled = true;
      configurations.IsMouseEnabled = true;
      MouseSourceState mouseSourceState = new MouseSourceState();
      mouseSourceState.IsValid = true;
      mouseSourceState.AimPose =
          new Posef(new Vector3f(1f, 0f, 0f), new Quaternionf(0f, 0f, 0f, 1f));
      EyesSourceState eyesSourceState = new EyesSourceState();
      eyesSourceState.IsValid = false;
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;
      HeadSourceState headSourceState = new HeadSourceState();
      headSourceState.IsValid = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateMouseSourceState(context, in mouseSourceState);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      // Move the mouse to make it available.
      mouseSourceState.AimPose =
          new Posef(new Vector3f(10f, 0f, 0f), new Quaternionf(0f, 0f, 0f, 1f));
      // Eyes become available.
      eyesSourceState.IsValid = true;
      NativeApi.Axrif_UpdateMouseSourceState(context, in mouseSourceState);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, (startTime.AddSeconds(1)).ToUnixTimeMilliseconds() * 1_000_000,
          out pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(1));
      Assert.That(pointerEvent.Pointers[0].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.Mouse));
      Assert.That(pointerEvent.Pointers[0].Handedness, Is.EqualTo(Handedness.Both));
    }

    [Test]
    public void GetSelectedPointerEventReturnsExpectedGazePinchEvent() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsGazePinchEnabled = true;
      configurations.IsHandRaycastEnabled = true;
      EyesSourceState eyesSourceState = new EyesSourceState();
      eyesSourceState.IsValid = true;
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;
      handsSourceState.LeftHand.IsActive = true;
      HeadSourceState headSourceState = new HeadSourceState();
      headSourceState.IsValid = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(1));
      Assert.That(pointerEvent.Pointers[0].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[0].IsActive, Is.True);
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.Gaze));
      Assert.That(pointerEvent.Pointers[0].Handedness, Is.EqualTo(Handedness.Both));
    }

    [Test]
    public void GetSelectedPointerEventReturnsExpectedHandRaycastEvent() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsGazePinchEnabled = true;
      configurations.IsHandRaycastEnabled = true;
      EyesSourceState eyesSourceState = new EyesSourceState();
      eyesSourceState.IsValid = false;
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;
      handsSourceState.LeftHand.IsActive = true;
      handsSourceState.RightHand.IsActive = true;
      HeadSourceState headSourceState = new HeadSourceState();
      headSourceState.IsValid = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(2));
      Assert.That(pointerEvent.Pointers[0].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.HandRaycast));
      Assert.That(pointerEvent.Pointers[0].Handedness, Is.EqualTo(Handedness.Left));
      Assert.That(pointerEvent.Pointers[1].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[1].Type, Is.EqualTo(Pointer.PointerType.HandRaycast));
      Assert.That(pointerEvent.Pointers[1].Handedness, Is.EqualTo(Handedness.Right));
    }

    [Test]
    public void GetSelectedPointerEventReturnsExpectedOneHandedRaycastEvent() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsGazePinchEnabled = true;
      configurations.IsHandRaycastEnabled = true;
      EyesSourceState eyesSourceState = new EyesSourceState();
      eyesSourceState.IsValid = false;
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;
      handsSourceState.LeftHand.IsActive = false;
      handsSourceState.RightHand.IsActive = true;
      HeadSourceState headSourceState = new HeadSourceState();
      headSourceState.IsValid = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateEyesSourceState(context, in eyesSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(1));
      Assert.That(pointerEvent.Pointers[0].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.HandRaycast));
      Assert.That(pointerEvent.Pointers[0].Handedness, Is.EqualTo(Handedness.Right));
    }

    [Test]
    public void GetSelectedPointerEventReturnsExpectedTwoDifferentModesEvent() {
      DateTimeOffset startTime = DateTimeOffset.UnixEpoch.AddHours(42);
      Configurations configurations = new Configurations();
      configurations.IsControllerEnabled = true;
      configurations.IsHandRaycastEnabled = true;
      ControllersSourceState controllerSourceState = new ControllersSourceState();
      controllerSourceState.IsValid = true;
      controllerSourceState.LeftController.IsActive = true;
      controllerSourceState.LeftController.AimPose =
          new Posef(new Vector3f(1f, 1f, 1f), new Quaternionf(0f, 0f, 0f, 1f));
      HandsSourceState handsSourceState = new HandsSourceState();
      handsSourceState.IsValid = true;
      handsSourceState.RightHand.IsActive = true;
      HeadSourceState headSourceState = new HeadSourceState();
      headSourceState.IsValid = true;

      NativeApi.Axrif_Create(in configurations, ref context);
      NativeApi.Axrif_UpdateControllersSourceState(context, in controllerSourceState);
      NativeApi.Axrif_UpdateHeadSourceState(context, in headSourceState);
      NativeApi.Axrif_UpdateHandsSourceState(context, in handsSourceState);
      NativeApi.Axrif_GetSelectedPointerEvent(
          context, startTime.ToUnixTimeMilliseconds() * 1_000_000, out PointerEvent pointerEvent);

      Assert.That(pointerEvent.IsValid, Is.True);
      Assert.That(pointerEvent.ValidPointerCount, Is.EqualTo(2));
      Assert.That(pointerEvent.Pointers[0].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[0].Type, Is.EqualTo(Pointer.PointerType.Controller));
      Assert.That(pointerEvent.Pointers[0].Handedness, Is.EqualTo(Handedness.Left));
      Assert.That(pointerEvent.Pointers[1].IsValid, Is.True);
      Assert.That(pointerEvent.Pointers[1].Type, Is.EqualTo(Pointer.PointerType.HandRaycast));
      Assert.That(pointerEvent.Pointers[1].Handedness, Is.EqualTo(Handedness.Right));
    }
  }
}  // namespace Android.XR.InteractionFramework
