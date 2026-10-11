using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    [CustomPropertyDrawer(typeof(WheelPointDefinition))]
    public sealed class WheelPointPropertyDrawer : PropertyDrawer
    {
        private static readonly string[] Fields = { "coordinate", "brightness", "visible", "onUpdate" };
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var foldout = new Foldout { text = property.displayName };
            foreach (string field in Fields) foldout.Add(new PropertyField(property.FindPropertyRelative(field)));
            return foldout;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                foreach (string field in Fields)
                {
                    SerializedProperty child = property.FindPropertyRelative(field);
                    rect.y += rect.height + 2; rect.height = EditorGUI.GetPropertyHeight(child, true);
                    EditorGUI.PropertyField(rect, child, true);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (property.isExpanded) foreach (string field in Fields) height += 2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(field), true);
            return height;
        }
    }
}


