using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony
{
    [CustomPropertyDrawer(typeof(ColourWheel))]
    public class ColourWheelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);
            if (GUI.Button(buttonRect, "Open Color Wheel"))
            {
                ColourWheelEditorWindow.Open(property);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
