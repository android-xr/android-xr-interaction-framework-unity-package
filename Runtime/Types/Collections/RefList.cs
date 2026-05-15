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

using System;

namespace Android.XR.InteractionFramework {
  /// <summary>
  /// A list of items that can be accessed by reference.
  /// </summary>
  /// <typeparam name="T">The type of the items in the list.</typeparam>
  public struct RefList<T>
      where T : struct {
    /// <summary>
    /// The default initial size of the list.
    /// </summary>
    private const int _initialSize = 16;

    /// <summary>
    /// The data array for the list.
    /// </summary>
    private T[] _data;

    /// <summary>
    /// The number of items in the list.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Creates a new RefList with the specified initial size.
    /// </summary>
    /// <param name="initialSize">The initial size of the list.</param>
    public RefList(int initialSize = _initialSize) {
      _data = new T[initialSize];
      Count = 0;
    }

    /// <summary>
    /// Returns a reference to the item at the specified index.
    /// </summary>
    /// <param name="index">The index of the item to return.</param>
    /// <returns>A reference to the item at the specified index.</returns>
    public ref T this[int index] => ref _data[index];

    /// <summary>
    /// Adds an item to the end of the list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item) {
      // Ensure the data array is initialized
      if (_data == null) {
        _data = new T[_initialSize];
      }
      if (Count == _data.Length) {
        Array.Resize(ref _data, _data.Length * 2);
      }
      _data[Count++] = item;
    }

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    public void FastRemoveAt(int index) {
      _data[index] = _data[--Count];
      _data[Count] = default;
    }

    /// <summary>
    /// Clears the list of all items.
    /// </summary>
    public void Clear() {
      if (_data == null) {
        return;
      }
      Array.Clear(_data, 0, Count);
      Count = 0;
    }

    /// <summary>
    /// Returns a span of the list's data.
    /// </summary>
    /// <returns>A span of the list's data.</returns>
    public Span<T> AsSpan() => _data.AsSpan(0, Count);
  }
}  // namespace Android.XR.InteractionFramework
