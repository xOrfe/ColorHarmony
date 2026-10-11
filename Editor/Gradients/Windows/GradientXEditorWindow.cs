using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace XO.ColorHarmony.Editor
{
    public sealed class GradientXEditorWindow : EditorWindow
    {
        [SerializeField] private UnityEngine.Object targetObject;
        [SerializeField] private string propertyPath;
        [SerializeField] private Material targetMaterial;
        [SerializeField] private string referenceName;
        [SerializeField] private GradientAsset source;
        [SerializeField] private GradientAsset working;
        [SerializeField] private int selectedLayer;
        [SerializeField] private bool legacyLoaded;
        private GradientPropertySession propertySession;
        private GradientEditorElement editor;
        private Label status;
        private Image output;
        private Texture2D outputTexture;
        private readonly Color[] outputPixels = new Color[512 * 32];
        private int gestureGroup = -1;
        private bool PropertyMode => !string.IsNullOrEmpty(propertyPath);

        [MenuItem("Window/Color Harmony/GradientX")]
        public static void OpenStandalone() => CreateWindow().Show();

        private static GradientXEditorWindow CreateWindow()
        {
            var window = CreateInstance<GradientXEditorWindow>();
            window.titleContent = new GUIContent("GradientX");
            window.minSize = new Vector2(440, 560);
            return window;
        }

        public static void ExposeWindow(UnityEngine.Object target, string path)
        {
            var window = CreateWindow();
            window.targetObject = target; window.propertyPath = path;
            window.propertySession = new GradientPropertySession(target, path);
            GradientX value = window.propertySession.Read();
            if (value == null) { DestroyImmediate(window); return; }
            window.working.Gradients[0] = value; window.Show();
        }

        public static void ExposeWindow(Material material, string label, string reference)
        {
            var window = CreateWindow();
            window.targetMaterial = material; window.referenceName = reference;
            window.source = material.HasProperty(reference) ? GradientAssetService.FindSource(material.GetTexture(reference)) : null;
            if (window.source) GradientAssetService.Copy(window.source, window.working);
            else
            {
                var legacy = GradientAssetService.ReadLegacy(material, reference, label);
                if (legacy != null && legacy.Count > 0) { window.working.Gradients.Clear(); window.working.Gradients.AddRange(legacy); window.legacyLoaded = true; }
            }
            window.Show();
        }

        public static void OpenAsset(GradientAsset asset)
        {
            var window = CreateWindow(); window.source = asset;
            GradientAssetService.Copy(asset, window.working); window.Show();
        }

        private void OnEnable()
        {
            if (!working) working = CreateInstance<GradientAsset>();
            working.hideFlags = HideFlags.HideAndDontSave;
            if (targetObject) propertySession = new GradientPropertySession(targetObject, propertyPath);
            Undo.undoRedoPerformed += OnUndo;
            saveChangesMessage = "Apply your GradientX changes before closing?";
        }

        public void CreateGUI()
        {
            editor?.Dispose(); rootVisualElement.Clear();
            ColorHarmonyStyle.Apply(rootVisualElement);
            rootVisualElement.Add(ColorHarmonyStyle.Heading("GradientX"));
            if (PropertyMode && !targetObject)
            {
                rootVisualElement.Add(new HelpBox("The gradient target no longer exists. Close this window and select another target.", HelpBoxMessageType.Warning));
                return;
            }
            var scroll = new ScrollView(); rootVisualElement.Add(scroll);
            string target = PropertyMode ? targetObject.name + " / " + propertyPath : targetMaterial ? targetMaterial.name + " / " + referenceName : source ? source.name : "New Gradient Asset";
            var label = new Label(target); label.AddToClassList("ch-muted"); scroll.Add(label);
            status = new Label(); status.AddToClassList("ch-muted"); scroll.Add(status);
            if (legacyLoaded) scroll.Add(new HelpBox("Legacy source keys were loaded. Corrected alpha, brightness and circular interpolation may change the new bake. The existing texture is preserved until Apply.", HelpBoxMessageType.Warning));
            if (!PropertyMode)
            {
                var preset = new ObjectField("Load Preset") { objectType = typeof(GradientAsset), allowSceneObjects = false };
                preset.RegisterValueChangedCallback(e =>
                {
                    if (!(e.newValue is GradientAsset asset)) return;
                    Undo.RecordObject(working, "Load Gradient Preset");
                    GradientAssetService.Copy(asset, working); selectedLayer = 0;
                    hasUnsavedChanges = true; CreateGUI();
                });
                scroll.Add(preset);
                var names = new List<string>();
                for (int i = 0; i < working.Gradients.Count; i++) names.Add("Layer " + (i + 1));
                selectedLayer = Mathf.Clamp(selectedLayer, 0, working.Gradients.Count - 1);
                var layers = new PopupField<string>("Layer", names, selectedLayer);
                layers.RegisterValueChangedCallback(e => { selectedLayer = names.IndexOf(e.newValue); CreateGUI(); });
                scroll.Add(ColorHarmonyStyle.Row(layers,
                    new Button(() => { Undo.RecordObject(working, "Add Gradient Layer"); working.Gradients.Add(new GradientX()); selectedLayer = working.Gradients.Count - 1; hasUnsavedChanges = true; CreateGUI(); }) { text = "Add Layer" },
                    new Button(() => { if (working.Gradients.Count <= 1) return; Undo.RecordObject(working, "Remove Gradient Layer"); working.Gradients.RemoveAt(selectedLayer); selectedLayer = 0; hasUnsavedChanges = true; CreateGUI(); }) { text = "Remove Layer" }));
                if (working.Gradients.Count > 1) scroll.Add(new HelpBox("Layers use equal-weight averaging (legacy composition). Output shows the combined result.", HelpBoxMessageType.Info));
            }
            editor = new GradientEditorElement(working.Gradients[selectedLayer], OnEdited);
            editor.BeginGesture += () => { Undo.IncrementCurrentGroup(); gestureGroup = Undo.GetCurrentGroup(); Undo.SetCurrentGroupName("Move Gradient Key"); };
            editor.EndGesture += () => { if (gestureGroup >= 0) Undo.CollapseUndoOperations(gestureGroup); gestureGroup = -1; };
            scroll.Add(editor);
            scroll.Add(new Button(() => ColourWheelEditorWindow.OpenPalette(colors =>
            {
                if (editor == null || editor.panel == null) return;
                var linear = new List<Color>(); foreach (Color color in colors) linear.Add(ColorX.SrgbToLinear(color));
                editor.SetPalette(linear);
            })) { text = "Choose Colors with Color Wheel" });
            if (!PropertyMode)
            {
                output = new Image { scaleMode = ScaleMode.StretchToFill }; output.AddToClassList("ch-preview");
                scroll.Add(new Label("OUTPUT")); scroll.Add(output);
                var resolution = new IntegerField("Resolution") { value = working.Resolution };
                resolution.RegisterValueChangedCallback(e => { Undo.RecordObject(working, "Change Gradient Resolution"); working.Resolution = e.newValue; resolution.SetValueWithoutNotify(working.Resolution); hasUnsavedChanges = true; RefreshOutput(); });
                var hdr = new Toggle("HDR Texture (Linear RGB)") { value = working.Hdr };
                hdr.RegisterValueChangedCallback(e => { Undo.RecordObject(working, "Change Gradient Format"); working.Hdr = e.newValue; hasUnsavedChanges = true; RefreshOutput(); });
                scroll.Add(resolution); scroll.Add(hdr);
                scroll.Add(ColorHarmonyStyle.Row(
                    new Button(() => Apply(false)) { text = "Apply / Bake" },
                    new Button(() => Apply(true)) { text = "Save As New Asset" }));
                scroll.Add(new HelpBox("Edits stay in this window until Apply. Existing material textures and legacy source data are preserved until you apply a replacement.", HelpBoxMessageType.Info));
            }
            RefreshOutput();
        }

        private void OnEdited(GradientX value)
        {
            if (PropertyMode) { propertySession?.Write(value); working.Gradients[0] = value; }
            else { Undo.RecordObject(working, "Edit GradientX"); working.Gradients[selectedLayer] = value; hasUnsavedChanges = true; }
            RefreshOutput();
        }

        private void RefreshOutput()
        {
            if (status != null) status.text = PropertyMode ? "Inspector changes are saved immediately. Undo / Redo is supported." : hasUnsavedChanges ? "Unsaved changes" : "Ready";
            if (output == null) return;
            if (!outputTexture) outputTexture = new Texture2D(512, 32, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave, name = "GradientX Output Preview", wrapMode = TextureWrapMode.Clamp };
            GradientPreviewUtility.Paint(outputTexture, working.Sample(512), outputPixels); output.image = outputTexture;
        }

        private bool Apply(bool saveAs)
        {
            if (PropertyMode) return true;
            if (targetMaterial && (!targetMaterial.HasProperty(referenceName) || !AssetDatabase.IsOpenForEdit(targetMaterial)))
            {
                status.text = "The target material is missing this property or is read-only."; return false;
            }
            Undo.IncrementCurrentGroup(); int group = Undo.GetCurrentGroup();
            try
            {
                GradientAsset destination = saveAs ? null : source;
                if (!destination || !GradientAssetService.CanWrite(destination))
                {
                    string path = EditorUtility.SaveFilePanelInProject("Save Gradient Asset", source ? source.name : "New Gradient", "asset", "Choose a source asset location under Assets.");
                    if (string.IsNullOrEmpty(path)) return false;
                    destination = CreateInstance<GradientAsset>();
                    GradientAssetService.Copy(working, destination);
                    AssetDatabase.CreateAsset(destination, AssetDatabase.GenerateUniqueAssetPath(path));
                    Undo.RegisterCreatedObjectUndo(destination, "Create Gradient Asset");
                }
                Undo.RecordObject(destination, "Apply GradientX");
                GradientAssetService.Copy(working, destination);
                GradientAssetService.Bake(destination);
                if (targetMaterial)
                {
                    Undo.RecordObject(targetMaterial, "Assign Gradient Texture");
                    targetMaterial.SetTexture(referenceName, destination.BakedTexture);
                    EditorUtility.SetDirty(targetMaterial);
                    MaterialEditor.ApplyMaterialPropertyDrawers(targetMaterial);
                    XO.ColorHarmony.Editor.PipelineMaterialAdapter.Validate(targetMaterial);
                    AssetDatabase.SaveAssetIfDirty(targetMaterial);
                }
                source = destination; legacyLoaded = false; hasUnsavedChanges = false; RefreshOutput(); return true;
            }
            catch (Exception exception) { Undo.RevertAllDownToGroup(group); status.text = exception.Message; Debug.LogException(exception); return false; }
            finally { Undo.CollapseUndoOperations(group); }
        }

        public override void SaveChanges() { if (Apply(false)) base.SaveChanges(); }
        public override void DiscardChanges() { hasUnsavedChanges = false; base.DiscardChanges(); }

        private void OnUndo()
        {
            if (PropertyMode)
            {
                GradientX value = propertySession?.Read();
                if (value != null) { working.Gradients[0] = value; editor?.SetModel(value); }
            }
            else { hasUnsavedChanges = true; CreateGUI(); }
            RefreshOutput();
        }

        private void OnInspectorUpdate()
        {
            if (!PropertyMode || gestureGroup >= 0) return;
            GradientX value = propertySession?.Read();
            if (value != null && !value.Equals(working.Gradients[0]))
            {
                working.Gradients[0] = value; editor?.SetModel(value); RefreshOutput();
            }
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndo; editor?.Dispose();
            if (outputTexture) DestroyImmediate(outputTexture);
            outputTexture = null;
        }

        private void OnDestroy() { if (working) DestroyImmediate(working); }
    }
}


