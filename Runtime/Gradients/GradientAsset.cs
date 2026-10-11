using System;
using System.Collections.Generic;
using UnityEngine;

namespace XO.ColorHarmony
{
    /// <summary>Source data and a baked texture that can be referenced by materials and player code.</summary>
    [CreateAssetMenu(menuName = "Color Harmony/Gradient Asset", fileName = "New Gradient")]
    public sealed class GradientAsset : ScriptableObject
    {
        [SerializeField] private int schemaVersion = 1;
        [SerializeField] private List<GradientX> gradients = new List<GradientX> { new GradientX() };
        [SerializeField, Range(2, 4096)] private int resolution = 256;
        [SerializeField] private bool hdr = true;
        [SerializeField] private Texture2D bakedTexture;

        public int SchemaVersion => schemaVersion;
        public List<GradientX> Gradients => gradients;
        public int Resolution { get => Mathf.Clamp(resolution, 2, 4096); set => resolution = Mathf.Clamp(value, 2, 4096); }
        public bool Hdr { get => hdr; set => hdr = value; }
        public Texture2D BakedTexture { get => bakedTexture; set => bakedTexture = value; }

        public Color[] Sample(int width)
        {
            width = Mathf.Max(1, width);
            var pixels = new Color[width];
            int count = 0;
            foreach (GradientX gradient in gradients ?? new List<GradientX>())
            {
                if (gradient == null) continue;
                var snapshot = gradient.CreateSnapshot();
                for (int i = 0; i < width; i++) pixels[i] += snapshot.Evaluate(width == 1 ? 0 : i / (width - 1f));
                count++;
            }
            for (int i = 0; i < width; i++) pixels[i] = count == 0 ? Color.white : pixels[i] / count;
            return pixels;
        }
    }
}
