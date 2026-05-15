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

using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Android.XR.InteractionFramework {
  public class AxrifSubsystemTest {
    public AxrifSubsystem _subsystem;

    [SetUp]
    public void Setup() {
      if (!AxrifSubsystemUtils.TryGetSubsystem(out _subsystem)) {
        Assert.Fail("Subsystem is not available");
      }

      Assert.NotNull(_subsystem);
    }

    [TearDown]
    public void TearDown() {
      _subsystem?.Destroy();
    }

    [UnityTest]
    public IEnumerator StartAndStopSubsystem() {
      if (_subsystem == null) {
        yield break;
      }

      _subsystem.Start();
      yield return null;

      Assert.IsTrue(_subsystem.running);

      _subsystem.Stop();
      yield return null;

      Assert.IsFalse(_subsystem.running);
    }

    [UnityTest]
    public IEnumerator InputModeChangedEvent() {
      if (_subsystem == null) {
        yield break;
      }

      var mockProvider = _subsystem.provider as AxrifSubsystemMockProvider;
      Assert.NotNull(mockProvider);

      InteractionType? leftMode = null;
      InteractionType? rightMode = null;

      _subsystem.OnInputModeChangedLeft += (mode) => leftMode = mode;
      _subsystem.OnInputModeChangedRight += (mode) => rightMode = mode;

      _subsystem.Start();
      yield return null;

      // Set left hand HandRaycast
      mockProvider.SelectedPointerEvent = new PointerEvent {
        IsValid = true, ValidPointerCount = 1,
        Pointers = new Pointer[] { new Pointer { IsValid = true, IsActive = true,
                                                 Type = Pointer.PointerType.HandRaycast,
                                                 Handedness = Handedness.Left } }
      };

      // Wait for the next frame to allow the InputSystem to process updates and trigger
      // onAfterUpdate.
      yield return null;

      Assert.AreEqual(InteractionType.HandRaycast, leftMode);
      Assert.AreEqual(null, rightMode);
      Assert.AreEqual(InteractionType.HandRaycast, _subsystem.CurrentInputModeLeft);
      Assert.AreEqual(null, _subsystem.CurrentInputModeRight);

      // Set right hand Controller
      mockProvider.SelectedPointerEvent = new PointerEvent {
        IsValid = true, ValidPointerCount = 2,
        Pointers = new Pointer[] { new Pointer { IsValid = true, IsActive = true,
                                                 Type = Pointer.PointerType.HandRaycast,
                                                 Handedness = Handedness.Left },
                                   new Pointer { IsValid = true, IsActive = true,
                                                 Type = Pointer.PointerType.Controller,
                                                 Handedness = Handedness.Right } }
      };

      // Wait for the next frame to allow the InputSystem to process updates and trigger
      // onAfterUpdate.
      yield return null;

      Assert.AreEqual(InteractionType.HandRaycast, leftMode);
      Assert.AreEqual(InteractionType.Controller, rightMode);
      Assert.AreEqual(InteractionType.HandRaycast, _subsystem.CurrentInputModeLeft);
      Assert.AreEqual(InteractionType.Controller, _subsystem.CurrentInputModeRight);

      // Set both hands GazePinch
      mockProvider.SelectedPointerEvent = new PointerEvent {
        IsValid = true, ValidPointerCount = 1,
        Pointers = new Pointer[] { new Pointer { IsValid = true, IsActive = true,
                                                 Type = Pointer.PointerType.HandGazePinch,
                                                 Handedness = Handedness.Both } }
      };

      // Wait for the next frame to allow the InputSystem to process updates and trigger
      // onAfterUpdate.
      yield return null;

      Assert.AreEqual(InteractionType.GazePinch, leftMode);
      Assert.AreEqual(InteractionType.GazePinch, rightMode);
      Assert.AreEqual(InteractionType.GazePinch, _subsystem.CurrentInputModeLeft);
      Assert.AreEqual(InteractionType.GazePinch, _subsystem.CurrentInputModeRight);

      _subsystem.Stop();
    }
  }
}  // namespace Android.XR.InteractionFramework
