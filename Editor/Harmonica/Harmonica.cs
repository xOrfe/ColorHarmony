using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace XO.ColorHarmony.Editor
{
    public class Harmonica : ShaderGUI
    {
        private readonly Dictionary<string, bool> expanded = new Dictionary<string, bool>();
        private ShaderPropertyDescriptor[] descriptors;
        private string signature;
        private Shader shader;
        private ShaderGUI pipeline;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Material material = materialEditor.target as Material;
            if (!material) return;
            var key = new StringBuilder();
            foreach (MaterialProperty property in properties) key.Append(property.name).Append('|').Append(property.displayName).Append('|');
            string next = key.ToString();
            if (shader != material.shader || signature != next)
            {
                shader = material.shader; signature = next;
                descriptors = new ShaderPropertyDescriptor[properties.Length];
                for (int i = 0; i < properties.Length; i++) descriptors[i] = new ShaderPropertyDescriptor(properties[i].displayName);
                pipeline = PipelineMaterialAdapter.Create(material);
            }
            EditorGUILayout.LabelField("Color Harmony", EditorStyles.boldLabel);
            var visibility = new List<bool>();
            var enabled = new List<bool>();
            var pipelineProperties = new List<MaterialProperty>();
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < properties.Length; i++)
            {
                MaterialProperty property = properties[i];
                if ((PropertyFlags(property) & ShaderPropertyFlags.HideInInspector) != 0) { pipelineProperties.Add(property); continue; }
                ShaderPropertyDescriptor descriptor = descriptors[i];
                int depth = descriptor.Depth;
                string diagnostic = descriptor.Diagnostic;
                if (depth > visibility.Count) { depth = 0; diagnostic = "This sub-property has no matching parent foldout."; }
                while (visibility.Count > depth) { visibility.RemoveAt(visibility.Count - 1); enabled.RemoveAt(enabled.Count - 1); }
                bool visible = true, canEdit = true;
                foreach (bool value in visibility) visible &= value;
                foreach (bool value in enabled) canEdit &= value;
                if (!visible)
                {
                    if (descriptor.IsToggleFoldout) { visibility.Add(false); enabled.Add(false); }
                    continue;
                }
                int indent = EditorGUI.indentLevel; EditorGUI.indentLevel = indent + depth;
                using (new EditorGUI.DisabledScope(!canEdit))
                {
                    if (diagnostic != null) EditorGUILayout.HelpBox(diagnostic, MessageType.Warning);
                    if (descriptor.IsToggleFoldout && diagnostic == null && (PropertyType(property) == ShaderPropertyType.Float || PropertyType(property) == ShaderPropertyType.Range))
                    {
                        if (!expanded.TryGetValue(property.name, out bool open)) open = true;
                        Rect rect = EditorGUILayout.GetControlRect();
                        Rect arrow = new Rect(rect.x, rect.y, 18, rect.height);
                        open = EditorGUI.Foldout(arrow, open, GUIContent.none);
                        expanded[property.name] = open;
                        EditorGUI.showMixedValue = property.hasMixedValue;
                        EditorGUI.BeginChangeCheck();
                        bool active = EditorGUI.ToggleLeft(new Rect(rect.x + 20, rect.y, rect.width - 20, rect.height), descriptor.Label, property.floatValue > 0.5f);
                        if (EditorGUI.EndChangeCheck()) { materialEditor.RegisterPropertyChangeUndo(descriptor.Label); property.floatValue = active ? 1 : 0; }
                        EditorGUI.showMixedValue = false;
                        visibility.Add(open); enabled.Add(canEdit && (active || property.hasMixedValue));
                    }
                    else if (descriptor.IsGradient && diagnostic == null && PropertyType(property) == ShaderPropertyType.Texture && property.textureDimension == TextureDimension.Tex2D)
                        DrawGradient(materialEditor, material, property, descriptor.Label);
                    else
                    {
                        if (descriptor.IsGradient || descriptor.IsToggleFoldout) EditorGUILayout.HelpBox("Gradient requires Texture2D; ToggleFoldout requires Float or Range.", MessageType.Warning);
                        materialEditor.ShaderProperty(property, descriptor.Label);
                    }
                }
                EditorGUI.indentLevel = indent;
            }
            if (EditorGUI.EndChangeCheck()) foreach (Object target in materialEditor.targets) ValidateMaterial((Material)target);

            if (pipeline != null)
            {
                // URP receives its hidden surface/advanced properties. Visible graph inputs were drawn above.
                pipeline.OnGUI(materialEditor, pipelineProperties.ToArray());
            }
            else
            {
                materialEditor.RenderQueueField(); materialEditor.EnableInstancingField(); materialEditor.DoubleSidedGIField();
            }
        }

        private static void DrawGradient(MaterialEditor editor, Material material, MaterialProperty property, string label)
        {
            editor.TexturePropertySingleLine(new GUIContent(label), property);
            GradientAsset source = GradientAssetService.FindSource(property.textureValue);
            using (new EditorGUI.DisabledScope(true)) EditorGUILayout.ObjectField("Source", source, typeof(GradientAsset), false);
            using (new EditorGUI.DisabledScope(editor.targets.Length != 1))
                if (GUILayout.Button(source ? "Edit GradientX" : "Create / Migrate GradientX")) GradientXEditorWindow.ExposeWindow(material, property.displayName, property.name);
            if (editor.targets.Length != 1) EditorGUILayout.HelpBox("Select one material to edit source gradient data. Texture assignment supports mixed values.", MessageType.Info);
        }

        private static ShaderPropertyFlags PropertyFlags(MaterialProperty property)
        {
#if UNITY_6000_3_OR_NEWER
            return property.propertyFlags;
#else
            return (ShaderPropertyFlags)property.flags;
#endif
        }

        private static ShaderPropertyType PropertyType(MaterialProperty property)
        {
#if UNITY_6000_3_OR_NEWER
            return property.propertyType;
#else
            // The older enums have matching names but different numeric values.
            return (ShaderPropertyType)System.Enum.Parse(typeof(ShaderPropertyType), property.type.ToString());
#endif
        }

        public override void ValidateMaterial(Material material)
        {
            PipelineMaterialAdapter.Validate(material);
            MaterialEditor.ApplyMaterialPropertyDrawers(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader); ValidateMaterial(material);
        }
    }
}
