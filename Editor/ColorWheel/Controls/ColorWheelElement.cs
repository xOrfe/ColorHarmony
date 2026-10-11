using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    public sealed class ColorWheelElement : VisualElement, IDisposable
    {
        private ColourWheel model;
        private readonly Action<ColourWheel> changed;
        private readonly VisualElement canvas = new VisualElement();
        private readonly List<Label> points = new List<Label>();
        private readonly List<Button> swatches = new List<Button>();
        private readonly TextField nameField = new TextField("Name");
        private readonly EnumField harmonyField = new EnumField("Harmony", HarmonyType.Custom);
        private readonly EnumField wheelField = new EnumField("Wheel", WheelType.Standard);
        private readonly Toggle lockedField = new Toggle("Lock Harmony");
        private readonly Slider brightnessField = new Slider("Brightness", 0, 1) { showInputField = true };
        private readonly Slider pointBrightness = new Slider("Point Brightness", 0, 1) { showInputField = true };
        private readonly ColorField colorField = new ColorField("Selected Color");
        private readonly TextField hexField = new TextField("HEX") { isReadOnly = true };
        private readonly Label channels = new Label();
        private readonly Button addButton;
        private readonly Button removeButton;
        private int selected;
        private bool dragging;
        private int pointer;
        private ColourWheel dragStart;
        private Texture2D texture;
        private WheelType? textureType;
        private float textureBrightness = -1;
        private readonly Color[] wheelPixels = new Color[512 * 512];
        public event Action BeginGesture;
        public event Action EndGesture;

        public ColorWheelElement(ColourWheel value, Action<ColourWheel> changed)
        {
            this.changed = changed;
            ColorHarmonyStyle.Apply(this); focusable = true;
            RegisterCallback<GeometryChangedEvent>(_ =>
            {
                float size = Mathf.Clamp(contentRect.width - 32, 160, 360);
                canvas.style.width = size; canvas.style.height = size;
            });
            Add(nameField); Add(harmonyField); Add(wheelField); Add(lockedField); Add(brightnessField);
            canvas.AddToClassList("ch-canvas"); canvas.style.alignSelf = Align.Center;
            canvas.style.marginTop = 12; canvas.style.marginBottom = 16; canvas.style.marginRight = 0;
            canvas.generateVisualContent += context =>
            {
                if (model == null || model.PointCount < 2 || !model.IsPointsLocked) return;
                var painter = context.painter2D;
                painter.lineWidth = 1.5f; painter.strokeColor = new Color(1, 1, 1, 0.5f);
                painter.BeginPath();
                for (int i = 0; i < model.PointCount; i++)
                {
                    Vector2 position = ColorWheelUtility.CoordinateToPosition(model.Points[i].Coordinate, canvas.contentRect.size);
                    if (i == 0) painter.MoveTo(position); else painter.LineTo(position);
                }
                if (model.PointCount > 2) painter.ClosePath();
                painter.Stroke();
            };
            Add(canvas);
            var row = new VisualElement(); row.AddToClassList("ch-row"); row.style.justifyContent = Justify.Center;
            for (int i = 0; i < ColourWheel.MaxPointCount; i++)
            {
                int index = i;
                var point = new Label((i + 1).ToString()) { userData = index }; point.AddToClassList("ch-point"); canvas.Add(point); points.Add(point);
                var swatch = new Button(() => { selected = index; Refresh(); }) { tooltip = "Select Color " + (i + 1) }; swatch.AddToClassList("ch-swatch"); row.Add(swatch); swatches.Add(swatch);
            }
            Add(row);
            var fields = new VisualElement(); fields.AddToClassList("ch-section");
            fields.Add(colorField); fields.Add(hexField); fields.Add(channels); fields.Add(pointBrightness); Add(fields);
            addButton = new Button(() => Mutate(w => { w.AddPoint(); selected = w.PointCount - 1; })) { text = "Add Point" };
            removeButton = new Button(() => Mutate(w => w.RemovePoint())) { text = "Remove Point" };
            Add(ColorHarmonyStyle.Row(addButton, removeButton,
                new Button(() => EditorGUIUtility.systemCopyBuffer = string.Join("\n", HexPalette())) { text = "Copy Palette" }));
            nameField.RegisterValueChangedCallback(e => Mutate(w => w.WheelName = e.newValue));
            harmonyField.RegisterValueChangedCallback(e => Mutate(w => w.SetHarmony((HarmonyType)e.newValue)));
            wheelField.RegisterValueChangedCallback(e => Mutate(w => w.SetWheel((WheelType)e.newValue)));
            lockedField.RegisterValueChangedCallback(e => Mutate(w => w.SetPointsLocked(e.newValue)));
            brightnessField.RegisterValueChangedCallback(e => Mutate(w => w.SetBrightness(e.newValue)));
            pointBrightness.RegisterValueChangedCallback(e => Mutate(w => w.Points[selected].SetBrightness(e.newValue, w.WheelType)));
            colorField.RegisterValueChangedCallback(e => Mutate(w => SetSelectedColor(w, e.newValue)));
            canvas.RegisterCallback<GeometryChangedEvent>(_ => PositionPoints());
            canvas.RegisterCallback<PointerDownEvent>(e =>
            {
                if (e.button != 0) return;
                Focus();
                if (e.target is VisualElement hit && hit.userData is int index) selected = index;
                dragStart = Clone(model); dragging = true; pointer = e.pointerId; BeginGesture?.Invoke();
                canvas.CapturePointer(pointer); Move(e.position); e.StopPropagation();
            });
            canvas.RegisterCallback<PointerMoveEvent>(e => { if (dragging && canvas.HasPointerCapture(e.pointerId)) { Move(e.position); e.StopPropagation(); } });
            canvas.RegisterCallback<PointerUpEvent>(e => { if (canvas.HasPointerCapture(e.pointerId)) canvas.ReleasePointer(e.pointerId); FinishDrag(); });
            canvas.RegisterCallback<PointerCaptureOutEvent>(_ => FinishDrag());
            RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.keyCode != KeyCode.Escape || !dragging) return;
                model = dragStart; changed?.Invoke(Clone(model)); canvas.ReleasePointer(pointer); FinishDrag(); Refresh(); e.StopPropagation();
            });
            RegisterCallback<DetachFromPanelEvent>(_ => { FinishDrag(); Dispose(); });
            RegisterCallback<AttachToPanelEvent>(_ => Refresh());
            SetModel(value);
        }

        public void SetModel(ColourWheel value) { FinishDrag(); model = Clone(value ?? new ColourWheel()); model.Refresh(); Refresh(); }
        private static ColourWheel Clone(ColourWheel value) => WheelSerialization.CloneForEditing(value);
        public IReadOnlyList<Color> GetPalette()
        {
            var colors = new List<Color>(); for (int i = 0; i < model.PointCount; i++) colors.Add(model.Points[i].Color); return colors;
        }
        private IEnumerable<string> HexPalette() { foreach (Color color in GetPalette()) yield return "#" + ColorUtility.ToHtmlStringRGB(color); }
        private void Mutate(Action<ColourWheel> edit) { edit(model); changed?.Invoke(Clone(model)); Refresh(); }
        private void Move(Vector3 position)
        {
            Vector2 local = canvas.WorldToLocal(new Vector2(position.x, position.y));
            Mutate(w => w.MovePoint(selected, ColorWheelUtility.PositionToCoordinate(local, canvas.contentRect.size)));
        }
        private void SetSelectedColor(ColourWheel wheel, Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            ColorWheelCoordinate coordinate;
            if (wheel.WheelType == WheelType.Oklch)
            {
                Color lab = ColorX.RgbToOklab(ColorX.SrgbToLinear(color));
                coordinate = new ColorWheelCoordinate(Mathf.Atan2(lab.b, lab.g) / (2 * Mathf.PI), Mathf.Sqrt(lab.g * lab.g + lab.b * lab.b) / 0.32f);
                v = Mathf.InverseLerp(0.18f, 0.95f, lab.r);
            }
            else coordinate = new ColorWheelCoordinate(h, s);
            wheel.MovePoint(selected, coordinate);
            wheel.Points[selected].SetBrightness(v, wheel.WheelType);
        }
        private void FinishDrag() { if (!dragging) return; dragging = false; dragStart = null; EndGesture?.Invoke(); }

        private void Refresh()
        {
            if (model == null) return;
            selected = Mathf.Clamp(selected, 0, model.PointCount - 1);
            nameField.SetValueWithoutNotify(model.WheelName); harmonyField.SetValueWithoutNotify(model.HarmonyType);
            wheelField.SetValueWithoutNotify(model.WheelType); lockedField.SetValueWithoutNotify(model.IsPointsLocked);
            brightnessField.SetValueWithoutNotify(model.Brightness); pointBrightness.SetValueWithoutNotify(model.Points[selected].Brightness);
            colorField.SetValueWithoutNotify(model.Points[selected].Color);
            hexField.SetValueWithoutNotify("#" + ColorUtility.ToHtmlStringRGB(model.Points[selected].Color));
            Color color = model.Points[selected].Color; Color.RGBToHSV(color, out float h, out float s, out float v);
            channels.text = string.Format(CultureInfo.InvariantCulture, "RGB  {0:F3}, {1:F3}, {2:F3}\nHSV  {3:F1}°, {4:F3}, {5:F3}", color.r, color.g, color.b, h * 360, s, v);
            addButton.SetEnabled(model.PointCount < ColourWheel.MaxPointCount); removeButton.SetEnabled(model.PointCount > 1);
            for (int i = 0; i < points.Count; i++)
            {
                bool visible = i < model.PointCount;
                points[i].style.display = swatches[i].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
                if (!visible) continue;
                points[i].style.backgroundColor = swatches[i].style.backgroundColor = model.Points[i].Color;
                points[i].EnableInClassList("ch-selected", selected == i); swatches[i].EnableInClassList("ch-selected", selected == i);
            }
            RebuildTexture(); PositionPoints();
            canvas.MarkDirtyRepaint();
        }
        private void PositionPoints()
        {
            if (model == null) return;
            for (int i = 0; i < model.PointCount; i++)
            {
                Vector2 position = ColorWheelUtility.CoordinateToPosition(model.Points[i].Coordinate, canvas.contentRect.size);
                points[i].style.left = position.x - 12; points[i].style.top = position.y - 12;
            }
        }
        private void RebuildTexture()
        {
            if (texture && textureType == model.WheelType && Mathf.Approximately(textureBrightness, model.Brightness)) return;
            if (!texture) texture = new Texture2D(512, 512, TextureFormat.RGBA32, false)
            {
                name = "Color Wheel Preview", hideFlags = HideFlags.HideAndDontSave,
                wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear
            };
            ColorWheelTextureGenerator.Fill(texture, model.WheelType, model.Brightness, wheelPixels);
            textureType = model.WheelType; textureBrightness = model.Brightness; canvas.style.backgroundImage = texture;
        }
        public void Dispose() { canvas.style.backgroundImage = StyleKeyword.None; if (texture) UnityEngine.Object.DestroyImmediate(texture); texture = null; textureType = null; }
    }
}

