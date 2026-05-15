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

using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Android.XR.InteractionFramework {

  /// <summary>
  /// AxrifPluginComponent's XR fixed updates player loop system.
  /// This PlayerLoopSystem's purpose is to provide to the subsystem an update delegate for Early
  /// Update > XR Update phase. Ensuring input synchronization before any MonoBehaviour script
  /// update.
  /// </summary>
  public partial class AxrifSubsystem {
    private void AddPlayerLoopSystem(Type parentLoopSystemType = null,
                                     Type parentLoopSubsystemType = null,
                                     Type playerLoopSystemType = null,
                                     PlayerLoopSystem.UpdateFunction updateDelegate = null) {
      var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

      if (!TryFindPlayerLoopSystem(playerLoop, parentLoopSystemType, out var earlyIndex))
        return;

      if (!TryFindPlayerLoopSystem(playerLoop.subSystemList[earlyIndex], parentLoopSubsystemType,
                                   out var xrIndex))
        return;

      var loopSystem = playerLoop.subSystemList[earlyIndex].subSystemList[xrIndex];
      if (TryFindPlayerLoopSystem(loopSystem, playerLoopSystemType, out _))
        return;  // already exists

      loopSystem.subSystemList ??= new PlayerLoopSystem[] {};

      var list = loopSystem.subSystemList.ToList();
      list.Add(
          new PlayerLoopSystem { type = playerLoopSystemType, updateDelegate = updateDelegate });
      playerLoop.subSystemList[earlyIndex].subSystemList[xrIndex].subSystemList = list.ToArray();

      PlayerLoop.SetPlayerLoop(playerLoop);
    }

    private void RemovePlayerLoopSystem(Type parentLoopSystemType = null,
                                        Type parentLoopSubsystemType = null,
                                        Type playerLoopSystemType = null) {
      var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

      if (!TryFindPlayerLoopSystem(playerLoop, parentLoopSystemType, out var earlyIndex)) {
        return;
      }

      if (!TryFindPlayerLoopSystem(playerLoop.subSystemList[earlyIndex], parentLoopSubsystemType,
                                   out var xrIndex))
        return;

      var loopSystem = playerLoop.subSystemList[earlyIndex].subSystemList[xrIndex];
      if (!TryFindPlayerLoopSystem(loopSystem, playerLoopSystemType, out _))
        return;

      var list = loopSystem.subSystemList.ToList();
      list.RemoveAll(x => x.type == playerLoopSystemType);
      playerLoop.subSystemList[earlyIndex].subSystemList[xrIndex].subSystemList = list.ToArray();

      PlayerLoop.SetPlayerLoop(playerLoop);
    }

    private static bool TryFindPlayerLoopSystem(PlayerLoopSystem playerLoop, Type type,
                                                out int index) {
      if (playerLoop.subSystemList == null) {
        index = -1;
        return false;
      }

      for (var i = 0; i < playerLoop.subSystemList.Length; i++) {
        if (playerLoop.subSystemList[i].type != type)
          continue;
        index = i;
        return true;
      }

      index = -1;
      return false;
    }
  }
}  // namespace Android.XR.InteractionFramework
