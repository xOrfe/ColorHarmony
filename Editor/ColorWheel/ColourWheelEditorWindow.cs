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
        private const float WheelSize = 340f;
        private const float PointSize = 28f;

        [SerializeField] private UnityEngine.Object targetObject;
        [SerializeField] private string propertyPath;
        private SerializedObject serializedObject;
        private ColourWheel wheel;

        private VisualElement wheelSurface;
        private VisualElement pointLayer;
        private readonly List<WheelPointElement> pointElements = new List<WheelPointElement>();
        private readonly List<Button> swatches = new List<Button>();
        private IntegerField selectedIndexField;
        private Slider pointBrightnessSlider;
        private Texture2D wheelTexture;
        private WheelType? textureWheelType;
        private int selectedIndex;

        public static void Open(SerializedProperty property)
        {
            ColourWheelEditorWindow window = GetWindow<ColourWheelEditorWindow>();
            window.titleContent = new GUIContent("Color Wheel");
            window.minSize = new Vector2(560f, 680f);
            window.targetObject = property.serializedObject.targetObject;
            window.propertyPath = property.propertyPath;
            window.Rebind();
            window.BuildGui();
            window.Show();
        }

        private void OnEnable()
        {
            if (targetObject != null)
            {
                Rebind();
                BuildGui();
            }
        }

        private void OnDisable()
        {
            if (wheelTexture != null)
            {
                DestroyImmediate(wheelTexture);
                wheelTexture = null;
            }
        }

        private void Rebind()
        {
            if (targetObject == null || string.IsNullOrEmpty(propertyPath))
            {
                return;
            }

            serializedObject = new SerializedObject(targetObject);
            SerializedProperty property = serializedObject.FindProperty(propertyPath);
            wheel = property != null ? SerializedPropertyUtility.GetManagedValue<ColourWheel>(property) : null;
            wheel?.Refresh();
        }

        private void BuildGui()
        {
            rootVisualElement.Clear();
            if (wheel == null)
            {
                rootVisualElement.Add(new HelpBox("Color wheel target could not be resolved.", HelpBoxMessageType.Error));
                return;
            }

            rootVisualElement.style.paddingLeft = 12;
            rootVisualElement.style.paddingRight = 12;
            rootVisualElement.style.paddingTop = 12;
            rootVisualElement.style.paddingBottom = 12;

            TextField nameField = new TextField("Name") { value = wheel.WheelName };
            nameField.RegisterValueChangedCallback(evt => Mutate("Rename Color Wheel", () => wheel.WheelName = evt.newValue));
            rootVisualElement.Add(nameField);

            VisualElement toolbar = new VisualElement { style = { flexDirection = FlexDirection.Row, marginTop = 8, marginBottom = 8 } };
            EnumField harmonyField = new EnumField("Harmony", wheel.HarmonyType);
            harmonyField.RegisterValueChangedCallback(evt => Mutate("Change Harmony", () => wheel.SetHarmony((HarmonyType)evt.newValue)));
            EnumField wheelField = new EnumField("Wheel", wheel.WheelType);
            wheelField.RegisterValueChangedCallback(evt => Mutate("Change Wheel", () => wheel.SetWheel((WheelType)evt.newValue), true));
            Toggle lockedToggle = new Toggle("Lock Points") { value = wheel.IsPointsLocked };
            lockedToggle.RegisterValueChangedCallback(evt => Mutate("Toggle Point Lock", () => wheel.SetPointsLocked(evt.newValue)));

            toolbar.Add(harmonyField);
            toolbar.Add(wheelField);
            toolbar.Add(lockedToggle);
            rootVisualElement.Add(toolbar);

            Slider brightness = new Slider("Brightness", 0f, 1f) { value = wheel.Brightness, showInputField = true };
            brightness.RegisterValueChangedCallback(evt => Mutate("Change Wheel Brightness", () => wheel.SetBrightness(evt.newValue)));
            rootVisualElement.Add(brightness);

            BuildWheelSurface();
            BuildPointControls();
            RefreshView();
        }

        private void BuildWheelSurface()
        {
            wheelSurface = new VisualElement
            {
                style =
                {
                    width = WheelSize,
                    height = WheelSize,
                    alignSelf = Align.Center,
                    marginTop = 12,
                    marginBottom = 12,
                    borderBottomLeftRadius = WheelSize,
                    borderBottomRightRadius = WheelSize,
                    borderTopLeftRadius = WheelSize,
                    borderTopRightRadius = WheelSize,
                    overflow = Overflow.Hidden,
                    position = Position.Relative
                }
            };

            pointLayer = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    left = 0,
                    top = 0,
                    width = WheelSize,
                    height = WheelSize
                }
            };

            wheelSurface.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0)
                {
                    return;
                }

                MoveSelectedPoint(evt.localPosition);
                wheelSurface.CapturePointer(evt.pointerId);
            });
            wheelSurface.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (wheelSurface.HasPointerCapture(evt.pointerId))
                {
                    MoveSelectedPoint(evt.localPosition);
                }
            });
            wheelSurface.RegisterCallback<PointerUpEvent>(evt => wheelSurface.ReleasePointer(evt.pointerId));

            wheelSurface.Add(pointLayer);
            rootVisualElement.Add(wheelSurface);
        }

        private void BuildPointControls()
        {
            VisualElement buttons = new VisualElement { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.Center, marginBottom = 10 } };
            Button add = new Button(() => Mutate("Add Color Wheel Point", wheel.AddPoint)) { text = "Add Point" };
            Button remove = new Button(() => Mutate("Remove Color Wheel Point", wheel.RemovePoint)) { text = "Remove Point" };
            buttons.Add(add);
            buttons.Add(remove);
            rootVisualElement.Add(buttons);

            VisualElement swatchRow = new VisualElement { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.Center, marginBottom = 8 } };
            for (int i = 0; i < ColourWheel.MaxPointCount; i++)
            {
                int index = i;
                Button swatch = new Button(() =>
                {
                    selectedIndex = index;
                    RefreshView();
                });
                swatch.style.width = 42;
                swatch.style.height = 42;
                swatch.style.marginLeft = 2;
                swatch.style.marginRight = 2;
                swatches.Add(swatch);
                swatchRow.Add(swatch);
            }
            rootVisualElement.Add(swatchRow);

            selectedIndexField = new IntegerField("Selected") { value = selectedIndex };
            selectedIndexField.SetEnabled(false);
            rootVisualElement.Add(selectedIndexField);

            pointBrightnessSlider = new Slider("Point Brightness", 0f, 1f) { showInputField = true };
            pointBrightnessSlider.RegisterValueChangedCallback(evt =>
            {
                int index = Mathf.Clamp(selectedIndex, 0, wheel.PointCount - 1);
                Mutate("Change Point Brightness", () => wheel.Points[index].SetBrightness(evt.newValue, wheel.WheelType));
            });
            rootVisualElement.Add(pointBrightnessSlider);
        }

        private void MoveSelectedPoint(Vector2 localPosition)
        {
            int index = Mathf.Clamp(selectedIndex, 0, wheel.PointCount - 1);
            ColorWheelCoordinate coordinate = ColorWheelUtility.PositionToCoordinate(localPosition, new Vector2(WheelSize, WheelSize));
            Mutate("Move Color Wheel Point", () => wheel.MovePoint(index, coordinate));
        }

        private void Mutate(string undoName, Action action, bool rebuildTexture = false)
        {
            if (targetObject == null || wheel == null)
            {
                return;
            }

            Undo.RecordObject(targetObject, undoName);
            action.Invoke();
            EditorUtility.SetDirty(targetObject);
            serializedObject?.Update();

            if (rebuildTexture)
            {
                textureWheelType = null;
                RebuildTexture();
            }

            RefreshView();
        }

        private void RefreshView()
        {
            if (wheel == null || wheelSurface == null)
            {
                return;
            }

            RebuildTexture();
            pointLayer.Clear();
            pointElements.Clear();

            for (int i = 0; i < wheel.PointCount; i++)
            {
                WheelPointDefinition point = wheel.Points[i];
                WheelPointElement element = new WheelPointElement(i);
                element.AddToClassList("colour-wheel-point");
                element.style.position = Position.Absolute;
                element.style.width = PointSize;
                element.style.height = PointSize;
                element.style.borderBottomLeftRadius = PointSize;
                element.style.borderBottomRightRadius = PointSize;
                element.style.borderTopLeftRadius = PointSize;
                element.style.borderTopRightRadius = PointSize;
                element.style.backgroundColor = point.Color;
                element.style.borderBottomWidth = i == selectedIndex ? 3 : 1;
                element.style.borderLeftWidth = i == selectedIndex ? 3 : 1;
                element.style.borderRightWidth = i == selectedIndex ? 3 : 1;
                element.style.borderTopWidth = i == selectedIndex ? 3 : 1;
                element.style.borderBottomColor = Color.white;
                element.style.borderLeftColor = Color.white;
                element.style.borderRightColor = Color.white;
                element.style.borderTopColor = Color.white;

                Vector2 pos = ColorWheelUtility.CoordinateToPosition(point.Coordinate, new Vector2(WheelSize, WheelSize));
                element.style.left = pos.x - PointSize * 0.5f;
                element.style.top = pos.y - PointSize * 0.5f;
                pointLayer.Add(element);
                pointElements.Add(element);
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, wheel.PointCount - 1));
            selectedIndexField.value = selectedIndex;
            pointBrightnessSlider.SetValueWithoutNotify(wheel.Points[selectedIndex].Brightness);

            for (int i = 0; i < swatches.Count; i++)
            {
                bool visible = i < wheel.PointCount;
                swatches[i].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                if (visible)
                {
                    swatches[i].style.backgroundColor = wheel.Points[i].Color;
                    swatches[i].style.borderBottomWidth = i == selectedIndex ? 3 : 1;
                    swatches[i].style.borderLeftWidth = i == selectedIndex ? 3 : 1;
                    swatches[i].style.borderRightWidth = i == selectedIndex ? 3 : 1;
                    swatches[i].style.borderTopWidth = i == selectedIndex ? 3 : 1;
                }
            }
        }

        private void RebuildTexture()
        {
            if (wheelSurface == null || textureWheelType == wheel.WheelType)
            {
                return;
            }

            if (wheelTexture != null)
            {
                DestroyImmediate(wheelTexture);
            }

            wheelTexture = ColorWheelTextureGenerator.Generate(wheel.WheelType);
            textureWheelType = wheel.WheelType;
            wheelSurface.style.backgroundImage = new StyleBackground(wheelTexture);
        }
    }
}
