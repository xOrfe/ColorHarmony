using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XO.ColorHarmony.Editor
{
    internal static class GradientAssetService
    {
        public static GradientAsset FindSource(Texture texture)
        {
            if (!texture) return null;
            string path = AssetDatabase.GetAssetPath(texture);
            return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<GradientAsset>(path);
        }

        public static List<GradientX> ReadLegacy(Material material, string referenceName, string displayName)
        {
            string path = AssetDatabase.GetAssetPath(material);
            if (string.IsNullOrEmpty(path)) return null;
            foreach (UnityEngine.Object item in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (!(item is GradientXDataScriptable legacy)) continue;
                if (item.name != "GradientData {" + displayName + "}" && item.name != "GradientData {" + referenceName + "}") continue;
                var layers = new List<GradientX>();
                foreach (GradientXData data in legacy.Gradients ?? new List<GradientXData>())
                {
                    if (data == null) continue;
                    var colors = new List<ColorKey>();
                    foreach (ColorKeyData key in data.ColorKeys ?? new List<ColorKeyData>())
                        if (key != null) colors.Add(new ColorKey(key.Time, key.Color, key.Brightness));
                    var alphas = new List<AlphaKey>();
                    foreach (AlphaKeyData key in data.AlphaKeys ?? new List<AlphaKeyData>())
                        if (key != null) alphas.Add(new AlphaKey(key.Time, key.Alpha));
                    layers.Add(new GradientX { ColorKeys = colors.ToArray(), AlphaKeys = alphas.ToArray(), InterpolationColorSpace = data.ColorSpaceType, GradientType = data.GradientType });
                }
                return layers;
            }
            return null;
        }

        public static void Copy(GradientAsset source, GradientAsset destination)
        {
            destination.Gradients.Clear();
            foreach (GradientX gradient in source.Gradients) destination.Gradients.Add(gradient?.Clone() ?? new GradientX());
            if (destination.Gradients.Count == 0) destination.Gradients.Add(new GradientX());
            destination.Resolution = source.Resolution;
            destination.Hdr = source.Hdr;
        }

        public static bool CanWrite(GradientAsset asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            return path.StartsWith("Assets/", StringComparison.Ordinal) && AssetDatabase.IsOpenForEdit(asset);
        }

        public static void Bake(GradientAsset asset)
        {
            if (!CanWrite(asset)) throw new InvalidOperationException("Save the gradient as an editable asset under Assets before baking.");
            TextureFormat format = asset.Hdr ? TextureFormat.RGBAHalf : TextureFormat.RGBA32;
            Texture2D texture = asset.BakedTexture;
            if (texture != null && AssetDatabase.GetAssetPath(texture) != AssetDatabase.GetAssetPath(asset))
                throw new InvalidOperationException("The baked texture must belong to this gradient asset.");
            if (texture == null)
            {
                texture = new Texture2D(asset.Resolution, 1, format, false, true) { name = "Gradient Texture" };
                AssetDatabase.AddObjectToAsset(texture, asset);
                Undo.RegisterCreatedObjectUndo(texture, "Create Gradient Texture");
                asset.BakedTexture = texture;
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(texture, "Bake Gradient Texture");
                if (texture.width != asset.Resolution || texture.format != format)
                    texture.Reinitialize(asset.Resolution, 1, format, false);
            }
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = asset.Gradients.Count == 1 && asset.Gradients[0].GradientType == GradientType.Step ? FilterMode.Point : FilterMode.Bilinear;
            texture.SetPixels(asset.Sample(asset.Resolution));
            texture.Apply(false, false);
            EditorUtility.SetDirty(texture); EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
            AssetDatabase.SaveAssetIfDirty(texture);
        }
    }
}

