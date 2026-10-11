using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    [CustomPropertyDrawer(typeof(ColourWheel))]
    public sealed class ColourWheelDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new Label(property.displayName));
            var button = new Button(() => ColourWheelEditorWindow.Open(property)) { text = "Open Color Wheel" };
            button.SetEnabled(!property.serializedObject.isEditingMultipleObjects); root.Add(button);
            if (property.serializedObject.isEditingMultipleObjects) root.Add(new HelpBox("Select one object to edit its color wheel.", HelpBoxMessageType.Info));
            else
            {
                var foldout = new Foldout { text = "Palette Preview" };
                root.Add(foldout);
                void Refresh()
                {
                    foldout.Clear();
                    var model = WheelSerialization.Read(property);
                    if (model == null) return;
                    var row = new VisualElement(); row.style.flexDirection = FlexDirection.Row; row.style.flexWrap = Wrap.Wrap;
                    for (int i = 0; i < model.PointCount; i++)
                    {
                        var swatch = new VisualElement { tooltip = "#" + ColorUtility.ToHtmlStringRGB(model.Points[i].Color) };
                        swatch.style.width = 28; swatch.style.height = 24; swatch.style.marginRight = 4; swatch.style.backgroundColor = model.Points[i].Color; row.Add(swatch);
                    }
                    foldout.Add(row);
                }
                root.TrackPropertyValue(property, _ => Refresh());
                root.RegisterCallback<AttachToPanelEvent>(_ => Refresh());
            }
            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = EditorGUI.PrefixLabel(position, label);
            using (new EditorGUI.DisabledScope(property.serializedObject.isEditingMultipleObjects))
                if (GUI.Button(rect, "Open Color Wheel")) ColourWheelEditorWindow.Open(property);
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}


