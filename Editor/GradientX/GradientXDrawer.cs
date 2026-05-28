using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony
{
    [CustomPropertyDrawer(typeof(GradientX))]
    public class GradientXDrawer : PropertyDrawer
    {
        private const float PreviewHeight = 18f;
        private const float Padding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float y = headerRect.yMax + Padding;

                SerializedProperty colorSpace = property.FindPropertyRelative("interpolationColorSpace");
                SerializedProperty gradientType = property.FindPropertyRelative("gradientType");
                SerializedProperty colorKeys = property.FindPropertyRelative("colorKeys");
                SerializedProperty alphaKeys = property.FindPropertyRelative("alphaKeys");

                Rect previewRect = new Rect(
                    position.x + EditorGUI.indentLevel * 15f,
                    y,
                    position.width - EditorGUI.indentLevel * 15f,
                    PreviewHeight);
                DrawPreview(previewRect, property);
                y += PreviewHeight + Padding;

                Rect buttonRect = new Rect(
                    position.x + EditorGUI.indentLevel * 15f,
                    y,
                    130f,
                    EditorGUIUtility.singleLineHeight);
                if (GUI.Button(buttonRect, "Open GradientX"))
                {
                    GradientXEditorWindow.ExposeWindow(property.serializedObject.targetObject, property.propertyPath);
                }

                y += EditorGUIUtility.singleLineHeight + Padding;

                DrawProperty(position, colorSpace, ref y);
                DrawProperty(position, gradientType, ref y);

                DrawProperty(position, colorKeys, ref y, true);
                DrawProperty(position, alphaKeys, ref y, true);

                Rect resetRect = new Rect(position.x + EditorGUI.indentLevel * 15f, y, 90f, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(resetRect, "Reset"))
                {
                    Reset(property);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded)
            {
                return height;
            }

            height += Padding;
            height += PreviewHeight;
            height += Padding + EditorGUIUtility.singleLineHeight;
            height += Padding;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("interpolationColorSpace"));
            height += Padding;
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("gradientType"));
            height += Padding + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("colorKeys"), true);
            height += Padding + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("alphaKeys"), true);
            height += Padding + EditorGUIUtility.singleLineHeight;
            return height;
        }

        private static void DrawProperty(Rect position, SerializedProperty property, ref float y, bool includeChildren = false)
        {
            float height = EditorGUI.GetPropertyHeight(property, includeChildren);
            Rect rect = new Rect(position.x, y, position.width, height);
            EditorGUI.PropertyField(rect, property, includeChildren);
            y += height + Padding;
        }

        private static void DrawPreview(Rect rect, SerializedProperty property)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            int width = Mathf.Max(1, Mathf.RoundToInt(rect.width));
            for (int i = 0; i < width; i++)
            {
                float time = width <= 1 ? 0f : i / (width - 1f);
                Color color = EvaluatePreviewColor(property, time);
                EditorGUI.DrawRect(new Rect(rect.x + i, rect.y, 1f, rect.height), color);
            }
        }

        private static Color EvaluatePreviewColor(SerializedProperty property, float time)
        {
            SerializedProperty colorKeys = property.FindPropertyRelative("colorKeys");
            SerializedProperty alphaKeys = property.FindPropertyRelative("alphaKeys");
            if (colorKeys == null || colorKeys.arraySize == 0)
            {
                return Color.white;
            }

            Color color = EvaluateColorKeys(colorKeys, property.FindPropertyRelative("gradientType"), time);
            color.a = EvaluateAlphaKeys(alphaKeys, time);
            return color;
        }

        private static Color EvaluateColorKeys(SerializedProperty colorKeys, SerializedProperty gradientType, float time)
        {
            Color selected = colorKeys.GetArrayElementAtIndex(0).FindPropertyRelative("color").colorValue;
            for (int i = 0; i < colorKeys.arraySize; i++)
            {
                SerializedProperty key = colorKeys.GetArrayElementAtIndex(i);
                if (time >= key.FindPropertyRelative("time").floatValue)
                {
                    selected = key.FindPropertyRelative("color").colorValue;
                }
            }

            if ((GradientType)gradientType.enumValueIndex == GradientType.Step)
            {
                return selected;
            }

            for (int i = 0; i < colorKeys.arraySize - 1; i++)
            {
                SerializedProperty a = colorKeys.GetArrayElementAtIndex(i);
                SerializedProperty b = colorKeys.GetArrayElementAtIndex(i + 1);
                float aTime = a.FindPropertyRelative("time").floatValue;
                float bTime = b.FindPropertyRelative("time").floatValue;
                if (time < aTime || time > bTime)
                {
                    continue;
                }

                float t = Mathf.InverseLerp(aTime, bTime, time);
                return Color.LerpUnclamped(
                    a.FindPropertyRelative("color").colorValue,
                    b.FindPropertyRelative("color").colorValue,
                    t);
            }

            return selected;
        }

        private static float EvaluateAlphaKeys(SerializedProperty alphaKeys, float time)
        {
            if (alphaKeys == null || alphaKeys.arraySize == 0)
            {
                return 1f;
            }

            float selected = alphaKeys.GetArrayElementAtIndex(0).FindPropertyRelative("alpha").floatValue;
            for (int i = 0; i < alphaKeys.arraySize; i++)
            {
                SerializedProperty key = alphaKeys.GetArrayElementAtIndex(i);
                if (time >= key.FindPropertyRelative("time").floatValue)
                {
                    selected = key.FindPropertyRelative("alpha").floatValue;
                }
            }

            for (int i = 0; i < alphaKeys.arraySize - 1; i++)
            {
                SerializedProperty a = alphaKeys.GetArrayElementAtIndex(i);
                SerializedProperty b = alphaKeys.GetArrayElementAtIndex(i + 1);
                float aTime = a.FindPropertyRelative("time").floatValue;
                float bTime = b.FindPropertyRelative("time").floatValue;
                if (time < aTime || time > bTime)
                {
                    continue;
                }

                float t = Mathf.InverseLerp(aTime, bTime, time);
                return Mathf.LerpUnclamped(
                    a.FindPropertyRelative("alpha").floatValue,
                    b.FindPropertyRelative("alpha").floatValue,
                    t);
            }

            return selected;
        }

        private static void Reset(SerializedProperty property)
        {
            property.FindPropertyRelative("interpolationColorSpace").enumValueIndex = (int)ColorSpaceType.RGB;
            property.FindPropertyRelative("gradientType").enumValueIndex = (int)GradientType.Interpolate;

            SerializedProperty colorKeys = property.FindPropertyRelative("colorKeys");
            colorKeys.arraySize = 2;
            SetColorKey(colorKeys.GetArrayElementAtIndex(0), 0f, Color.black, 1f);
            SetColorKey(colorKeys.GetArrayElementAtIndex(1), 1f, Color.white, 1f);

            SerializedProperty alphaKeys = property.FindPropertyRelative("alphaKeys");
            alphaKeys.arraySize = 2;
            SetAlphaKey(alphaKeys.GetArrayElementAtIndex(0), 0f, 1f);
            SetAlphaKey(alphaKeys.GetArrayElementAtIndex(1), 1f, 1f);

            property.serializedObject.ApplyModifiedProperties();
        }

        private static void SetColorKey(SerializedProperty key, float time, Color color, float brightness)
        {
            key.FindPropertyRelative("time").floatValue = time;
            key.FindPropertyRelative("color").colorValue = color;
            key.FindPropertyRelative("brightness").floatValue = brightness;
        }

        private static void SetAlphaKey(SerializedProperty key, float time, float alpha)
        {
            key.FindPropertyRelative("time").floatValue = time;
            key.FindPropertyRelative("alpha").floatValue = alpha;
        }
    }
}
