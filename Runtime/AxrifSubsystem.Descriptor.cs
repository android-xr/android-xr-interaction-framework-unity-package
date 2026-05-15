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
using UnityEngine.SubsystemsImplementation;

namespace Android.XR.InteractionFramework {

  public partial class AxrifSubsystem {
    public class Descriptor
        : SubsystemDescriptorWithProvider<AxrifSubsystem, AxrifSubsystem.Provider> {
      public struct Cinfo : IEquatable<Cinfo> {
        public string id { get; set; }
        public Type providerType { get; set; }
        public Type subsystemTypeOverride { get; set; }

        public bool Equals(Cinfo other) {
          return ReferenceEquals(id, other.id) &&
                 ReferenceEquals(providerType, other.providerType) &&
                 ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride);
        }

        public override bool Equals(object obj) {
          return obj is Cinfo other && Equals(other);
        }

        public override int GetHashCode() {
          unchecked {
            var hashCode = id != null ? id.GetHashCode() : 0;
            hashCode = hashCode * 4999591 + (providerType != null ? providerType.GetHashCode() : 0);
            hashCode = hashCode * 4999591 +
                       (subsystemTypeOverride != null ? subsystemTypeOverride.GetHashCode() : 0);
            return hashCode;
          }
        }

        public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);
        public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);
      }

      public static void Create(Cinfo cinfo) {
        SubsystemDescriptorStore.RegisterDescriptor(new Descriptor(cinfo));
      }

      private Descriptor(Cinfo cinfo) {
        id = cinfo.id;
        providerType = cinfo.providerType;
        subsystemTypeOverride = cinfo.subsystemTypeOverride;
      }
    }
  }
}  // namespace Android.XR.InteractionFramework