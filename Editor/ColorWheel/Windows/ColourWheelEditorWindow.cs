using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    public sealed class ColourWheelEditorWindow : EditorWindow
    {
        [SerializeField] private UnityEngine.Object targetObject;
        [SerializeField] private string propertyPath;
        [SerializeField] private WheelEditingState state;
        private ColorWheelElement editor;
        private Action<IReadOnlyList<Color>> accept;
        private int undoGroup = -1;

        [MenuItem("Window/Color Harmony/Color Wheel")]
        public static void OpenStandalone() => Create().Show();

        private static ColourWheelEditorWindow Create()
        {
            var window = CreateInstance<ColourWheelEditorWindow>();
            window.titleContent = new GUIContent("Color Wheel");
            window.minSize = new Vector2(360, 620); return window;
        }

        public static void Open(SerializedProperty property)
        {
            if (property.serializedObject.isEditingMultipleObjects) return;
            var window = Create();
            window.targetObject = property.serializedObject.targetObject; window.propertyPath = property.propertyPath;
            var model = WheelSerialization.Read(property);
            if (model == null) { DestroyImmediate(window); return; }
            window.state.Wheel = model; window.Show();
        }

        public static void OpenPalette(Action<IReadOnlyList<Color>> callback)
        {
            var window = Create(); window.accept = callback; window.Show();
        }

        private void OnEnable()
        {
            if (!state) state = CreateInstance<WheelEditingState>();
            state.hideFlags = HideFlags.HideAndDontSave;
            Undo.undoRedoPerformed += OnUndo;
        }

        public void CreateGUI()
        {
            editor?.Dispose(); rootVisualElement.Clear(); ColorHarmonyStyle.Apply(rootVisualElement);
            rootVisualElement.Add(ColorHarmonyStyle.Heading("Color Wheel"));
            var scroll = new ScrollView(); rootVisualElement.Add(scroll);
            editor = new ColorWheelElement(state.Wheel, Write);
            editor.BeginGesture += () => { Undo.IncrementCurrentGroup(); undoGroup = Undo.GetCurrentGroup(); Undo.SetCurrentGroupName("Move Color Wheel Point"); };
            editor.EndGesture += () => { if (undoGroup >= 0) Undo.CollapseUndoOperations(undoGroup); undoGroup = -1; };
            scroll.Add(editor);
            if (accept != null) scroll.Add(new Button(() => { accept(editor.GetPalette()); Close(); }) { text = "Send Palette to GradientX" });
        }

        private void Write(ColourWheel wheel)
        {
            if (targetObject)
            {
                using (var serialized = new SerializedObject(targetObject))
                {
                    SerializedProperty property = serialized.FindProperty(propertyPath);
                    if (property == null) return;
                    WheelSerialization.Write(property, wheel); serialized.ApplyModifiedProperties();
                }
            }
            else Undo.RecordObject(state, "Edit Color Wheel");
            state.Wheel = wheel;
        }

        private void OnUndo()
        {
            if (targetObject)
            {
                using (var serialized = new SerializedObject(targetObject))
                {
                    SerializedProperty property = serialized.FindProperty(propertyPath);
                    if (property == null) return;
                    state.Wheel = WheelSerialization.Read(property);
                }
            }
            editor?.SetModel(state.Wheel);
        }

        private void OnDisable() { Undo.undoRedoPerformed -= OnUndo; editor?.Dispose(); }
        private void OnDestroy() { if (state) DestroyImmediate(state); }
    }
}


