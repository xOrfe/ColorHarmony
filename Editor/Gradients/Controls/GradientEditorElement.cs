using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    /// <summary>A reusable gradient editor. The host owns persistence; this element owns interaction and preview resources.</summary>
    public sealed class GradientEditorElement : VisualElement, IDisposable
    {
        private GradientX model;
        private readonly Action<GradientX> changed;
        private readonly VisualElement colorTrack = new VisualElement();
        private readonly VisualElement alphaTrack = new VisualElement();
        private readonly Image preview = new Image();
        private readonly ColorField colorField = new ColorField("Color") { hdr = true };
        private readonly FloatField timeField = new FloatField("Position");
        private readonly FloatField brightnessField = new FloatField("Brightness");
        private readonly Slider alphaField = new Slider("Alpha", 0, 1) { showInputField = true };
        private readonly EnumField spaceField = new EnumField("Color Space", ColorSpaceType.RGB);
        private readonly EnumField modeField = new EnumField("Mode", GradientType.Interpolate);
        private readonly Toggle hueField = new Toggle("Shortest Hue Path");
        private readonly Label selectionLabel = new Label();
        private Texture2D texture;
        private int selected;
        private bool alphaSelected;
        private bool dragging;
        private int pointerId;
        private GradientX dragStart;
        private readonly Color[] previewPixels = new Color[512 * 32];
        private readonly Color[] previewSamples = new Color[512];
        public event Action BeginGesture;
        public event Action EndGesture;

        public GradientEditorElement(GradientX model, Action<GradientX> changed)
        {
            this.changed = changed;
            focusable = true;
            ColorHarmonyStyle.Apply(this);
            var section = new VisualElement(); section.AddToClassList("ch-section");
            alphaTrack.AddToClassList("ch-track"); colorTrack.AddToClassList("ch-track");
            preview.AddToClassList("ch-preview"); preview.scaleMode = ScaleMode.StretchToFill;
            section.Add(new Label("ALPHA")); section.Add(alphaTrack); section.Add(preview);
            section.Add(colorTrack); section.Add(new Label("COLOR   •   Double-click a track to add a key"));
            Add(section);
            Add(ColorHarmonyStyle.Row(
                new Button(() => AddKey(false, 0.5f)) { text = "Add Color Key" },
                new Button(() => AddKey(true, 0.5f)) { text = "Add Alpha Key" },
                new Button(DeleteSelected) { text = "Delete Key" },
                new Button(DuplicateSelected) { text = "Duplicate Key" }));
            var fields = new VisualElement(); fields.AddToClassList("ch-section");
            fields.Add(selectionLabel); fields.Add(timeField); fields.Add(colorField); fields.Add(brightnessField); fields.Add(alphaField);
            Add(fields);
            Add(spaceField); Add(modeField); Add(hueField);
            Add(ColorHarmonyStyle.Row(
                new Button(() => EditorGUIUtility.systemCopyBuffer = "ColorHarmonyGradient:" + JsonUtility.ToJson(this.model)) { text = "Copy" },
                new Button(Paste) { text = "Paste" },
                new Button(() => { this.model.Reset(); selected = 0; alphaSelected = false; Commit(); }) { text = "Reset" }));
            timeField.RegisterValueChangedCallback(e => EditSelected(GradientSnapshot.NormalizeTime(e.newValue), null, null, null));
            colorField.RegisterValueChangedCallback(e => EditSelected(null, e.newValue, null, null));
            brightnessField.RegisterValueChangedCallback(e => EditSelected(null, null, Mathf.Max(0, e.newValue), null));
            alphaField.RegisterValueChangedCallback(e => EditSelected(null, null, null, e.newValue));
            spaceField.RegisterValueChangedCallback(e => { this.model.InterpolationColorSpace = (ColorSpaceType)e.newValue; Commit(); });
            modeField.RegisterValueChangedCallback(e => { this.model.GradientType = (GradientType)e.newValue; Commit(); });
            hueField.RegisterValueChangedCallback(e => { this.model.ShortestHue = e.newValue; Commit(); });
            BindTrack(colorTrack, false); BindTrack(alphaTrack, true);
            RegisterCallback<KeyDownEvent>(OnKey);
            RegisterCallback<DetachFromPanelEvent>(_ => { FinishDrag(); Dispose(); });
            RegisterCallback<AttachToPanelEvent>(_ => Refresh());
            SetModel(model);
        }

        public void SetModel(GradientX value)
        {
            FinishDrag();
            model = value?.Clone() ?? new GradientX();
            ClampSelection();
            Refresh();
        }

        private void BindTrack(VisualElement track, bool alpha)
        {
            track.RegisterCallback<PointerDownEvent>(e =>
            {
                if (e.button != 0) return;
                Focus();
                if (e.target == track && e.clickCount == 2)
                {
                    AddKey(alpha, PointerTime(track, e.position)); e.StopPropagation(); return;
                }
                if (!(e.target is VisualElement handle) || !(handle.userData is int index)) return;
                alphaSelected = alpha; selected = index;
                dragStart = model.Clone(); dragging = true; pointerId = e.pointerId;
                BeginGesture?.Invoke();
                track.CapturePointer(pointerId);
                RefreshFields(); RefreshSelection(); e.StopPropagation();
            });
            track.RegisterCallback<PointerMoveEvent>(e =>
            {
                if (!dragging || !track.HasPointerCapture(e.pointerId)) return;
                EditSelected(PointerTime(track, e.position), null, null, null);
                e.StopPropagation();
            });
            track.RegisterCallback<PointerUpEvent>(e => { if (track.HasPointerCapture(e.pointerId)) { track.ReleasePointer(e.pointerId); FinishDrag(); } });
            track.RegisterCallback<PointerCaptureOutEvent>(_ => FinishDrag());
            track.RegisterCallback<GeometryChangedEvent>(_ => PositionHandles());
        }

        private static float PointerTime(VisualElement track, Vector3 position)
            => Mathf.Clamp01(track.WorldToLocal(new Vector2(position.x, position.y)).x / Mathf.Max(1, track.contentRect.width));

        private void EditSelected(float? time, Color? color, float? brightness, float? alpha)
        {
            if (alphaSelected)
            {
                if (selected >= model.AlphaKeys.Length) return;
                AlphaKey key = model.AlphaKeys[selected];
                if (time.HasValue) key.Time(time.Value);
                if (alpha.HasValue) key.Alpha(alpha.Value);
                model.AlphaKeys[selected] = key;
            }
            else
            {
                if (selected >= model.ColorKeys.Length) return;
                ColorKey key = model.ColorKeys[selected];
                if (time.HasValue) key.Time(time.Value);
                if (color.HasValue) key.Color(color.Value);
                if (brightness.HasValue) key.Brightness(brightness.Value);
                model.ColorKeys[selected] = key;
            }
            Commit(false);
        }

        private void AddKey(bool alpha, float time)
        {
            Color sample = model.Evaluate(time);
            alphaSelected = alpha;
            if (alpha)
            {
                var keys = new List<AlphaKey>(model.AlphaKeys) { new AlphaKey(time, sample.a) };
                model.AlphaKeys = keys.ToArray(); selected = keys.Count - 1;
            }
            else
            {
                var keys = new List<ColorKey>(model.ColorKeys) { new ColorKey(time, sample) };
                model.ColorKeys = keys.ToArray(); selected = keys.Count - 1;
            }
            Commit();
        }

        private void DeleteSelected()
        {
            if (alphaSelected)
            {
                if (model.AlphaKeys.Length <= 1) return;
                var keys = new List<AlphaKey>(model.AlphaKeys); keys.RemoveAt(selected); model.AlphaKeys = keys.ToArray();
            }
            else
            {
                if (model.ColorKeys.Length <= 1) return;
                var keys = new List<ColorKey>(model.ColorKeys); keys.RemoveAt(selected); model.ColorKeys = keys.ToArray();
            }
            ClampSelection(); Commit();
        }

        private void DuplicateSelected()
        {
            if (alphaSelected && model.AlphaKeys.Length > 0)
            {
                AlphaKey key = model.AlphaKeys[selected]; key.Time(Mathf.Clamp01(key.Time() + 0.025f));
                var keys = new List<AlphaKey>(model.AlphaKeys) { key }; model.AlphaKeys = keys.ToArray(); selected = keys.Count - 1;
            }
            else if (!alphaSelected && model.ColorKeys.Length > 0)
            {
                ColorKey key = model.ColorKeys[selected]; key.Time(Mathf.Clamp01(key.Time() + 0.025f));
                var keys = new List<ColorKey>(model.ColorKeys) { key }; model.ColorKeys = keys.ToArray(); selected = keys.Count - 1;
            }
            Commit();
        }

        public void SetPalette(IReadOnlyList<Color> colors)
        {
            if (colors == null || colors.Count == 0) return;
            var keys = new ColorKey[colors.Count];
            for (int i = 0; i < keys.Length; i++) keys[i] = new ColorKey(keys.Length == 1 ? 0 : i / (keys.Length - 1f), colors[i]);
            model.ColorKeys = keys; selected = 0; alphaSelected = false; Commit();
        }

        private void Paste()
        {
            const string prefix = "ColorHarmonyGradient:";
            string text = EditorGUIUtility.systemCopyBuffer;
            if (!text.StartsWith(prefix, StringComparison.Ordinal)) return;
            try
            {
                GradientX value = JsonUtility.FromJson<GradientX>(text.Substring(prefix.Length));
                if (value == null || value.ColorKeys.Length > 4096 || value.AlphaKeys.Length > 4096) return;
                model = value; ClampSelection(); Commit();
            }
            catch (ArgumentException) { Debug.LogWarning("Clipboard does not contain valid GradientX data."); }
        }

        private void OnKey(KeyDownEvent e)
        {
            // Leave text/numeric inputs to their own keyboard handlers.
            if (e.target is TextElement) return;
            if (e.keyCode == KeyCode.Escape && dragging)
            {
                model = dragStart; changed?.Invoke(model.Clone());
                colorTrack.ReleasePointer(pointerId); alphaTrack.ReleasePointer(pointerId);
                FinishDrag(); Refresh(); e.StopPropagation();
            }
            else if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace) { DeleteSelected(); e.StopPropagation(); }
        }

        private void FinishDrag()
        {
            if (!dragging) return;
            dragging = false; dragStart = null; EndGesture?.Invoke();
        }

        private void Commit(bool rebuild = true)
        {
            changed?.Invoke(model.Clone());
            if (rebuild) Refresh(); else { RefreshPreview(); RefreshFields(); PositionHandles(); }
        }

        private void ClampSelection() => selected = Mathf.Clamp(selected, 0, Mathf.Max(0, (alphaSelected ? model.AlphaKeys.Length : model.ColorKeys.Length) - 1));

        private void Refresh()
        {
            if (model == null) return;
            BuildHandles(colorTrack, false); BuildHandles(alphaTrack, true);
            RefreshFields(); RefreshPreview(); PositionHandles();
        }

        private void BuildHandles(VisualElement track, bool alpha)
        {
            track.Clear();
            int count = alpha ? model.AlphaKeys.Length : model.ColorKeys.Length;
            for (int i = 0; i < count; i++)
            {
                var handle = new VisualElement { userData = i, tooltip = (alpha ? "Alpha" : "Color") + " Key " + (i + 1) };
                handle.AddToClassList("ch-key");
                Color color = alpha ? Color.Lerp(Color.black, Color.white, model.AlphaKeys[i].Alpha()) : ColorX.LinearToSrgb(model.ColorKeys[i].Color());
                handle.style.backgroundColor = color;
                handle.EnableInClassList("ch-selected", alpha == alphaSelected && i == selected);
                track.Add(handle);
            }
        }

        private void RefreshSelection()
        {
            for (int i = 0; i < colorTrack.childCount; i++) colorTrack[i].EnableInClassList("ch-selected", !alphaSelected && selected == i);
            for (int i = 0; i < alphaTrack.childCount; i++) alphaTrack[i].EnableInClassList("ch-selected", alphaSelected && selected == i);
        }

        private void PositionHandles()
        {
            if (model == null) return;
            for (int i = 0; i < colorTrack.childCount; i++) colorTrack[i].style.left = GradientSnapshot.NormalizeTime(model.ColorKeys[i].Time()) * colorTrack.contentRect.width - 7;
            for (int i = 0; i < alphaTrack.childCount; i++) alphaTrack[i].style.left = GradientSnapshot.NormalizeTime(model.AlphaKeys[i].Time()) * alphaTrack.contentRect.width - 7;
        }

        private void RefreshFields()
        {
            bool valid = selected < (alphaSelected ? model.AlphaKeys.Length : model.ColorKeys.Length);
            selectionLabel.text = valid ? (alphaSelected ? "Alpha Key " : "Color Key ") + (selected + 1) : "No Key Selected";
            timeField.SetEnabled(valid); colorField.SetEnabled(valid); brightnessField.SetEnabled(valid); alphaField.SetEnabled(valid);
            colorField.style.display = brightnessField.style.display = alphaSelected ? DisplayStyle.None : DisplayStyle.Flex;
            alphaField.style.display = alphaSelected ? DisplayStyle.Flex : DisplayStyle.None;
            if (valid)
            {
                timeField.SetValueWithoutNotify(alphaSelected ? model.AlphaKeys[selected].Time() : model.ColorKeys[selected].Time());
                if (alphaSelected) alphaField.SetValueWithoutNotify(model.AlphaKeys[selected].Alpha());
                else { colorField.SetValueWithoutNotify(model.ColorKeys[selected].Color()); brightnessField.SetValueWithoutNotify(model.ColorKeys[selected].Brightness()); }
            }
            spaceField.SetValueWithoutNotify(model.InterpolationColorSpace);
            modeField.SetValueWithoutNotify(model.GradientType);
            hueField.SetValueWithoutNotify(model.ShortestHue);
            hueField.SetEnabled(model.InterpolationColorSpace == ColorSpaceType.HSV || model.InterpolationColorSpace == ColorSpaceType.HSL || model.InterpolationColorSpace == ColorSpaceType.HCV || model.InterpolationColorSpace == ColorSpaceType.HCY);
        }

        private void RefreshPreview()
        {
            const int width = 512, height = 32;
            if (texture == null) texture = new Texture2D(width, height, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave, wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear, name = "GradientX Preview" };
            var pixels = previewPixels;
            var snapshot = model.CreateSnapshot();
            var samples = previewSamples; snapshot.Sample(samples);
            GradientPreviewUtility.Paint(texture, samples, pixels);
            preview.image = texture;
        }

        public void Dispose()
        {
            preview.image = null;
            if (texture != null) UnityEngine.Object.DestroyImmediate(texture);
            texture = null;
        }
    }
}

