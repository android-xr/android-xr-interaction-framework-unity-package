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

using UnityEditor;
using UnityEngine;

namespace Android.XR.InteractionFramework.Editor {

  /// <summary>
  /// Provides custom editor visualization for the Configurations struct.
  /// </summary>
  [CustomPropertyDrawer(typeof(Configurations))]
  public class ConfigurationsDrawer : PropertyDrawer {
    /// <summary>
    /// The number of serialized properties in the Configurations struct. This must be updated as
    /// new fields are added to the structure.
    /// </summary>
    private const int PropertyCount = 7;

    /// <summary>
    /// The vertical padding to apply to each field in the editor.
    /// </summary>
    private const int FieldPadding = 2;

    /// <summary>
    /// Ensures the property labels are not truncated by checkboxes or text fields.
    /// </summary>
    private const int LabelWidth = 230;

    /// <summary>
    /// The label for the configurations structure in the editor.
    /// </summary>
    private const string ConfigurationsLabel = "Axrif Configuration";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      float originalLabelWidth = EditorGUIUtility.labelWidth;
      EditorGUIUtility.labelWidth = LabelWidth;
      label = EditorGUI.BeginProperty(position, label, property);

      SerializedProperty controllerEnabled = property.FindPropertyRelative("IsControllerEnabled");
      SerializedProperty gazePinchEnabled = property.FindPropertyRelative("IsGazePinchEnabled");
      SerializedProperty handRaycastEnabled = property.FindPropertyRelative("IsHandRaycastEnabled");
      SerializedProperty handPokeEnabled = property.FindPropertyRelative("IsHandPokeEnabled");
      SerializedProperty mouseEnabled = property.FindPropertyRelative("IsMouseEnabled");
      SerializedProperty useSensitiveTransitions =
          property.FindPropertyRelative("UseSensitiveTransitions");
      SerializedProperty useHeadForGazeInteraction =
          property.FindPropertyRelative("UseHeadForGazeInteraction");

      Rect fieldRect =
          new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
      property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, label, true);
      if (property.isExpanded) {
        EditorGUI.indentLevel++;
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.PropertyField(fieldRect, controllerEnabled);
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.PropertyField(fieldRect, gazePinchEnabled);
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(fieldRect, useSensitiveTransitions);
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.PropertyField(fieldRect, useHeadForGazeInteraction);
        EditorGUI.indentLevel--;
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.PropertyField(fieldRect, handRaycastEnabled);
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.PropertyField(fieldRect, handPokeEnabled);
        fieldRect.y += EditorGUIUtility.singleLineHeight + FieldPadding;
        EditorGUI.PropertyField(fieldRect, mouseEnabled);
        EditorGUI.indentLevel--;
      }

      EditorGUI.EndProperty();
      EditorGUIUtility.labelWidth = originalLabelWidth;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      // Total height must account for all fields drawn and an extra space for the label
      if (property.isExpanded) {
        return (EditorGUIUtility.singleLineHeight + FieldPadding) * (PropertyCount + 1);
      } else {
        return EditorGUIUtility.singleLineHeight + FieldPadding;
      }
    }
  }
}  // namespace Android.XR.InteractionFramework.Editor
