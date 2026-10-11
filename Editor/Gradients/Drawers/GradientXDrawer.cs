using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    [CustomPropertyDrawer(typeof(GradientX))]
    public sealed class GradientXDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var preview = new Image { scaleMode = ScaleMode.StretchToFill };
            preview.style.height = 24; preview.style.marginBottom = 4;
            Texture2D texture = null;
            var pixels = new Color[128 * 16];
            var samples = new Color[128];
            void Refresh()
            {
                if (property.serializedObject.targetObject == null) return;
                if (!texture) texture = new Texture2D(128, 16, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave, wrapMode = TextureWrapMode.Clamp };
                GradientSerialization.Read(property).CreateSnapshot().Sample(samples);
                GradientPreviewUtility.Paint(texture, samples, pixels); preview.image = texture;
            }
            root.Add(new Label(property.displayName)); root.Add(preview);
            var edit = new Button(() => GradientXEditorWindow.ExposeWindow(property.serializedObject.targetObject, property.propertyPath)) { text = "Edit GradientX" };
            edit.SetEnabled(!property.serializedObject.isEditingMultipleObjects);
            if (property.serializedObject.isEditingMultipleObjects) root.Add(new HelpBox("Select one object to edit its gradient. Mixed gradient editing is unavailable.", HelpBoxMessageType.Info));
            root.Add(edit);
            root.TrackPropertyValue(property, _ => Refresh());
            root.RegisterCallback<AttachToPanelEvent>(_ => Refresh());
            root.RegisterCallback<DetachFromPanelEvent>(_ => { preview.image = null; if (texture) Object.DestroyImmediate(texture); texture = null; });
            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect field = EditorGUI.PrefixLabel(position, label);
            using (new EditorGUI.DisabledScope(property.serializedObject.isEditingMultipleObjects))
                if (GUI.Button(field, "Edit GradientX")) GradientXEditorWindow.ExposeWindow(property.serializedObject.targetObject, property.propertyPath);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }

}


