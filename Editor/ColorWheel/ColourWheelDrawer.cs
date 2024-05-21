using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony
{
    [CustomPropertyDrawer(typeof(ColourWheel))]
    public class ColourWheelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColourWheel target = fieldInfo.GetValue(property.serializedObject.targetObject) as ColourWheel;
            GUIContent content = new GUIContent("Colour Wheel");
            if (GUILayout.Button(content))
            {
                ColourWheelEditorWindow.ExposeWindow(GetVisualElement(property,target));
            }
        }

        private VisualElement GetVisualElement(SerializedProperty property, ColourWheel _colourWheel)
        {
            var root = new VisualElement();
            var styleSheet = Resources.Load<StyleSheet>("Editor/ColorWheel/ColourWheel");
            root.styleSheets.Add(styleSheet);
            
            _colourWheel.Root = root;
            
            var visualTree = Resources.Load<VisualTreeAsset>("Editor/ColorWheel/ColourWheel");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);
            
            VisualElement wheelElement = root.Query("ColourWheel");
            
            for (int i = 0; i < _colourWheel.wheelPointDefinitions.Count; i++)
                AddPointToWheel(_colourWheel,property,i);
            
            root.Q("WheelPointDef0").style.display = DisplayStyle.Flex;
            
            Button lockButton = (Button)root.Query("LockButton");
            lockButton.clicked += _colourWheel.OnLockButtonClicked;
            
            _colourWheel.SetWheel(WheelType.Standard);
            
            wheelElement.RegisterCallback<PointerMoveEvent>(_colourWheel.OnPointerMove);
            wheelElement.RegisterCallback<PointerUpEvent>(_colourWheel.OnPointerUp);
            
            
            Button addPointButton = root.Q("AddPoint") as Button;
            if (addPointButton != null) addPointButton.clicked += _colourWheel.OnAddPointButtonClicked;
            Button removePointButton = root.Q("RemovePoint") as Button;
            if (removePointButton != null) removePointButton.clicked += _colourWheel.OnRemovePointButtonClicked;
            
            
            Slider brightness = root.Q("Brightness") as Slider;
            brightness.RegisterValueChangedCallback(_colourWheel.OnBrightnessChange);
            
            EnumField harmonyTypeElement = (EnumField)root.Query("HarmonyType");
            harmonyTypeElement.value = _colourWheel.harmonyType;
            harmonyTypeElement.RegisterValueChangedCallback(_colourWheel.SetHarmony);
            
            EnumField wheelTypeElement = (EnumField)root.Query("WheelType");
            wheelTypeElement.value = _colourWheel.wheelType;
            wheelTypeElement.RegisterValueChangedCallback(_colourWheel.SetWheel);

            return root;
        }
        
        private void AddPointToWheel(ColourWheel _colourWheel,SerializedProperty _serializedProperty,int i)
        {
            VisualElement root = _colourWheel.Root;
            
            VisualElement wheelPointContainerElement = root.Q("WheelPointContainer");
            
            var pd = _colourWheel.wheelPointDefinitions[i];
            
            pd.WheelPointElement = new WheelPointElement(i);
            pd.WheelPointElement.AddToClassList("colour-wheel-point");
            wheelPointContainerElement.Add(pd.WheelPointElement);
            pd.WheelPointElement.RegisterCallback<PointerDownEvent>(
                (evt) => {
                    if (evt.button != 0)
                    {
                        return;
                    }
                    _colourWheel.StartDragging(pd.WheelPointElement.Index);
                }
            );
            
            pd.ExposedColor = root.Q("ExposedColor" + i);
            
            ((Button)pd.ExposedColor).clicked += _colourWheel.UnExposePoints;
            ((Button)pd.ExposedColor).clicked += pd.ExposePointData;
            
            pd.ExposedData = root.Q("WheelPointDef" + i);
            
            SerializedProperty wheelPointProperty = _serializedProperty.FindPropertyRelative("wheelPointDefinitions").GetArrayElementAtIndex(i);
            PropertyField onUpdateProperty = new PropertyField(wheelPointProperty.FindPropertyRelative("onUpdate"));

            Slider b = new Slider();
            b.highValue = 1.0f;
            b.lowValue = 0f;
            pd.ExposedData.Add(b);
            b.value = _colourWheel.wheelPointDefinitions[i].brightness;
            b.RegisterValueChangedCallback(
                (evt) => {
                    _colourWheel.wheelPointDefinitions[i].brightness = evt.newValue;
                    _colourWheel.wheelPointDefinitions[i].OnBrightnessChange();
                }
            );
            pd.ExposedData.Add(new PropertyField(wheelPointProperty));
            pd.ExposedData.Add(onUpdateProperty);
            
            // pd.ExposedData.Add(new PropertyField(wheelPointProperty.FindPropertyRelative("wheelCoordinate")));
            // pd.ExposedData.Add(new PropertyField(wheelPointProperty.FindPropertyRelative("wheelCoordinateDiffWithCoordinateFactor")));
            
            pd.UpdateWheelCoordinate(_colourWheel.DynamicWheelCoordinateFactor);
            
            pd.ExposedData.style.display = DisplayStyle.None;
            
            pd.SetVisible(pd.Visible);
        }
    }
}