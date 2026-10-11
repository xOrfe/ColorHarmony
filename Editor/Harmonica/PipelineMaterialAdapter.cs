using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    /// <summary>Optional URP bridge. No compile-time dependency on a render pipeline assembly.</summary>
    internal static class PipelineMaterialAdapter
    {
        private static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public static ShaderGUI Create(Material material)
        {
            if (material.GetTag("RenderPipeline", false) != "UniversalPipeline") return null;
            string kind = material.GetTag("UniversalMaterialType", false);
            string name = kind == "Lit" || kind == "ComplexLit" ? "UnityEditor.ShaderGraphLitGUI" : kind == "Unlit" ? "UnityEditor.ShaderGraphUnlitGUI" : null;
            if (name == null) return null;
            if (!Types.TryGetValue(name, out Type type))
            {
                foreach (Type candidate in TypeCache.GetTypesDerivedFrom<ShaderGUI>())
                {
                    if (candidate.FullName == name) { type = candidate; break; }
                }
                Types[name] = type;
            }
            return type == null ? null : Activator.CreateInstance(type, true) as ShaderGUI;
        }
        public static void Validate(Material material) => Create(material)?.ValidateMaterial(material);
    }
}
