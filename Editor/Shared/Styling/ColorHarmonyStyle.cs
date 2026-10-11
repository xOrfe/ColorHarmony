using UnityEditor;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    internal static class ColorHarmonyStyle
    {
        public static void Apply(VisualElement root)
        {
            var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.xorfe.colorharmony/Editor/Shared/Styles/ColorHarmony.uss");
            if (sheet == null)
            {
                // Embedded packages can have a folder name that differs from their manifest name.
                string[] guids = AssetDatabase.FindAssets("ColorHarmony t:StyleSheet");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.EndsWith("/Shared/Styles/ColorHarmony.uss")) { sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path); break; }
                }
            }
            if (sheet != null && !root.styleSheets.Contains(sheet)) root.styleSheets.Add(sheet);
            root.AddToClassList("ch-root");
            root.EnableInClassList("ch-light", !EditorGUIUtility.isProSkin);
        }
        public static VisualElement Row(params VisualElement[] elements)
        {
            var row = new VisualElement(); row.AddToClassList("ch-row");
            foreach (var element in elements) row.Add(element);
            return row;
        }
        public static Label Heading(string text)
        {
            var label = new Label(text); label.AddToClassList("ch-heading"); return label;
        }
    }
}

