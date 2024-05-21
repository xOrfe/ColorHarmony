using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    public class ColourWheelEditorWindow : EditorWindow
    {
        private static VisualElement _visualElement;
        public static void ExposeWindow(VisualElement visualElement)
        {
            _visualElement = visualElement;
            ColourWheelEditorWindow wnd = GetWindow<ColourWheelEditorWindow>();
            wnd.titleContent = new GUIContent("Colour Wheel");
            wnd.maxSize = new Vector2(720f, 1440f);
            wnd.minSize = new Vector2(600f, 750f);
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            root.Add(_visualElement);
        }
    }
}